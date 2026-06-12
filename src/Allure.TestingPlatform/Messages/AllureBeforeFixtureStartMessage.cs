using Allure.Net.Commons;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureBeforeFixtureStartMessage(
    SessionUid sessionUid,
    string fixtureUid,
    string scopeId,
    string fixtureName
) :
    AllureFixtureStartMessage(sessionUid, fixtureUid, scopeId, fixtureName)
{
    protected override void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult) =>
        lifecycle.StartBeforeFixture(fixtureResult);
}
