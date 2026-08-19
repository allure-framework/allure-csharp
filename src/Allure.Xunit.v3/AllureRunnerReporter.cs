using System.Threading.Tasks;
using Xunit.Runner.Common;
using Xunit.Sdk;
using Allure.Xunit.Internal;
using Allure.Sdk.Registration;
using Allure.Xunit.Internal.Registration;

namespace Allure.Xunit;

/// <summary>
/// Provides the xUnit.net v3 runner reporter that forwards test lifecycle messages
/// to Allure when the Allure Microsoft Testing Platform runtime is active.
/// </summary>
public class AllureRunnerReporter : IRunnerReporter
{
    readonly static LateBoundReference<AllureMessageHandler> messageHandlerReference = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="AllureRunnerReporter"/> class.
    /// </summary>
    public AllureRunnerReporter() { }

    /// <inheritdoc />
    public bool CanBeEnvironmentallyEnabled => AllureXunitRegistration.IsEnabled;

    /// <inheritdoc />
    public string Description => "Allure runner reporter for xUnit.net v3";

    /// <inheritdoc />
    public bool ForceNoLogo => false;

    /// <inheritdoc />
    public bool IsEnvironmentallyEnabled => AllureXunitRegistration.IsEnabled;

    /// <inheritdoc />
    public string RunnerSwitch => "allure";

    /// <inheritdoc />
    public async ValueTask<IRunnerReporterMessageHandler> CreateMessageHandler(
        IRunnerLogger logger,
        IMessageSink? diagnosticMessageSink
    )
    {
        if (!AllureXunitRegistration.IsEnabled)
        {
            return new DefaultRunnerReporterMessageHandler(logger);
        }

        var allureHandler = new AllureMessageHandler(
            logger,
            AllureXunitRegistration.Current.MessageChannel
        );

        messageHandlerReference.Bind(allureHandler);

        return allureHandler;
    }

    internal static IReadOnlyLateBoundReference<AllureMessageHandler> MessageHandlerReference =>
        messageHandlerReference;
}
