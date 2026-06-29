using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureFixtureStopMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid
) :
    AllureModelRemoveMessage(
        "Allure fixture stop",
        "This message reports that an Allure fixture has stopped.",
        correlationUid,
        fixtureUid
    )
{
    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        allureRuntime.Lifecycle.UpdateFixture((fixture) =>
        {
            this.ApplyProperties(allureRuntime, fixture);
        });
        allureRuntime.Lifecycle.StopFixture();
    }
}