using Allure.Abstractions;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Api;

public class ParameterSerializationTests
{
    [Test]
    public async Task SyncObjectParameterUsesSerializerAndOperationsFromSameEndpoint()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        var value = new object();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(
            sync: operations.Instance,
            serializer: new TestParameterSerializer("endpoint")
        ));

        AllureApi.AddTestParameterFromObject("argument", value, ParameterMode.Masked, true);

        var parameter = (Parameter)operations.SingleCall.Arguments[0]!;
        await Assert.That(parameter.Name).IsEqualTo("argument");
        await Assert.That(parameter.Value).IsEqualTo($"endpoint:{value}");
        await Assert.That(parameter.Mode).IsEqualTo(ParameterMode.Masked);
        await Assert.That(parameter.Excluded).IsTrue();
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task AsyncObjectParameterUsesSerializerAndOperationsFromSameEndpoint()
    {
        var operations = RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(
            @async: operations.Instance,
            serializer: new TestParameterSerializer("async")
        ));
        using var cancellation = new CancellationTokenSource();

        await AllureApi.AddTestParameterFromObjectAsync(
            "argument",
            42,
            ParameterMode.Hidden,
            true,
            cancellation.Token
        );

        var parameter = (Parameter)operations.SingleCall.Arguments[0]!;
        await Assert.That(parameter.Value).IsEqualTo("async:42");
        await Assert.That(parameter.Mode).IsEqualTo(ParameterMode.Hidden);
        await Assert.That(parameter.Excluded).IsTrue();
        await Assert.That(operations.SingleCall.Arguments[1]).IsEqualTo(cancellation.Token);
        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(1);
    }

    [Test]
    public async Task StringParameterBypassesSerializer()
    {
        var operations = RecordingInterface<IAllureOperations<IAllureStepContext, IAllureFixtureContext>>.Create();
        using var scope = FacadeTestEnvironment.Use(current: new TestApiEndpoint(
            sync: operations.Instance,
            serializer: new ThrowingSerializer()
        ));

        AllureApi.AddTestParameter("argument", "already serialized");

        var parameter = (Parameter)operations.SingleCall.Arguments[0]!;
        await Assert.That(parameter.Value).IsEqualTo("already serialized");
    }

    [Test]
    public async Task MissingEndpointDoesNotSerializeObject()
    {
        using var scope = FacadeTestEnvironment.Use();

        AllureApi.AddTestParameterFromObject("argument", new object());
        await AllureApi.AddTestParameterFromObjectAsync("argument", new object());

        await Assert.That(scope.CurrentResolutionCount).IsEqualTo(2);
    }

    sealed class ThrowingSerializer : IAllureParameterSerializer
    {
        public string Serialize(object? value) =>
            throw new InvalidOperationException("The serializer must not be used.");
    }
}
