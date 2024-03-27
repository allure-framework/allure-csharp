using Xunit;
using Xunit.Abstractions;

#nullable enable

namespace Allure.XUnit
{
    public class AllureRunnerReporter : IRunnerReporter
    {
        public string Description { get; }
            = "Creates allure input files for xunit tests";

        public bool IsEnvironmentallyEnabled { get; } = true;

        public string RunnerSwitch { get; } = "allure";

        public IMessageSink CreateMessageHandler(IRunnerLogger logger) =>
            AllureXunitFacade.CreateAllureXunitMessageHandler(logger);
    }
}