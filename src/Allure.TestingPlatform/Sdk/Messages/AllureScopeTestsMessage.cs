using System.Collections.Generic;
using System.Collections.Immutable;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports which tests belong to an Allure scope.
/// </summary>
public sealed class AllureScopeTestsMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid,
    IEnumerable<TestContextUid> testUids
) :
    AllureCorrelatedMessage(
        "Allure scope tests detected",
        "This message reports that an Allure scope contains a set of tests.",
        correlationUid
    )
{
    /// <summary>
    /// Gets the scope context identifier.
    /// </summary>
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    /// <summary>
    /// Gets the test context identifiers in the scope.
    /// </summary>
    public ImmutableArray<TestContextUid> TestUids { get; }
        = testUids.ToImmutableArray();
}
