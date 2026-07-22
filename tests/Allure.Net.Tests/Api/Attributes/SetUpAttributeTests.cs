using System.Reflection;
using Allure.Abstractions;
using Allure.Model;
using TUnit.Mocks.Assertions;

namespace Allure.Net.Tests.Api.Attributes;

public class SetUpAttributeTests : AllureApiTestsBase
{
    [Test]
    public async Task AttributeHasExpectedUsage()
    {
        var usage = typeof(AllureSetUpAttribute).GetCustomAttribute<AttributeUsageAttribute>()!;

        await Assert.That(usage.ValidOn)
            .IsEqualTo(AttributeTargets.Method | AttributeTargets.Constructor);
        await Assert.That(usage.AllowMultiple).IsFalse();
        await Assert.That(usage.Inherited).IsTrue();
    }

    [Test]
    public async Task DefaultConstructorSetsNameToNull()
    {
        await Assert.That(new AllureSetUpAttribute().Name).IsNull();
    }

    [Test]
    public async Task NamedConstructorPreservesName()
    {
        await Assert.That(new AllureSetUpAttribute("Fixture name").Name)
            .IsEqualTo("Fixture name");
    }

    [Test]
    public async Task VoidMethodIsExecutedAsSetUp()
    {
        CallCounter calls = new();
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(Any(), Any(), Any<Action>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body();
            }
        );

        CompleteSetUp(calls, 17);

        await Assert.That(calls.Value).IsEqualTo(1);
        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Name).IsEqualTo("value");
        await Assert.That(parameter.Value).IsEqualTo("serialized:17");
        await Assert.That(endpoint.SyncApi.SetUp(
            "Complete serialized:17",
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Action>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task UnnamedSetUpUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = UnnamedSetUp();

        await Assert.That(endpoint.SyncApi.SetUp(
            nameof(UnnamedSetUp),
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Func<int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task EmptySetUpNameUsesMethodName()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);

        _ = EmptyNamedSetUp();

        await Assert.That(endpoint.SyncApi.SetUp(
            nameof(EmptyNamedSetUp),
            IsNotNull<IEnumerable<Parameter>>(),
            IsNotNull<Func<int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task FunctionReturnsEndpointValue()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(Any(), Any(), Any<Func<int>>()).Returns(42);

        var result = ResultSetUp();

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(endpoint.SyncApi.SetUp(
            "Result setup",
            IsEmpty<IEnumerable<Parameter>>(),
            IsNotNull<Func<int>>()
        )).WasCalled(Times.Once);
    }

    [Test]
    public async Task ParameterAttributeControlsCreatedParameter()
    {
        Parameter[] parameters = [];
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(Any(), Any(), Any<Action>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body();
            }
        );

        ParameterizedSetUp(17, "hidden");

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
        endpoint.SyncApi.SetUp(Any(), Any(), Any<Action>()).Callback(
            (_, values, body) =>
            {
                parameters = [.. values];
                body();
            }
        );

        SharedSerializationSetUp(argument1, argument2, argument3);

        await Assert.That(argument1.InvocationCount).IsEqualTo(1);
        await Assert.That(argument2.InvocationCount).IsEqualTo(1);
        await Assert.That(argument3.InvocationCount).IsZero();
        var parameter = await Assert.That(parameters).HasSingleItem();
        await Assert.That(parameter.Value).IsEqualTo("serialized:included:1");
        await Assert.That(endpoint.SyncApi.SetUp(
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

        NoEndpointSetUp(argument);

        await Assert.That(argument.InvocationCount).IsZero();
    }

    [Test]
    public async Task TaskMethodIsRoutedToAsyncApi()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.AsyncApi.SetUpAsync(Any(), Any(), Any<Func<Task>>(), Any())
            .ReturnsAsync(Task.CompletedTask);

        await AsyncSetUp();

        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Async setup",
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
        endpoint.AsyncApi.SetUpAsync(Any(), Any(), Any<Func<Task<int>>>(), Any())
            .ReturnsAsync(Task.FromResult(42));

        var result = await AsyncResultSetUp();

        await Assert.That(result).IsEqualTo(42);
        await Assert.That(endpoint.AsyncApi.SetUpAsync(
            "Async result setup",
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

        var result = ResultSetUp();

        await Assert.That(result).IsEqualTo(17);
    }

    [Test]
    public async Task AsyncMethodExecutesAndReturnsValueWithoutEndpoint()
    {
        using var _ = InstallNoEndpoint();

        var result = await AsyncResultSetUp();

        await Assert.That(result).IsEqualTo(18);
    }

    [Test]
    public async Task ExceptionFromMethodIsPropagated()
    {
        using var endpoint = InstallEndpoint(InstallationScope.Current);
        endpoint.SyncApi.SetUp(Any(), Any(), Any<Action>()).Callback(
            (_, _, body) => body()
        );

        await Assert.That(FailingSetUp).Throws<FixtureMethodException>();
    }

    [AllureSetUp("Complete {value}")]
    private static void CompleteSetUp(
        [AllureParameter(Ignore = true)] CallCounter calls,
        int value
    ) => calls.Value++;

    [AllureSetUp]
    private static int UnnamedSetUp() => 17;

    [AllureSetUp("")]
    private static int EmptyNamedSetUp() => 17;

    [AllureSetUp("Result setup")]
    private static int ResultSetUp() => 17;

    [AllureSetUp("Parameters")]
    private static void ParameterizedSetUp(
        [AllureParameter(Name = "renamed", Mode = ParameterMode.Masked, Excluded = true)]
        int value,
        [AllureParameter(Ignore = true)] string ignored
    ) { }

    [AllureSetUp("{included} {included} {usedOnlyInName}")]
    private static void SharedSerializationSetUp(
        ToStringCounter included,
        [AllureParameter(Ignore = true)] ToStringCounter usedOnlyInName,
        [AllureParameter(Ignore = true)] ToStringCounter unused
    ) { }

    [AllureSetUp("{argument} {argument}")]
    private static void NoEndpointSetUp(ToStringCounter argument) { }

    [AllureSetUp("Async setup")]
    private static async Task AsyncSetUp() => await Task.CompletedTask;

    [AllureSetUp("Async result setup")]
    private static async Task<int> AsyncResultSetUp() => await Task.FromResult(18);

    [AllureSetUp]
    private static void FailingSetUp() => throw new FixtureMethodException();

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
