using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Messages;

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

    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allure, fixture);
        });
    }
}