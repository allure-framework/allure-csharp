using Allure.Model;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that start an Allure fixture.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="fixtureUid">The identifier of the fixture context to create.</param>
/// <param name="scopeUid">The identifier of the scope that owns the fixture.</param>
/// <param name="fixtureName">The fixture name.</param>
public abstract class AllureFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureExecutionStateUid fixtureUid,
    ScopeExecutionStateUid scopeUid,
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
    public ScopeExecutionStateUid ScopeUid { get; } = scopeUid;

    /// <summary>
    /// Gets the fixture context identifier.
    /// </summary>
    public FixtureExecutionStateUid FixtureUid { get; } = fixtureUid;

    /// <summary>
    /// Gets the fixture name.
    /// </summary>
    public string FixtureName { get; } = fixtureName;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        var fixture = this.CreateFixture();
        this.ApplyProperties(allureRuntime, fixture);
        this.StartFixture(allureRuntime.LifecycleApi, fixture);
    }

    /// <summary>
    /// Starts the fixture in the Allure lifecycle.
    /// </summary>
    /// <param name="lifecycle">The Allure lifecycle API.</param>
    /// <param name="fixtureResult">The fixture result to start.</param>
    protected abstract void StartFixture(IAllureLifecycleApi lifecycle, FixtureResult fixtureResult);

    /// <summary>
    /// Creates the fixture result started by this message.
    /// </summary>
    /// <returns>The new fixture result.</returns>
    protected FixtureResult CreateFixture() => new() { Name = this.FixtureName };
}
