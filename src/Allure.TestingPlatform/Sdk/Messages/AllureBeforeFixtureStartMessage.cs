using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureBeforeFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid,
    ScopeContextUid scopeUid,
    string fixtureName
) :
    AllureFixtureStartMessage(correlationUid, fixtureUid, scopeUid, fixtureName)
{
    protected override void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult) =>
        lifecycle.StartBeforeFixture(fixtureResult);
}
