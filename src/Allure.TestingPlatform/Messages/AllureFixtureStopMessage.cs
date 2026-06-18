using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Messages;

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

    public override void Mutate(IAllureInfrastructure allure)
    {
        allure.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allure, fixture);
        });
        allure.Lifecycle.StopFixture();
    }
}