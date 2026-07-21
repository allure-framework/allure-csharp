using System.Reflection;
using Allure.Aspects;
using Allure.Model;
using Allure.Net.Tests.Infrastructure;

namespace Allure.Net.Tests.Aspects;

public class OperationAspectTests
{
    [Test]
    public async Task StepRoutesSyncResultWithNameAndSerializedParameters()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(
            current: operations.Endpoint(new TestParameterSerializer("argument"))
        );
        var aspect = new AllureStepAspect();
        var metadata = Method(nameof(AttributedStep));

        var result = aspect.Around(
            nameof(AttributedStep),
            [42, "secret"],
            _ => "result",
            metadata,
            typeof(string)
        );

        var call = operations.Sync.SingleCall;
        var parameters = ((IEnumerable<Parameter>)call.Arguments[1]!).ToArray();
        await Assert.That(result).IsEqualTo("result");
        await Assert.That(call.Method.Name).IsEqualTo("Step");
        await Assert.That(call.Arguments[0]).IsEqualTo("operation argument:42");
        await Assert.That(parameters.Length).IsEqualTo(1);
        await Assert.That(parameters[0].Name).IsEqualTo("renamed");
        await Assert.That(parameters[0].Value).IsEqualTo("argument:42");
        await Assert.That(parameters[0].Mode).IsEqualTo(ParameterMode.Masked);
        await Assert.That(parameters[0].Excluded).IsTrue();
    }

    [Test]
    public async Task UnannotatedArgumentsBecomeDefaultParameters()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(
            current: operations.Endpoint(new TestParameterSerializer("argument"))
        );

        var result = new AllureStepAspect().Around(
            nameof(DefaultParameterStep),
            [42],
            _ => 7,
            Method(nameof(DefaultParameterStep)),
            typeof(int)
        );

        var parameter = ((IEnumerable<Parameter>)operations.Sync.SingleCall.Arguments[1]!).Single();
        await Assert.That(result).IsEqualTo(7);
        await Assert.That(parameter.Name).IsEqualTo("value");
        await Assert.That(parameter.Value).IsEqualTo("argument:42");
        await Assert.That(parameter.Mode).IsNull();
        await Assert.That(parameter.Excluded).IsFalse();
    }

    [Test]
    public async Task ArgumentsAreSerializedLazilyAndOnlyOnce()
    {
        var operations = new ExecutingOperations();
        var serializer = new CountingParameterSerializer();
        using var scope = FacadeTestEnvironment.Use(
            current: operations.Endpoint(serializer)
        );

        _ = new AllureStepAspect().Around(
            nameof(LazySerializationStep),
            ["included", "used", "unused"],
            _ => null!,
            Method(nameof(LazySerializationStep)),
            typeof(void)
        );

        await Assert.That(serializer.Values)
            .IsEquivalentTo(new object?[] { "included", "used" });
        await Assert.That(serializer.InvocationCount).IsEqualTo(2);
        await Assert.That(operations.Sync.SingleCall.Arguments[0])
            .IsEqualTo("included included used");
    }

    [Test]
    public async Task AsyncStepUsesAsyncOperationsAndPreservesResult()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());
        var expected = Task.FromResult(11);

        var result = new AllureStepAspect().Around(
            nameof(AsyncStep),
            [],
            _ => expected,
            Method(nameof(AsyncStep)),
            typeof(Task<int>)
        );

        await Assert.That(result).IsSameReferenceAs(expected);
        await Assert.That(operations.Async.SingleCall.Method.Name).IsEqualTo("StepAsync");
        await Assert.That(operations.Sync.Calls).IsEmpty();
    }

    [Test]
    public async Task MissingEndpointExecutesTargetWithoutRouting()
    {
        using var scope = FacadeTestEnvironment.Use();
        var calls = 0;

        var result = new AllureStepAspect().Around(
            nameof(DefaultParameterStep),
            [42],
            _ => { calls++; return 7; },
            Method(nameof(DefaultParameterStep)),
            typeof(int)
        );

        await Assert.That(result).IsEqualTo(7);
        await Assert.That(calls).IsEqualTo(1);
    }

    [Test]
    public async Task BeforeRoutesToSetUp()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());

        _ = new AllureSetUpAspect().Around(
            nameof(Before), [], _ => null!, Method(nameof(Before)), typeof(void)
        );

        await Assert.That(operations.Sync.SingleCall.Method.Name).IsEqualTo("SetUp");
    }

    [Test]
    public async Task AfterRoutesToTearDown()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());

        _ = new AllureTearDownAspect().Around(
            nameof(After), [], _ => null!, Method(nameof(After)), typeof(void)
        );

        await Assert.That(operations.Sync.SingleCall.Method.Name).IsEqualTo("TearDown");
    }

    [Test]
    public async Task FixtureAspectsRouteSyncAndAsyncResultShapes()
    {
        var operations = new ExecutingOperations();
        using var scope = FacadeTestEnvironment.Use(current: operations.Endpoint());
        var setUp = new AllureSetUpAspect();
        var tearDown = new AllureTearDownAspect();

        var setUpResult = setUp.Around(
            nameof(BeforeResult), [], _ => 17, Method(nameof(BeforeResult)), typeof(int)
        );
        var setUpTask = (Task)setUp.Around(
            nameof(BeforeAsync), [], _ => Task.CompletedTask, Method(nameof(BeforeAsync)), typeof(Task)
        )!;
        var setUpResultTask = (Task<int>)setUp.Around(
            nameof(BeforeAsyncResult), [], _ => Task.FromResult(18),
            Method(nameof(BeforeAsyncResult)), typeof(Task<int>)
        )!;
        var tearDownResult = tearDown.Around(
            nameof(AfterResult), [], _ => 19, Method(nameof(AfterResult)), typeof(int)
        );
        var tearDownTask = (Task)tearDown.Around(
            nameof(AfterAsync), [], _ => Task.CompletedTask, Method(nameof(AfterAsync)), typeof(Task)
        )!;
        var tearDownResultTask = (Task<int>)tearDown.Around(
            nameof(AfterAsyncResult), [], _ => Task.FromResult(20),
            Method(nameof(AfterAsyncResult)), typeof(Task<int>)
        )!;

        await setUpTask;
        await tearDownTask;
        await Assert.That(setUpResult).IsEqualTo(17);
        await Assert.That(await setUpResultTask).IsEqualTo(18);
        await Assert.That(tearDownResult).IsEqualTo(19);
        await Assert.That(await tearDownResultTask).IsEqualTo(20);
        await Assert.That(operations.Sync.Calls.Select(call => call.Method.Name))
            .IsEquivalentTo(["SetUp", "TearDown"]);
        await Assert.That(operations.Async.Calls.Select(call => call.Method.Name))
            .IsEquivalentTo(["SetUpAsync", "SetUpAsync", "TearDownAsync", "TearDownAsync"]);
    }

    static MethodInfo Method(string name) =>
        typeof(OperationAspectTests).GetMethod(name, BindingFlags.Static | BindingFlags.NonPublic)!;

    [AllureStep("operation {value}")]
    static string AttributedStep(
        [AllureParameter(Name = "renamed", Mode = ParameterMode.Masked, Excluded = true)] int value,
        [AllureParameter(Ignore = true)] string ignored
    ) => "result";

    [AllureStep]
    static int DefaultParameterStep(int value) => value;

    [AllureStep("{included} {included} {ignoredUsed}")]
    static void LazySerializationStep(
        string included,
        [AllureParameter(Ignore = true)] string ignoredUsed,
        [AllureParameter(Ignore = true)] string ignoredUnused
    ) { }

    [AllureStep]
    static Task<int> AsyncStep() => Task.FromResult(11);

    [AllureBefore]
    static void Before() { }

    [AllureAfter]
    static void After() { }

    [AllureBefore]
    static int BeforeResult() => 17;

    [AllureBefore]
    static Task BeforeAsync() => Task.CompletedTask;

    [AllureBefore]
    static Task<int> BeforeAsyncResult() => Task.FromResult(18);

    [AllureAfter]
    static int AfterResult() => 19;

    [AllureAfter]
    static Task AfterAsync() => Task.CompletedTask;

    [AllureAfter]
    static Task<int> AfterAsyncResult() => Task.FromResult(20);

    sealed class CountingParameterSerializer : Allure.Abstractions.IAllureParameterSerializer
    {
        readonly List<object?> values = [];

        public int InvocationCount => this.values.Count;

        public IReadOnlyList<object?> Values => this.values;

        public string Serialize(object? value)
        {
            this.values.Add(value);
            return value?.ToString() ?? "null";
        }
    }
}
