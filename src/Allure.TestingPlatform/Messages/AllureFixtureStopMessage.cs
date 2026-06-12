using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureFixtureStopMessage(
    SessionUid sessionUid,
    string fixtureUid
) :
    RemoveContextMessage(
        "Allure fixture stop",
        "This message reports that an Allure fixture has stopped.",
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
        allure.Lifecycle.StopFixture();
    }
}