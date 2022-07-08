using System.Threading;
using Allure.Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.XUnit
{
    public class AllureMessageBus : IMessageBus
    {
        public static readonly AsyncLocal<ITestOutputHelper> TestOutputHelper = new();
        private readonly IMessageBus _inner;

        public AllureMessageBus(IMessageBus inner)
        {
            _inner = inner;
        }

        public void Dispose() => _inner.Dispose();

        public bool QueueMessage(IMessageSinkMessage message)
        {
            switch (message)
            {
                case ITestCaseStarting testCaseStarting:
                    AllureXunitHelper.StartTestCase(testCaseStarting);
                    break;
                case ITestStarting testStarting:
                    var testOutputHelper = new TestOutputHelper();
                    testOutputHelper.Initialize(this, testStarting.Test);
                    TestOutputHelper.Value = testOutputHelper;
                    break;
                case ITestFailed testFailed:
                    AllureXunitHelper.MarkTestCaseAsFailed(testFailed);
                    break;
                case ITestPassed testPassed:
                    AllureXunitHelper.MarkTestCaseAsPassed(testPassed);
                    break;
                case ITestCaseFinished testCaseFinished:
                    AllureXunitHelper.FinishTestCase(testCaseFinished);
                    break;
            }

            return _inner.QueueMessage(message);
        }
    }
}