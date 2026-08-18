using Allure.Model;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure before-fixture has started.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="fixtureUid">The identifier of the fixture context to create.</param>
/// <param name="scopeUid">The identifier of the scope that owns the fixture.</param>
/// <param name="fixtureName">The fixture name.</param>
public sealed class AllureBeforeFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureExecutionStateUid fixtureUid,
    ScopeExecutionStateUid scopeUid,
    string fixtureName
) :
    AllureFixtureStartMessage(correlationUid, fixtureUid, scopeUid, fixtureName)
{
    /// <inheritdoc />
    protected override void StartFixture(IAllureLifecycleApi lifecycle, FixtureResult fixtureResult) =>
        lifecycle.StartSetUpFixture(fixtureResult);
}
