using Allure.Net.Commons;

namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class AllureFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid,
    ScopeContextUid scopeUid,
    string fixtureName)
        : CreateContextMessage(
            "Allure fixture start",
            "This message reports that an Allure fixture has started.",
            correlationUid,
            fixtureUid,
            scopeUid)
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    public string FixtureName { get; } = fixtureName;

    public override void Mutate(IAllureRuntime allure)
    {
        var fixture = this.CreateFixture();
        this.StartFixture(allure.Lifecycle, fixture);
        this.ApplyProperties(allure, fixture);

    }

    protected abstract void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult);

    protected FixtureResult CreateFixture() => new() { name = this.FixtureName };
}
