using System.Threading.Tasks;
using Xunit.Runner.Common;
using Xunit.Sdk;

[assembly: RegisterRunnerReporter(typeof(Allure.Xunit.AllureRunnerReporter))]

namespace Allure.Xunit;

public sealed class AllureRunnerReporter : IRunnerReporter
{
    public bool CanBeEnvironmentallyEnabled => false;

    public string Description => "Create Allure results from xUnit.net v3 in-process runs";

    public bool ForceNoLogo => false;

    public bool IsEnvironmentallyEnabled => false;

    public string RunnerSwitch => "allure";

    public ValueTask<IRunnerReporterMessageHandler> CreateMessageHandler(
        IRunnerLogger logger,
        IMessageSink diagnosticMessageSink
    ) => new(new AllureMessageSink(diagnosticMessageSink));
}