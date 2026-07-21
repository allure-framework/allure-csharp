using System.Reflection;

namespace Allure.Net.Tests.Infrastructure;

sealed record RecordedCall(MethodInfo Method, object?[] Arguments);

sealed class RecordingInterface<T> where T : class
{
    readonly RecordingDispatchProxy proxy;

    RecordingInterface(T instance, RecordingDispatchProxy proxy)
    {
        this.Instance = instance;
        this.proxy = proxy;
    }

    public T Instance { get; }

    public IReadOnlyList<RecordedCall> Calls => this.proxy.Calls;

    public Func<MethodInfo, object?[], object?>? Handler
    {
        get => this.proxy.Handler;
        set => this.proxy.Handler = value;
    }

    public RecordedCall SingleCall => this.Calls.Single();

    public static RecordingInterface<T> Create()
    {
        var instance = DispatchProxy.Create<T, RecordingDispatchProxy>();
        return new(instance, (RecordingDispatchProxy)(object)instance);
    }
}

class RecordingDispatchProxy : DispatchProxy
{
    readonly List<RecordedCall> calls = [];

    public IReadOnlyList<RecordedCall> Calls => this.calls;

    public Func<MethodInfo, object?[], object?>? Handler { get; set; }

    protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
    {
        var method = targetMethod ?? throw new InvalidOperationException("Missing target method.");
        var arguments = args ?? [];
        this.calls.Add(new(method, arguments));

        if (this.Handler is not null)
        {
            return this.Handler(method, arguments);
        }

        return DefaultValue(method.ReturnType);
    }

    static object? DefaultValue(Type type)
    {
        if (type == typeof(void))
        {
            return null;
        }

        if (type == typeof(Task))
        {
            return Task.CompletedTask;
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>))
        {
            var resultType = type.GetGenericArguments()[0];
            return typeof(Task)
                .GetMethod(nameof(Task.FromResult))!
                .MakeGenericMethod(resultType)
                .Invoke(null, [DefaultValue(resultType)]);
        }

        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
