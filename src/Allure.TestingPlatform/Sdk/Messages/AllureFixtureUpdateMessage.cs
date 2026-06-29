using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

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
    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureState)
    {
        allureState.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allureState, fixture);
        });
    }
}