using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Tests.Stubs;

public class TestFrameworkStub(params Func<SessionUid, IData>[] messageFactories) : ITestFramework, IDataProducer
{
    public string Uid => "foo";
    public string Version => "1.0.0";
    public string DisplayName => "";
    public string Description => "";

    public Type[] DataTypesProduced { get; } = [
        typeof(TestNodeUpdateMessage),
        typeof(SessionFileArtifact),

        typeof(AllureScopeStartMessage),
        typeof(AllureScopeTestsMessage),
        typeof(AllureScopeStopMessage),

        typeof(AllureSetUpFixtureStartMessage),
        typeof(AllureTearDownFixtureStartMessage),
        typeof(AllureFixtureUpdateMessage),
        typeof(AllureFixtureStopMessage),


        typeof(AllureTestUpdateMessage),
    ];

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        var request = (RunTestExecutionRequest)context.Request;

        var session = request.Session.SessionUid;

        foreach (var messageFactory in messageFactories)
        {
            await context.MessageBus.PublishAsync(this, messageFactory(session));
        }

        context.Complete();
    }

    public async Task<CreateTestSessionResult> CreateTestSessionAsync(CreateTestSessionContext context)
    {
        return new() { IsSuccess = true };
    }

    public async Task<CloseTestSessionResult> CloseTestSessionAsync(CloseTestSessionContext context)
    {
        return new() { IsSuccess = true };
    }
}