using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

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

    public override void Mutate(ReadyAllureTestingPlatform allureState)
    {
        allureState.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allureState, fixture);
        });
        allureState.Lifecycle.StopFixture();
    }
}