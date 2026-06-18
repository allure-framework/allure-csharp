using System.Collections.Generic;
using System.Collections.Immutable;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public sealed class AllureTestsScopeMessage(
    SessionUid sessionUid,
    string scopeUid,
    IEnumerable<string> testUids
) :
    DataWithSessionUid(
        "Allure scope tests detected",
        "This message reports that an Allure scope contains a set of tests.",
        sessionUid
    )
{
    public string ScopeUid => scopeUid;

    public ImmutableArray<string> TestUids { get; }
        = testUids.ToImmutableArray();
}