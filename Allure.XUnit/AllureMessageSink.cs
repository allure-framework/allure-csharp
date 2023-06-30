using Allure.Net.Commons;
using Allure.Net.Commons.Steps;
using Allure.Xunit;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;
using Xunit.Abstractions;

namespace Allure.XUnit
{
    public class AllureMessageSink : TestMessageSink
    {
        IRunnerLogger logger;
        Dictionary<ITest, AllureXunitTestResultAccessor> allureTestData = new();

        public AllureMessageSink(IRunnerLogger logger)
        {
            this.logger = logger;

            this.Runner.TestAssemblyExecutionStartingEvent +=
                this.OnTestAssemblyExecutionStarting;

            this.Execution.TestStartingEvent += this.OnTestStarting;
            this.Execution.TestClassConstructionFinishedEvent +=
                this.OnTestClassConstructionFinished;
            this.Execution.TestFailedEvent += this.OnTestFailed;
            this.Execution.TestPassedEvent += this.OnTestPassed;
            this.Execution.TestFinishedEvent += this.OnTestFinished;
            this.Execution.TestCaseFinishedEvent+= this.OnTestCaseFinished;
        }

        void OnTestAssemblyExecutionStarting(
            MessageHandlerArgs<ITestAssemblyExecutionStarting> args
        )
        {
            args.Message.ExecutionOptions.SetSynchronousMessageReporting(true);
        }

        internal void OnTestArgumentsCreated(ITest test, object[] arguments)
        {
            var accessor = this.GetOrCreateAllureResultAccessor(test);
            accessor.Arguments = arguments;
        }

        void OnTestStarting(MessageHandlerArgs<ITestStarting> args)
        {
            var message = args.Message;
            var test = message.Test;
            var accessor = this.GetOrCreateAllureResultAccessor(test);

            CoreStepsHelper.TestResultAccessor = accessor;

            if (message.TestMethod.Method.IsStatic)
            {
                accessor.TestResult = AllureXunitHelper.StartStaticAllureTestCase(test);
            }
            else
            {
                accessor.TestResultContainer = AllureXunitHelper.StartNewAllureContainer(
                    message.TestClass.Class.Name
                );
            }
        }

        void OnTestClassConstructionFinished(
            MessageHandlerArgs<ITestClassConstructionFinished> args
        )
        {
            var test = args.Message.Test;
            var accessor = this.allureTestData[test];
            var container = accessor.TestResultContainer;
            if (accessor.TestResult is null && container is not null)
            {
                accessor.TestResult = AllureXunitHelper.StartAllureTestCase(
                    test,
                    container
                );
            }
        }

        void OnTestFailed(MessageHandlerArgs<ITestFailed> args)
        {
            var message = args.Message;
            var test = message.Test;
            var testResult = this.allureTestData[test].TestResult;

            if (testResult is not null)
            {
                AllureXunitHelper.ApplyTestFailure(testResult, message);
            }
        }

        void OnTestPassed(MessageHandlerArgs<ITestPassed> args)
        {
            var message = args.Message;
            var test = message.Test;
            var testResult = this.allureTestData[test].TestResult;

            if (testResult is not null)
            {
                AllureXunitHelper.ApplyTestSuccess(testResult, message);
            }
        }

        void OnTestFinished(MessageHandlerArgs<ITestFinished> args)
        {
            var test = args.Message.Test;
            var accessor = this.allureTestData[test];
            this.allureTestData.Remove(test);
            var testResult = accessor.TestResult;
            if (testResult is not null)
            {
                this.AddAllureParameters(testResult, test, accessor.Arguments);
                AllureXunitHelper.ReportTestCase(testResult);

                var container = accessor.TestResultContainer;
                if (container is not null)
                {
                    AllureXunitHelper.ReportTestContainer(container);
                }
            }
        }

        void OnTestCaseFinished(MessageHandlerArgs<ITestCaseFinished> args)
        {
            var testCase = args.Message.TestCase;
            if (testCase.SkipReason != null)
            {
                AllureXunitHelper.ReportSkippedTestCase(testCase);
            }
        }

        AllureXunitTestResultAccessor GetOrCreateAllureResultAccessor(ITest test)
        {
            if (!this.allureTestData.TryGetValue(test, out var accessor))
            {
                accessor = new AllureXunitTestResultAccessor();
                this.allureTestData[test] = accessor;
            }
            return accessor;
        }

        void AddAllureParameters(
            TestResult testResult,
            ITest test,
            object[] arguments
        )
        {
            var testCase = test.TestCase;
            var parameters = testCase.TestMethod.Method.GetParameters();
            arguments ??= testCase.TestMethodArguments ?? Array.Empty<object>();

            if (parameters.Any() && !arguments.Any())
            {
                this.LogUnreportedTheoryArgs(test.DisplayName);
            }
            else
            {
                AllureXunitHelper.ApplyTestParameters(testResult, parameters, arguments);
            }
        }

        void LogUnreportedTheoryArgs(string testName)
        {
            var message = $"Unable to attach arguments of {testName} to " +
                "allure report";
#if !DEBUG
            message += ". You may try to compile the project in debug mode " +
                "as a workaround";
#endif
            this.logger.LogWarning(message);
        }
    }
}