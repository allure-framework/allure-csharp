using System.Reflection;

namespace Allure.Net.Sdk.Tests.Infrastructure;

static class InterfaceStub
{
    public static T Create<T>() where T : class =>
        DispatchProxy.Create<T, DefaultDispatchProxy>();

    class DefaultDispatchProxy : DispatchProxy
    {
        protected override object? Invoke(MethodInfo? targetMethod, object?[]? args)
        {
            var returnType = targetMethod?.ReturnType ?? typeof(void);

            if (returnType == typeof(void))
            {
                return null;
            }

            if (returnType == typeof(Task))
            {
                return Task.CompletedTask;
            }

            if (returnType.IsGenericType
                && returnType.GetGenericTypeDefinition() == typeof(Task<>))
            {
                var resultType = returnType.GetGenericArguments()[0];
                return typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(resultType)
                    .Invoke(null, [DefaultValue(resultType)]);
            }

            return DefaultValue(returnType);
        }

        static object? DefaultValue(Type type) =>
            type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
