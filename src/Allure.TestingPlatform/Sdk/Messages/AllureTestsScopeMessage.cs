using System.Collections.Generic;
using System.Collections.Immutable;
using Allure.TestingPlatform.Sdk.Runtime.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

public sealed class AllureTestsScopeMessage(
    CorrelationUid correlationUid,
    ScopeContextUid scopeUid,
    IEnumerable<TestContextUid> testUids
) :
    DataWithCorrelationUid(
        "Allure scope tests detected",
        "This message reports that an Allure scope contains a set of tests.",
        correlationUid
    )
{
    public ScopeContextUid ScopeUid { get; } = scopeUid;

    public ImmutableArray<TestContextUid> TestUids { get; }
        = testUids.ToImmutableArray();
}