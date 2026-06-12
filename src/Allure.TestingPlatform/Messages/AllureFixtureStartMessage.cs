using Allure.Net.Commons;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public abstract class AllureFixtureStartMessage(
    SessionUid sessionUid,
    string fixtureUid,
    string scopeId,
    string fixtureName)
        : CreateContextMessage(
            "Allure fixture start",
            "This message reports that an Allure fixture has started.",
            sessionUid,
            fixtureUid,
            scopeId)
{
    public string ScopeId { get; } = scopeId;

    public string FixtureUid { get; } = fixtureUid;

    public string FixtureName { get; } = fixtureName;

    public override void Mutate(IAllureInfrastructure allure)
    {
        var fixture = this.CreateFixture();
        this.StartFixture(allure.Lifecycle, fixture);
        this.ApplyProperties(allure, fixture);

    }

    protected abstract void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult);

    protected FixtureResult CreateFixture() => new() { name = this.FixtureName };
}
