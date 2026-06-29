using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

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

    public override void Mutate(ReadyAllureTestingPlatform allureState)
    {
        var fixture = this.CreateFixture();
        this.StartFixture(allureState.Lifecycle, fixture);
        this.ApplyProperties(allureState, fixture);

    }

    protected abstract void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult);

    protected FixtureResult CreateFixture() => new() { name = this.FixtureName };
}
