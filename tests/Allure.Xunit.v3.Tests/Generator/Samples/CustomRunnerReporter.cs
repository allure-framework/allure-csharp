using System;
using Allure.Net.Commons;
using System.Threading.Tasks;
using Xunit;
using Xunit.Runner.Common;
using Xunit.Sdk;
using Xunit.v3;

[assembly: RegisterRunnerReporter(typeof(Allure.Xunit.v3.Tests.Samples.Generator.CustomRunnerReporter.CustomRunnerReporter))]

namespace Allure.Xunit.v3.Tests.Samples.Generator.CustomRunnerReporter
{
    public sealed class CustomRunnerReporter : IRunnerReporter
    {
        public bool CanBeEnvironmentallyEnabled => true;

        public string Description => "Custom runner reporter for generator tests";

        public bool ForceNoLogo => false;

        public bool IsEnvironmentallyEnabled => true;

        public string RunnerSwitch => "custom-generator-test";

        public ValueTask<IRunnerReporterMessageHandler> CreateMessageHandler(
            IRunnerLogger logger,
            IMessageSink? diagnosticMessageSink
        )
        {
            AllureApi.AddGlobalError("custom reporter works");

            return new(new DefaultRunnerReporterMessageHandler(logger));
        }
    }

    public class TestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
