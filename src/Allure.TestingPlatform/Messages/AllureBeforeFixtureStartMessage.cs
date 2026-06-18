using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Messages;

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
