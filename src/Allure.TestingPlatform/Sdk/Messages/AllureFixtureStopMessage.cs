using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureFixtureStopMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid
) :
    RemoveContextMessage(
        "Allure fixture stop",
        "This message reports that an Allure fixture has stopped.",
        correlationUid,
        fixtureUid
    )
{
    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    public override void Mutate(ReadyAllureTestingPlatformRuntime allureState)
    {
        allureState.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allureState, fixture);
        });
        allureState.Lifecycle.StopFixture();
    }
}