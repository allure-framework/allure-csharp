using Allure.Model;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure after-fixture has started.
/// </summary>
public sealed class AllureAfterFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureExecutionStateUid fixtureUid,
    ScopeExecutionStateUid scopeUid,
    string fixtureName
) :
    AllureFixtureStartMessage(correlationUid, fixtureUid, scopeUid, fixtureName)
{
    /// <inheritdoc />
    protected override void StartFixture(IAllureLifecycleApi lifecycle, FixtureResult fixtureResult) =>
        lifecycle.StartTearDownFixture(fixtureResult);
}
