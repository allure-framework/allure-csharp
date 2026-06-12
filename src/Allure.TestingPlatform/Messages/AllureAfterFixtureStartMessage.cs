using Allure.Net.Commons;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureAfterFixtureStartMessage(
    SessionUid sessionUid,
    string fixtureUid,
    string scopeId,
    string fixtureName
) :
    AllureFixtureStartMessage(sessionUid, fixtureUid, scopeId, fixtureName)
{
    protected override void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult) =>
        lifecycle.StartAfterFixture(fixtureResult);
}
