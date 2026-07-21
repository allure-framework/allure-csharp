using System.Reflection;

namespace Allure.Net.Tests.Infrastructure;

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
                var fromResult = typeof(Task)
                    .GetMethod(nameof(Task.FromResult))!
                    .MakeGenericMethod(resultType);

                return fromResult.Invoke(null, [GetDefault(resultType)]);
            }

            return GetDefault(returnType);
        }

        static object? GetDefault(Type type) =>
            type.IsValueType ? Activator.CreateInstance(type) : null;
    }
}
