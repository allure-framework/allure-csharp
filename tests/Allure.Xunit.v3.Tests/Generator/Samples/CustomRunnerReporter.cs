using System;
using System.IO;
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
            IMessageSink diagnosticMessageSink
        )
        {
            var markerPath = Environment.GetEnvironmentVariable("__ALLURE_MARKER_FILE__");
            if (!string.IsNullOrEmpty(markerPath) && File.Exists(markerPath))
            {
                File.WriteAllText(markerPath, "custom reporter works");
            }

            return new(new DefaultRunnerReporterMessageHandler(logger));
        }
    }

    public class TestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
