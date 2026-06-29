using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class AllureFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid,
    ScopeContextUid scopeUid,
    string fixtureName)
        : AllureModelCreateMessage(
            "Allure fixture start",
            "This message reports that an Allure fixture has started.",
            correlationUid,
            fixtureUid,
            scopeUid)
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    public string FixtureName { get; } = fixtureName;

    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        var fixture = this.CreateFixture();
        this.StartFixture(allureRuntime.Lifecycle, fixture);
        this.ApplyProperties(allureRuntime, fixture);

    }

    protected abstract void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult);

    protected FixtureResult CreateFixture() => new() { name = this.FixtureName };
}
