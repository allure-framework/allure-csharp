using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports updates for an active Allure fixture.
/// </summary>
public sealed class AllureFixtureUpdateMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid
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
    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    /// <inheritdoc />
    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        allureRuntime.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allureRuntime, fixture);
        });
    }
}
