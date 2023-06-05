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
            AllurePatcher.PatchXunit(logger);
            var sink = new AllureMessageSink(logger);
            if (CurrentSink is null)
            {
                CurrentSink = sink;
            }
            return sink;
        }

        internal static void Log(string message, params object[] args)
        {
            AllureRunnerReporter.Instance.logger.LogImportantMessage(
                StackFrameInfo.None,
                $"{i} {System.Threading.Thread.CurrentThread.ManagedThreadId} {message}",
                args
            );
            i++;
        }

        static int i = 0;

        internal static AllureRunnerReporter Instance { get; private set; }
        internal static AllureMessageSink CurrentSink { get; private set; }
        IRunnerLogger logger;
    }
}