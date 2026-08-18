using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Tests.OperationTests;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Extensions.TestFramework;
using Microsoft.Testing.Platform.Requests;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Tests.Stubs;

sealed class OperationTestFrameworkStub(
    OperationExecutionContext executionContext,
    OperationTarget target,
    Func<Task> operation
) : ITestFramework, IDataProducer
{
    public string Uid => "operation-test-framework";

    public string Version => "1.0.0";

    public string DisplayName => "Operation test framework";

    public string Description => "Invokes one Allure API operation inside a test framework scope.";

    public Type[] DataTypesProduced =>
    [
        typeof(TestNodeUpdateMessage),
        typeof(AllureScopeStartMessage),
        typeof(AllureScopeTestsMessage),
        typeof(AllureScopeStopMessage),
        typeof(AllureBeforeFixtureStartMessage),
        typeof(AllureFixtureStopMessage),
        typeof(AllureStepStartMessage),
        typeof(AllureStepStopMessage),
    ];

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    public async Task ExecuteRequestAsync(ExecuteRequestContext context)
    {
        var request = (RunTestExecutionRequest)context.Request;
        var sessionUid = request.Session.SessionUid;

        await context.MessageBus.PublishAsync(
            this,
            new AllureScopeStartMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.ScopeUid
            )
        );
        await context.MessageBus.PublishAsync(
            this,
            new AllureScopeTestsMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.ScopeUid,
                [executionContext.TestUid]
            )
        );
        await context.MessageBus.PublishAsync(
            this,
            TestMessage(sessionUid, new InProgressTestNodeStateProperty())
        );

        if (target is OperationTarget.Test)
        {
            using (executionContext.Enter(OperationTarget.Test))
            {
                await operation();
            }
        }
        else if (target is OperationTarget.Fixture)
        {
            await this.ExecuteInFixtureAsync(context);
        }
        else
        {
            await this.ExecuteInStepAsync(context);
        }

        await context.MessageBus.PublishAsync(
            this,
            TestMessage(sessionUid, new PassedTestNodeStateProperty())
        );
        await context.MessageBus.PublishAsync(
            this,
            new AllureScopeStopMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.ScopeUid
            )
        );

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

    async Task ExecuteInFixtureAsync(ExecuteRequestContext context)
    {
        await context.MessageBus.PublishAsync(
            this,
            new AllureBeforeFixtureStartMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.FixtureUid,
                executionContext.ScopeUid,
                "fixture"
            )
        );

        using (executionContext.Enter(OperationTarget.Fixture))
        {
            await operation();
        }

        await context.MessageBus.PublishAsync(
            this,
            new AllureFixtureStopMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.FixtureUid
            )
        );
    }

    async Task ExecuteInStepAsync(ExecuteRequestContext context)
    {
        await context.MessageBus.PublishAsync(
            this,
            new AllureStepStartMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.TestUid,
                executionContext.StepUid,
                "step"
            )
        );

        using (executionContext.Enter(OperationTarget.Step))
        {
            await operation();
        }

        await context.MessageBus.PublishAsync(
            this,
            new AllureStepStopMessage(
                executionContext.CurrentCorrelationUid,
                executionContext.StepUid
            )
        );
    }

    TestNodeUpdateMessage TestMessage(
        SessionUid sessionUid,
        TestNodeStateProperty state
    ) =>
        new(
            sessionUid,
            new TestNode
            {
                Uid = executionContext.TestUid.Value,
                DisplayName = "test",
                Properties = new(state),
            }
        );
}
