using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports updates for an active Allure fixture.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="fixtureUid">The identifier of the fixture context to update.</param>
public sealed class AllureFixtureUpdateMessage(
    CorrelationUid correlationUid,
    FixtureExecutionStateUid fixtureUid
) :
    AllureModelUpdateMessage(
        "Allure fixture result update",
        "This message reports that some data needs to be associated with an Allure fixture result.",
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
    }
}
