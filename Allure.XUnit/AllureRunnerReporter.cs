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
            AllureXunitPatcher.PatchXunit(logger);
            var sink = new AllureMessageSink(logger);
            CurrentSink ??= sink;
            return sink;
        }

        internal static AllureMessageSink CurrentSink { get; private set; }
    }
}