using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Messages;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Tests.BindingTestExecution;

sealed class BindingExecutionScenario(
    IMessageBus messageBus,
    IDataProducer dataProducer,
    SessionUid sessionUid,
    BindingExecutionContext executionContext
)
{
    public Task BindAsync(string testNodeUid, string executionUid) =>
        this.PublishAsync(
            new AllureTestExecutionBindingMessage(
                executionContext.CurrentCorrelationUid,
                new(testNodeUid),
                new(executionUid)
            )
        );

    public Task FinishExecutionAsync(string executionUid) =>
        this.PublishAsync(
            new AllureTestExecutionFinishMessage(
                executionContext.CurrentCorrelationUid,
                new(executionUid)
            )
        );

    public Task StartTestNodeAsync(string testNodeUid, string displayName = null) =>
        this.PublishAsync(
            TestNodeMessage(
                sessionUid,
                testNodeUid,
                displayName ?? testNodeUid,
                new InProgressTestNodeStateProperty()
            )
        );

    public Task FinishTestNodeAsync(string testNodeUid, string displayName = null) =>
        this.PublishAsync(
            TestNodeMessage(
                sessionUid,
                testNodeUid,
                displayName ?? testNodeUid,
                new PassedTestNodeStateProperty()
            )
        );

    public async Task RunInExecutionAsync(
        string executionUid,
        Func<Task> operation,
        string scopeUid = null
    )
    {
        using (executionContext.Enter(
            new(executionUid),
            new(scopeUid ?? $"scope-{executionUid}")
        ))
        {
            await operation();
        }
    }

    Task PublishAsync(IData message) =>
        messageBus.PublishAsync(dataProducer, message);

    static TestNodeUpdateMessage TestNodeMessage(
        SessionUid sessionUid,
        string testNodeUid,
        string displayName,
        TestNodeStateProperty state
    ) =>
        new(
            sessionUid,
            new TestNode
            {
                Uid = testNodeUid,
                DisplayName = displayName,
                Properties = new(state),
            }
        );
}
