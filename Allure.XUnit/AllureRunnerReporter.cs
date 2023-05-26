using Xunit;
using Xunit.Abstractions;

namespace Allure.XUnit
{
    public class AllureRunnerReporter : IRunnerReporter
    {
        public string Description { get; } = "allure-xunit";

        public bool IsEnvironmentallyEnabled { get; } = true;

        public string RunnerSwitch { get; } = "allure";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger)
        {
            Instance = this;
            this.logger = logger;
            return new AllureMessageSink(logger);
        }

        internal static void Log(string message)
        {
            AllureRunnerReporter.Instance.logger.LogImportantMessage(
                StackFrameInfo.None,
                $"{i} {System.Threading.Thread.CurrentThread.ManagedThreadId} {message}"
            );
            i++;
        }

        static int i = 0;

        internal static AllureRunnerReporter Instance { get; private set; }
        IRunnerLogger logger;
    }
}