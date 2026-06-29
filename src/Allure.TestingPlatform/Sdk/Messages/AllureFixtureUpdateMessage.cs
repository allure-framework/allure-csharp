using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureFixtureUpdateMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid
) :
    MutateModelMessage(
        "Allure fixture result update",
        "This message reports that some data needs to be associated with an Allure fixture result.",
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
    }
}