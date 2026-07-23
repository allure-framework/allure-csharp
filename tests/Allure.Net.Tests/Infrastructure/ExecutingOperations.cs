using System.Reflection;
using Allure.Abstractions;

namespace Allure.Net.Tests.Infrastructure;

sealed class ExecutingOperations
{
    public RecordingInterface<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>> Sync { get; } =
        RecordingInterface<IAllureSyncOperations<IAllureSyncStepContext, IAllureSyncFixtureContext>>.Create();

    public RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>> Async { get; } =
        RecordingInterface<IAllureAsyncOperations<IAllureAsyncStepContext, IAllureAsyncFixtureContext>>.Create();

    public ExecutingOperations()
    {
        this.Sync.Handler = ExecuteBody;
        this.Async.Handler = ExecuteBody;
    }

    public TestApiEndpoint Endpoint(IAllureParameterSerializer? serializer = null) =>
        new(this.Sync.Instance, this.Async.Instance, serializer);

    static object? ExecuteBody(MethodInfo method, object?[] arguments)
    {
        if (method.Name is not (
            "Step" or "SetUp" or "TearDown"
            or "StepAsync" or "SetUpAsync" or "TearDownAsync"
        ))
        {
            return DefaultValue(method.ReturnType);
        }

        var body = (Delegate)arguments[2]!;
        var bodyArguments = body.Method.GetParameters()
            .Select(parameter => parameter.ParameterType == typeof(CancellationToken)
                ? arguments[^1]
                : parameter.ParameterType.IsInterface
                    ? InterfaceStub.Create(parameter.ParameterType)
                    : parameter.ParameterType.IsValueType
                        ? Activator.CreateInstance(parameter.ParameterType)
                        : null)
            .ToArray();
        return body.DynamicInvoke(bodyArguments);
    }

    static object? DefaultValue(Type type)
    {
        if (type == typeof(void)) return null;
        if (type == typeof(Task)) return Task.CompletedTask;
        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = type.GetGenericArguments()[0];
            return typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, [resultType.IsValueType ? Activator.CreateInstance(resultType) : null]);
        }
        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
