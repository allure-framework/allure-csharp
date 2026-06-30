using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that start an Allure fixture.
/// </summary>
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
    /// <summary>
    /// Gets the scope that owns the fixture.
    /// </summary>
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    /// <summary>
    /// Gets the fixture context identifier.
    /// </summary>
    public FixtureContextUid FixtureUid { get; } = fixtureUid;

    /// <summary>
    /// Gets the fixture name.
    /// </summary>
    public string FixtureName { get; } = fixtureName;

    /// <inheritdoc />
    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        var fixture = this.CreateFixture();
        this.StartFixture(allureRuntime.Lifecycle, fixture);
        this.ApplyProperties(allureRuntime, fixture);

    }

    /// <summary>
    /// Starts the fixture in the Allure lifecycle.
    /// </summary>
    protected abstract void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult);

    /// <summary>
    /// Creates the fixture result started by this message.
    /// </summary>
    protected FixtureResult CreateFixture() => new() { name = this.FixtureName };
}
