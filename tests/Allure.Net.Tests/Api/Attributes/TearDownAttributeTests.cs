using System.Reflection;
using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Attributes;

public class TearDownAttributeTests : ApiOperationTestsBase
{
    [Test]
    public async Task AttributeHasExpectedUsage()
    {
        var usage = typeof(AllureTearDownAttribute)
            .GetCustomAttribute<AttributeUsageAttribute>()!;

        await Assert.That(usage.ValidOn).IsEqualTo(AttributeTargets.Method);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsTrue();
    }

    [Test]
    public async Task DefaultConstructorSetsNameToNull()
    {
        await Assert.That(new AllureTearDownAttribute().Name).IsNull();
    }

    [Test]
    public async Task NamedConstructorPreservesName()
    {
        await Assert.That(new AllureTearDownAttribute("Fixture name").Name)
            .IsEqualTo("Fixture name");
    }

    [Test]
    public async Task VoidMethodIsExecutedAsTearDown()
    {
        CallCounter calls = new();
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(Any(), Any(), Any<Action>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body();
            }
        );

        CompleteTearDown(calls, 17);

        await Assert.That(calls.Value).IsEqualTo(1);
        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("value");
        await Assert.That(parameter.Value).IsEqualTo("serialized:17");
        await Assert.That(endpoint.SyncApi.TearDown(
            "Complete serialized:17",
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Action>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task UnnamedTearDownUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnnamedTearDown();

        await Assert.That(endpoint.SyncApi.TearDown(
            nameof(UnnamedTearDown),
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Func<int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task EmptyTearDownNameUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = EmptyNamedTearDown();

        await Assert.That(endpoint.SyncApi.TearDown(
            nameof(EmptyNamedTearDown),
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Func<int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task FunctionReturnsEndpointValue()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(Any(), Any(), Any<Func<int>>()).Returns(42);

        var result = ResultTearDown();

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.TearDown(
            "Result teardown",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ParameterAttributeControlsCreatedParameter()
    {
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(Any(), Any(), Any<Action>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body();
            }
        );

        ParameterizedTearDown(17, "hidden");

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
        endpoint.SyncApi.TearDown(Any(), Any(), Any<Action>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body();
            }
        );

        SharedSerializationTearDown(argument1, argument2, argument3);

        await Assert.That(argument1.InvocationCount).IsEqualTo(1);
        await Assert.That(argument2.InvocationCount).IsEqualTo(1);
        await Assert.That(argument3.InvocationCount).IsZero();
        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Value).IsEqualTo("serialized:included:1");
        await Assert.That(endpoint.SyncApi.TearDown(
            "serialized:included:1 serialized:included:1 serialized:usedOnlyInName:1",
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Action>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task NoArgumentsAreSerializedWithoutEndpoint()
    {
        ToStringCounter argument = new();
        using var _ = InstallNoEndpoint();

        NoEndpointTearDown(argument);

        await Assert.That(argument.InvocationCount).IsZero();
    }

    [Test]
    public async Task TaskMethodIsRoutedToAsyncApi()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(Any(), Any(), Any<Func<Task>>(), Any())
            .ReturnsAsync(Task.CompletedTask);

        await AsyncTearDown();

        await Assert.That(endpoint.AsyncApi.TearDownAsync(
            "Async teardown",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<Task>>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task TaskOfResultReturnsEndpointValue()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.TearDownAsync(Any(), Any(), Any<Func<Task<int>>>(), Any())
            .ReturnsAsync(Task.FromResult(42));

        var result = await AsyncResultTearDown();

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.TearDownAsync(
            "Async result teardown",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<Task<int>>>(),
            CancellationToken.None
        )).WasCalled(Times.Once);
        endpoint.SyncApi.VerifyNoOtherCalls();
    }

    [Test]
    public async Task SyncMethodExecutesAndReturnsValueWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var result = ResultTearDown();

        await Assert.That(result).IsEqualTo(17);
    }

    [Test]
    public async Task AsyncMethodExecutesAndReturnsValueWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var result = await AsyncResultTearDown();

        await Assert.That(result).IsEqualTo(18);
    }

    [Test]
    public async Task ExceptionFromMethodIsPropagated()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.TearDown(Any(), Any(), Any<Action>()).Callback(
            (_, _, body) => body()
        );

        await Assert.That(FailingTearDown).Throws<FixtureMethodException>();
    }

    [AllureTearDown("Complete {value}")]
    private static void CompleteTearDown(
        [AllureParameter(Ignore = true)] CallCounter calls,
        int value
    ) => calls.Value++;

    [AllureTearDown]
    private static int UnnamedTearDown() => 17;

    [AllureTearDown("")]
    private static int EmptyNamedTearDown() => 17;

    [AllureTearDown("Result teardown")]
    private static int ResultTearDown() => 17;

    [AllureTearDown("Parameters")]
    private static void ParameterizedTearDown(
        [AllureParameter(Name = "renamed", Mode = ParameterMode.Masked, Excluded = true)]
        int value,
        [AllureParameter(Ignore = true)] string ignored
    ) { }

    [AllureTearDown("{included} {included} {usedOnlyInName}")]
    private static void SharedSerializationTearDown(
        ToStringCounter included,
        [AllureParameter(Ignore = true)] ToStringCounter usedOnlyInName,
        [AllureParameter(Ignore = true)] ToStringCounter unused
    ) { }

    [AllureTearDown("{argument} {argument}")]
    private static void NoEndpointTearDown(ToStringCounter argument) { }

    [AllureTearDown("Async teardown")]
    private static async Task AsyncTearDown() => await Task.CompletedTask;

    [AllureTearDown("Async result teardown")]
    private static async Task<int> AsyncResultTearDown() => await Task.FromResult(18);

    [AllureTearDown]
    private static void FailingTearDown() => throw new FixtureMethodException();

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

    private sealed class FixtureMethodException : Exception;
}
