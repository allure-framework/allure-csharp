using System.Threading.Tasks;
using Xunit.Runner.Common;
using Xunit.Sdk;
using Allure.Xunit.Internal;

namespace Allure.Xunit;

public class AllureRunnerReporter : IRunnerReporter
{
    readonly IRunnerReporter? xunitSelectedReporter;

    public AllureRunnerReporter() { }

    public AllureRunnerReporter(IRunnerReporter xunitSelectedReporter) =>
        this.xunitSelectedReporter = xunitSelectedReporter;

    public bool CanBeEnvironmentallyEnabled => AllureXunitMtpServices.IsAllureAlive;

    public string Description => "Allure runner reporter for xUnit.net v3";

    public bool ForceNoLogo => false;

    public bool IsEnvironmentallyEnabled => this.CanBeEnvironmentallyEnabled;

    public string RunnerSwitch => "allure";

    public async ValueTask<IRunnerReporterMessageHandler> CreateMessageHandler(
        IRunnerLogger logger,
        IMessageSink? diagnosticMessageSink
    )
    {
        if (!AllureXunitMtpServices.IsAllureAlive)
        {
            return new DefaultRunnerReporterMessageHandler(logger);
        }

        var allureHandler = new AllureMessageHandler(logger, AllureXunitMtpServices.MessageBus);

        CurrentMessageHandler = allureHandler;

        return this.xunitSelectedReporter is null or AllureRunnerReporter
            ?  allureHandler
            : new CompositeMessageHandler(
                await this.xunitSelectedReporter.CreateMessageHandler(logger, diagnosticMessageSink),
                allureHandler
            );
    }

    internal static AllureMessageHandler? CurrentMessageHandler { get; private set; }
}
