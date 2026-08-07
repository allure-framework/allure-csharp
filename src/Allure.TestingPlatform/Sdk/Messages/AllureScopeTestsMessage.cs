using System.Collections.Generic;
using System.Collections.Immutable;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports which tests belong to an Allure scope.
/// </summary>
public sealed class AllureScopeTestsMessage(
    CorrelationUid correlationUid,
    ScopeExecutionStateUid scopeUid,
    IEnumerable<TestExecutionStateUid> testUids
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
    public ScopeExecutionStateUid ScopeUid { get; } = scopeUid;

    /// <summary>
    /// Gets the test context identifiers in the scope.
    /// </summary>
    public ImmutableArray<TestExecutionStateUid> TestUids { get; }
        = [.. testUids];
}
