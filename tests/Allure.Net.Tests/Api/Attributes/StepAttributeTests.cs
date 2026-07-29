using System.Reflection;
using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Attributes;

public class StepAttributeTests : AllureApiTestsBase
{
    [Test]
    public async Task AttributeHasExpectedUsage()
    {
        var usage = typeof(AllureStepAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;

        await Assert.That(usage.ValidOn).IsEqualTo(AttributeTargets.Method);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsTrue();
    }

    [Test]
    public async Task DefaultConstructorSetsNameToNull()
    {
        await Assert.That(new AllureStepAttribute().Name).IsNull();
    }

    [Test]
    public async Task NamedConstructorPreservesName()
    {
        await Assert.That(new AllureStepAttribute("Step name").Name)
            .IsEqualTo("Step name");
    }

    [Test]
    public async Task VoidMethodIsExecutedAsStep()
    {
        CallCounter calls = new();
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(Any(), Any(), Any<Action<IAllureSyncStepContext>>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body(null!);
            }
        );

        CompleteStep(calls, 17);

        await Assert.That(calls.Value).IsEqualTo(1);
        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("value");
        await Assert.That(parameter.Value).IsEqualTo("serialized:17");
        await Assert.That(endpoint.SyncApi.Step(
            "Complete serialized:17",
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Action<IAllureSyncStepContext>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task UnnamedStepUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnnamedStep();

        await Assert.That(endpoint.SyncApi.Step(
            nameof(UnnamedStep),
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Func<IAllureSyncStepContext, int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task EmptyStepNameUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = EmptyNamedStep();

        await Assert.That(endpoint.SyncApi.Step(
            nameof(EmptyNamedStep),
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Func<IAllureSyncStepContext, int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task FunctionReturnsEndpointValue()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(Any(), Any(), Any<Func<IAllureSyncStepContext, int>>()).Returns(42);

        var result = ResultStep();

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.Step(
            "Result step",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<IAllureSyncStepContext, int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ParameterAttributeControlsCreatedParameter()
    {
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(Any(), Any(), Any<Action<IAllureSyncStepContext>>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body(null!);
            }
        );

        ParameterizedStep(17, "hidden");

        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("renamed");
        await Assert.That(parameter.Value).IsEqualTo("serialized:17");
        await Assert.That(parameter.Mode).IsEqualTo(ParameterMode.Masked);
        await Assert.That(parameter.Excluded).IsTrue();
    }

    [Test]
    public async Task ArgumentSerializationIsSharedByNameAndParameters()
    {
        var argument1 = new ToStringCounter("included");
        var argument2 = new ToStringCounter("usedOnlyInName");
        var argument3 = new ToStringCounter("unused");
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(Any(), Any(), Any<Action<IAllureSyncStepContext>>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body(null!);
            }
        );

        SharedSerializationStep(argument1, argument2, argument3);

        await Assert.That(argument1.InvocationCount).IsEqualTo(1);
        await Assert.That(argument2.InvocationCount).IsEqualTo(1);
        await Assert.That(argument3.InvocationCount).IsZero();
        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Value).IsEqualTo("serialized:included:1");
        await Assert.That(endpoint.SyncApi.Step(
            "serialized:included:1 serialized:included:1 serialized:usedOnlyInName:1",
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Action<IAllureSyncStepContext>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task NoArgumentsAreSerializedWithoutEndpoint()
    {
        ToStringCounter argument = new();
        using var _ = InstallNoEndpoint();

        NoEndpointStep(argument);

        await Assert.That(argument.InvocationCount).IsZero();
    }

    [Test]
    public async Task TaskMethodIsRoutedToAsyncApi()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(),
            Any(),
            Any<Func<IAllureAsyncStepContext, CancellationToken, Task>>(),
            Any()
        ).ReturnsAsync(Task.CompletedTask);

        await AsyncStep();

        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Async step",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<IAllureAsyncStepContext, CancellationToken, Task>>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TaskOfResultReturnsEndpointValue()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.StepAsync(
            Any(),
            Any(),
            Any<Func<IAllureAsyncStepContext, CancellationToken, Task<int>>>(),
            Any()
        ).ReturnsAsync(Task.FromResult(42));

        var result = await AsyncResultStep();

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.StepAsync(
            "Async result step",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<IAllureAsyncStepContext, CancellationToken, Task<int>>>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SyncMethodExecutesAndReturnsValueWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var result = ResultStep();

        await Assert.That(result).IsEqualTo(17);
    }

    [Test]
    public async Task AsyncMethodExecutesAndReturnsValueWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var result = await AsyncResultStep();

        await Assert.That(result).IsEqualTo(18);
    }

    [Test]
    public async Task ExceptionFromMethodIsPropagated()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.Step(Any(), Any(), Any<Action<IAllureSyncStepContext>>()).Callback(
            (_, _, body) => body(null!)
        );

        await Assert.That(FailingStep).Throws<StepMethodException>();
    }

    [AllureStep("Complete {value}")]
    private static void CompleteStep(
        [AllureParameter(Ignore = true)] CallCounter calls,
        int value
    )
    {
        calls.Value++;
    }

    [AllureStep]
    private static int UnnamedStep() => 17;

    [AllureStep("")]
    private static int EmptyNamedStep() => 17;

    [AllureStep("Result step")]
    private static int ResultStep() => 17;

    [AllureStep("Parameters")]
    private static void ParameterizedStep(
        [AllureParameter(Name = "renamed", Mode = ParameterMode.Masked, Excluded = true)]
        int value,
        [AllureParameter(Ignore = true)] string ignored
    ) { }

    [AllureStep("{included} {included} {usedOnlyInName}")]
    private static void SharedSerializationStep(
        ToStringCounter included,
        [AllureParameter(Ignore = true)] ToStringCounter usedOnlyInName,
        [AllureParameter(Ignore = true)] ToStringCounter unused
    ) { }

    [AllureStep("{argument} {argument}")]
    private static void NoEndpointStep(ToStringCounter argument) { }

    [AllureStep("Async step")]
    private static async Task AsyncStep()
    {
        await Task.CompletedTask;
    }

    [AllureStep("Async result step")]
    private static async Task<int> AsyncResultStep()
    {
        return await Task.FromResult(18);
    }

    [AllureStep]
    private static void FailingStep() => throw new StepMethodException();

    private sealed class CallCounter
    {
        public int Value { get; set; }
    }

    private sealed class ToStringCounter(string prefix = "")
    {
        public int InvocationCount { get; private set; }

        public override string ToString()
        {
            this.InvocationCount++;
            return $"{prefix}:{this.InvocationCount}";
        }
    }

    private sealed class StepMethodException : Exception;
}
