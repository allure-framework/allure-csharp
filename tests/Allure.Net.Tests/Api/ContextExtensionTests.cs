using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class ContextExtensionTests
{
    [Test]
    public async Task SyncStepParameterExtensionsCoverEveryOverload()
    {
        var context = CreateSyncContext<IAllureStepContext>(out var calls);

        context.AddParameter("plain", "value");
        context.AddParameter("mode", "value", ParameterMode.Masked);
        context.AddParameterFromObject("object", 42);
        context.AddParameterFromObject("object-mode", 42, ParameterMode.Hidden);

        var additions = AddParameterCalls(calls);
        await AssertParameters(additions, "plain", "mode", "object", "object-mode");
        await Assert.That(Parameter(additions[1]).Mode).IsEqualTo(ParameterMode.Masked);
        await Assert.That(Parameter(additions[2]).Value).IsEqualTo("context:42");
        await Assert.That(Parameter(additions[3]).Mode).IsEqualTo(ParameterMode.Hidden);
    }

    [Test]
    public async Task SyncFixtureParameterExtensionsCoverEveryOverload()
    {
        var context = CreateSyncContext<IAllureFixtureContext>(out var calls);

        context.AddParameter("plain", "value");
        context.AddParameter("mode", "value", ParameterMode.Masked);
        context.AddParameterFromObject("object", 42);
        context.AddParameterFromObject("object-mode", 42, ParameterMode.Hidden);

        var additions = AddParameterCalls(calls);
        await AssertParameters(additions, "plain", "mode", "object", "object-mode");
        await Assert.That(Parameter(additions[2]).Value).IsEqualTo("context:42");
    }

    [Test]
    public async Task AsyncStepExtensionsCoverEveryOverload()
    {
        var context = CreateAsyncContext<IAllureAsyncStepContext>(out var calls);
        using var cancellation = new CancellationTokenSource();
        var parameter = new Parameter { Name = "model", Value = "value" };

        await context.SetNameAsync("renamed");
        await context.AddParameterAsync(parameter);
        await context.AddParameterAsync("plain", "value");
        await context.AddParameterAsync("token", "value", cancellation.Token);
        await context.AddParameterAsync("mode", "value", ParameterMode.Masked);
        await context.AddParameterAsync("mode-token", "value", ParameterMode.Hidden, cancellation.Token);
        await context.AddParameterFromObjectAsync("object", 42);
        await context.AddParameterFromObjectAsync("object-token", 42, cancellation.Token);
        await context.AddParameterFromObjectAsync("object-mode", 42, ParameterMode.Masked);
        await context.AddParameterFromObjectAsync("object-mode-token", 42, ParameterMode.Hidden, cancellation.Token);

        var operations = OperationCalls(calls);
        await Assert.That(operations.Count).IsEqualTo(10);
        await Assert.That(operations[0].Method.Name).IsEqualTo("SetNameAsync");
        await Assert.That(operations[0].Arguments[1]).IsEqualTo(CancellationToken.None);
        await Assert.That(operations[1].Arguments[0]).IsSameReferenceAs(parameter);
        await Assert.That(operations[3].Arguments[1]).IsEqualTo(cancellation.Token);
        await Assert.That(Parameter(operations[5]).Mode).IsEqualTo(ParameterMode.Hidden);
        await Assert.That(Parameter(operations[6]).Value).IsEqualTo("context:42");
        await Assert.That(operations[7].Arguments[1]).IsEqualTo(cancellation.Token);
        await Assert.That(Parameter(operations[9]).Mode).IsEqualTo(ParameterMode.Hidden);
    }

    [Test]
    public async Task AsyncFixtureExtensionsCoverEveryOverload()
    {
        var context = CreateAsyncContext<IAllureAsyncFixtureContext>(out var calls);
        using var cancellation = new CancellationTokenSource();
        var parameter = new Parameter { Name = "model", Value = "value" };

        await context.SetNameAsync("renamed");
        await context.AddParameterAsync(parameter);
        await context.AddParameterAsync("plain", "value");
        await context.AddParameterAsync("token", "value", cancellation.Token);
        await context.AddParameterAsync("mode", "value", ParameterMode.Masked);
        await context.AddParameterAsync("mode-token", "value", ParameterMode.Hidden, cancellation.Token);
        await context.AddParameterFromObjectAsync("object", 42);
        await context.AddParameterFromObjectAsync("object-token", 42, cancellation.Token);
        await context.AddParameterFromObjectAsync("object-mode", 42, ParameterMode.Masked);
        await context.AddParameterFromObjectAsync("object-mode-token", 42, ParameterMode.Hidden, cancellation.Token);

        var operations = OperationCalls(calls);
        await Assert.That(operations.Count).IsEqualTo(10);
        await Assert.That(operations[0].Method.Name).IsEqualTo("SetNameAsync");
        await Assert.That(operations[1].Arguments[0]).IsSameReferenceAs(parameter);
        await Assert.That(Parameter(operations[6]).Value).IsEqualTo("context:42");
        await Assert.That(operations[9].Arguments[1]).IsEqualTo(cancellation.Token);
    }

    static T CreateSyncContext<T>(out IReadOnlyList<RecordedCall> calls) where T : class
    {
        var recording = RecordingInterface<T>.Create();
        var serializer = new TestParameterSerializer("context");
        recording.Handler = (method, _) =>
            method.Name == "get_ParameterSerializer" ? serializer : null;
        calls = recording.Calls;
        return recording.Instance;
    }

    static T CreateAsyncContext<T>(out IReadOnlyList<RecordedCall> calls) where T : class
    {
        var recording = RecordingInterface<T>.Create();
        var serializer = new TestParameterSerializer("context");
        recording.Handler = (method, _) =>
            method.Name == "get_ParameterSerializer" ? serializer : Task.CompletedTask;
        calls = recording.Calls;
        return recording.Instance;
    }

    static Parameter Parameter(RecordedCall call) => (Parameter)call.Arguments[0]!;

    static IReadOnlyList<RecordedCall> AddParameterCalls(IReadOnlyList<RecordedCall> calls) =>
        calls.Where(call => call.Method.Name == "AddParameter").ToArray();

    static IReadOnlyList<RecordedCall> OperationCalls(IReadOnlyList<RecordedCall> calls) =>
        calls.Where(call => !call.Method.IsSpecialName).ToArray();

    static async Task AssertParameters(
        IReadOnlyList<RecordedCall> calls,
        params string[] expectedNames
    )
    {
        await Assert.That(calls.Count).IsEqualTo(expectedNames.Length);
        for (var index = 0; index < expectedNames.Length; index++)
        {
            await Assert.That(calls[index].Method.Name).IsEqualTo("AddParameter");
            await Assert.That(Parameter(calls[index]).Name).IsEqualTo(expectedNames[index]);
        }
    }
}
