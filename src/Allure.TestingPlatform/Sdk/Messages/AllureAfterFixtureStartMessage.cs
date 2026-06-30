using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure after-fixture has started.
/// </summary>
public sealed class AllureAfterFixtureStartMessage(
    CorrelationUid correlationUid,
    FixtureContextUid fixtureUid,
    ScopeContextUid scopeUid,
    string fixtureName
) :
    AllureFixtureStartMessage(correlationUid, fixtureUid, scopeUid, fixtureName)
{
    /// <inheritdoc />
    protected override void StartFixture(AllureLifecycle lifecycle, FixtureResult fixtureResult) =>
        lifecycle.StartAfterFixture(fixtureResult);
}
