using System.Linq.Expressions;
using System.Reflection;
using Allure.Model;

namespace Allure.Net.Tests.Infrastructure;

static class PublicMethodArguments
{
    public static MethodInfo Close(MethodInfo method) =>
        method.IsGenericMethodDefinition
            ? method.MakeGenericMethod(
                method.GetGenericArguments().Select(_ => typeof(string)).ToArray()
            )
            : method;

    public static object?[] Create(MethodInfo method) =>
        method.GetParameters().Select(Create).ToArray();

    static object? Create(ParameterInfo parameter)
    {
        var type = parameter.ParameterType;
        if (type.IsByRef)
        {
            type = type.GetElementType()!;
        }

        if (type == typeof(string)) return parameter.Name ?? "value";
        if (type == typeof(object)) return new object();
        if (type == typeof(bool)) return true;
        if (type == typeof(int)) return 42;
        if (type == typeof(CancellationToken)) return new CancellationTokenSource().Token;
        if (type == typeof(Stream)) return new MemoryStream([1, 2, 3]);
        if (type == typeof(ReadOnlyMemory<byte>)) return new ReadOnlyMemory<byte>([1, 2, 3]);
        if (type == typeof(Exception)) return new InvalidOperationException("failure");
        if (type == typeof(Label)) return Label.Create("label", "value");
        if (type == typeof(Link)) return new Link { Url = "https://example.test" };
        if (type == typeof(Parameter)) return new Parameter { Name = "parameter", Value = "value" };
        if (type == typeof(StatusDetails)) return new StatusDetails { Message = "details" };

        if (typeof(Delegate).IsAssignableFrom(type))
        {
            return CreateDelegate(type);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(IEnumerable<>))
        {
            return Array.CreateInstance(type.GetGenericArguments()[0], 0);
        }

        if (type.IsArray)
        {
            return Array.CreateInstance(type.GetElementType()!, 0);
        }

        return type.IsValueType ? Activator.CreateInstance(type) : null;
    }

    static Delegate CreateDelegate(Type delegateType)
    {
        var invoke = delegateType.GetMethod("Invoke")!;
        var parameters = invoke.GetParameters()
            .Select(parameter => Expression.Parameter(parameter.ParameterType, parameter.Name))
            .ToArray();
        Expression body = invoke.ReturnType == typeof(void)
            ? Expression.Empty()
            : Expression.Constant(DefaultReturn(invoke.ReturnType), invoke.ReturnType);
        return Expression.Lambda(delegateType, body, parameters).Compile();
    }

    static object? DefaultReturn(Type type)
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
