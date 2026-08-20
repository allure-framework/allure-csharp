using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;

namespace Allure.TestingPlatform.Tests.BindingTestExecution;

sealed class BindingExecutionTestFramework(
    BindingExecutionContext executionContext,
    Func<BindingExecutionScenario, Task> runScenario
) : ITestFramework, IDataProducer
{
    public string Uid => "binding-execution-test-framework";

    public string Version => "1.0.0";

    public string DisplayName => "Binding execution test framework";

    public string Description =>
        "Publishes test-node and execution-binding messages in a configured order.";

    public Type[] DataTypesProduced =>
    [
        typeof(TestNodeUpdateMessage),
        typeof(AllureTestExecutionBindingMessage),
        typeof(AllureTestExecutionFinishMessage),
    ];

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        var request = (RunTestExecutionRequest)context.Request;
        var scenario = new BindingExecutionScenario(
            context.MessageBus,
            this,
            request.Session.SessionUid,
            executionContext
        );

        await runScenario(scenario);
        context.Complete();
    }

    public Task<CreateTestSessionResult> CreateTestSessionAsync(
        CreateTestSessionContext context
    ) =>
        Task.FromResult(new CreateTestSessionResult { IsSuccess = true });

    public Task<CloseTestSessionResult> CloseTestSessionAsync(
        CloseTestSessionContext context
    ) =>
        Task.FromResult(new CloseTestSessionResult { IsSuccess = true });
}
