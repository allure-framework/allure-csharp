using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure fixture has stopped.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="fixtureUid">The identifier of the fixture context to stop.</param>
public sealed class AllureFixtureStopMessage(
    CorrelationUid correlationUid,
    FixtureExecutionStateUid fixtureUid
) :
    AllureModelRemoveMessage(
        "Allure fixture stop",
        "This message reports that an Allure fixture has stopped.",
        correlationUid,
        fixtureUid
    )
{
    /// <summary>
    /// Gets the fixture context identifier.
    /// </summary>
    public FixtureExecutionStateUid FixtureUid { get; } = fixtureUid;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        allureRuntime.ModelApi.UpdateFixtureResult((fixture) =>
        {
            this.ApplyProperties(allureRuntime, fixture);
        });
        allureRuntime.LifecycleApi.StopFixture();
    }
}
