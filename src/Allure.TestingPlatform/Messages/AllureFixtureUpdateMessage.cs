using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureFixtureUpdateMessage(
    SessionUid sessionUid,
    string fixtureUid
) :
    MutateModelMessage(
        "Allure fixture result update",
        "This message reports that some data needs to be associated with an Allure fixture result.",
        sessionUid,
        fixtureUid
    )
{
    public string FixtureUid { get; } = fixtureUid;

    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allure, fixture);
        });
    }
}