using Microsoft.Testing.Platform.Services;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Tests.Stubs;

class TestSessionContextStub : ITestSessionContext
{
    public SessionUid SessionUid { get; set; } = default;

    public CancellationToken CancellationToken { get; set; } = CancellationToken.None;
}