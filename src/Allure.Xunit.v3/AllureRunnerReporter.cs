using System.Threading.Tasks;
using Xunit.Runner.Common;
using Xunit.Sdk;
using Allure.Xunit.Internal;

namespace Allure.Xunit;

/// <summary>
/// Provides the xUnit.net v3 runner reporter that forwards test lifecycle messages
/// to Allure when the Allure Microsoft Testing Platform runtime is active.
/// </summary>
public class AllureRunnerReporter : IRunnerReporter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AllureRunnerReporter"/> class.
    /// </summary>
    public AllureRunnerReporter() { }

    /// <inheritdoc />
    public bool CanBeEnvironmentallyEnabled => AllureXunitMtpServices.IsAllureAlive;

    /// <inheritdoc />
    public string Description => "Allure runner reporter for xUnit.net v3";

    /// <inheritdoc />
    public bool ForceNoLogo => false;

    /// <inheritdoc />
    public bool IsEnvironmentallyEnabled => this.CanBeEnvironmentallyEnabled;

    /// <inheritdoc />
    public string RunnerSwitch => "allure";

    /// <inheritdoc />
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

        return allureHandler;
    }

    internal static AllureMessageHandler? CurrentMessageHandler { get; private set; }
}
