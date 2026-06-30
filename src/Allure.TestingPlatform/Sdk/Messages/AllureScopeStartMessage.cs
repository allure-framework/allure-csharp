using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure scope has started.
/// </summary>
public sealed class AllureScopeStartMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid,
    ScopeContextUid? parentScopeUid = null
) :
    AllureModelCreateMessage(
        "Allure scope start",
        "This message reports that an Allure test scope has started.",
        correlationUid,
        scopeUid,
        parentScopeUid
    )
{
    /// <summary>
    /// Gets the scope context identifier.
    /// </summary>
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    /// <summary>
    /// Gets the parent scope context identifier, if one exists.
    /// </summary>
    public ScopeContextUid? ParentScopeUid { get; } = parentScopeUid;

    /// <inheritdoc />
    public override void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime)
    {
        allureRuntime.Lifecycle.StartTestContainer(new() { uuid = IdFunctions.CreateUUID() });
    }
}
