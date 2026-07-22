using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class FacadeForwardingTests
{
    [Test]
    public async Task SyncCallUsesCurrentEndpoint()
    {
        var sync = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: sync.Instance));

        AllureApi.SetTestName("new name");

        await Assert.That(sync.SingleCall.Method.Name).IsEqualTo("SetTestName");
        await Assert.That(sync.SingleCall.Arguments[0]).IsEqualTo("new name");
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
        await Assert.That(scope.GlobalResolutionCount).IsEqualTo(0);
    }

    [Test]
    public async Task AsyncCallUsesAsyncOperationsAndForwardsCancellationToken()
    {
        var operations = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(@async: operations.Instance));
        using var cancellation = new CancellationTokenSource();

        await AllureApi.SetDescriptionAsync("description", cancellation.Token);

        await Assert.That(operations.SingleCall.Method.Name).IsEqualTo("SetDescriptionAsync");
        await Assert.That(operations.SingleCall.Arguments[0]).IsEqualTo("description");
        await Assert.That(operations.SingleCall.Arguments[1]).IsEqualTo(cancellation.Token);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task MissingCurrentEndpointMakesSyncAndAsyncCallsNoOps()
    {
        using var scope = FacadeTestEnvironment.Use();

        AllureApi.SetName("ignored");
        await AllureApi.SetNameAsync("ignored");

    }

    [Test]
    public async Task SyncOperationExceptionIsPropagated()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var expected = new InvalidOperationException("operation failed");
        operations.Handler = (_, _) => throw expected;
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: operations.Instance));

        await Assert.That(() => AllureApi.SetName("name"))
            .Throws<InvalidOperationException>()
            .WithMessage("operation failed");
    }

    [Test]
    public async Task AsyncOperationExceptionIsPropagated()
    {
        var operations = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        var expected = new InvalidOperationException("operation failed");
        operations.Handler = (_, _) => Task.FromException(expected);
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(@async: operations.Instance));

        await Assert.That(() => AllureApi.SetNameAsync("name"))
            .Throws<InvalidOperationException>()
            .WithMessage("operation failed");
    }

    [Test]
    public async Task ConvenienceLabelOverloadBuildsExpectedModel()
    {
        var sync = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: sync.Instance));

        AllureApi.SetOwner("Ada");

        await Assert.That(sync.SingleCall.Method.Name).IsEqualTo("SetLabel");
        await Assert.That(sync.SingleCall.Arguments[0]).IsEqualTo(LabelName.Owner);
        await Assert.That(sync.SingleCall.Arguments[1]).IsEqualTo("Ada");
    }

    [Test]
    public async Task ConvenienceLinkOverloadBuildsExpectedModel()
    {
        var sync = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: sync.Instance));

        AllureApi.AddIssue("https://issues/42", "Issue 42");

        var link = (Link)sync.SingleCall.Arguments[0]!;
        await Assert.That(sync.SingleCall.Method.Name).IsEqualTo("AddLink");
        await Assert.That(link.Url).IsEqualTo("https://issues/42");
        await Assert.That(link.Name).IsEqualTo("Issue 42");
        await Assert.That(link.Type).IsEqualTo(LinkType.Issue);
    }

    [Test]
    public async Task AttachmentUsesCurrentEndpointAndPreservesStream()
    {
        var sync = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(sync: sync.Instance));
        using var content = new MemoryStream([1, 2, 3]);

        AllureApi.AddAttachment("data", content, "application/example", ".bin");

        await Assert.That(sync.SingleCall.Method.Name).IsEqualTo("AddAttachment");
        await Assert.That(sync.SingleCall.Arguments[1]).IsSameReferenceAs(content);
        await Assert.That(sync.SingleCall.Arguments[2]).IsEqualTo("application/example");
        await Assert.That(sync.SingleCall.Arguments[3]).IsEqualTo(".bin");
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task GlobalAttachmentUsesGlobalEndpointOnly()
    {
        var current = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var global = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(
            new TestApiEndpoint(sync: current.Instance),
            new TestApiEndpoint(sync: global.Instance)
        );
        using var content = new MemoryStream([1]);

        AllureApi.AddGlobalAttachment("global", content, "text/plain", ".txt");

        await Assert.That(current.Calls).IsEmpty();
        await Assert.That(global.SingleCall.Method.Name).IsEqualTo("AddGlobalAttachment");
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(0);
        await Assert.That(scope.GlobalResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task AsyncGlobalErrorUsesGlobalEndpointAndToken()
    {
        var global = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(global: new TestApiEndpoint(@async: global.Instance));
        using var cancellation = new CancellationTokenSource();

        await AllureApi.AddGlobalErrorAsync("failure", cancellation.Token);

        var error = (GlobalError)global.SingleCall.Arguments[0]!;
        await Assert.That(global.SingleCall.Method.Name).IsEqualTo("AddGlobalErrorAsync");
        await Assert.That(error.Message).IsEqualTo("failure");
        await Assert.That(global.SingleCall.Arguments[1]).IsEqualTo(cancellation.Token);
        await Assert.That(scope.GlobalResolutionCount).IsEqualTo(1);
    }
}
