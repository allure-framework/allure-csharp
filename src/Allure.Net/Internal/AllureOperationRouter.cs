using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;
using Allure.Runtime;

namespace Allure.Internal;

abstract class AllureOperationRouter
{
    public object? Route(
        string methodName,
        MethodBase method,
        Type returnType,
        Func<object?[], object?> body,
        object?[] arguments
    )
    {
        var endpoint = AllureFrontend.Client.ResolveCurrentScope();
        if (endpoint is null)
        {
            return body(arguments);
        }

        var preparedMthodParameters = method.PrepareParametersForSerialization(endpoint, arguments);
        var parameters = ConstructAllureParameters(preparedMthodParameters);
        var name = method.ConstructAllureName<AllureOperationAttribute>(preparedMthodParameters)
            ?? (method.IsConstructor && methodName is ".ctor" or ".cctor"
                ? $"{method.DeclaringType.Name}{methodName}"
                : methodName);

        if (returnType == typeof(void))
        {
            this.Run(endpoint, name, parameters, () => { body(arguments); });
            return null;
        }

        if (returnType == typeof(Task))
        {
            return this.RunAsync(endpoint, name, parameters, () => (Task)body(arguments)!, default);
        }

        var dispatcher = DispatcherCache.Get(returnType);
        return dispatcher.Invoke(this, endpoint, name, parameters, body, arguments);
    }

    List<Parameter> ConstructAllureParameters(
        IEnumerable<(ParameterInfo parameter, Lazy<string> argument)> preparedMethodParameters
    ) => [
        .. preparedMethodParameters
            .Select(static (t) => (
                t.parameter,
                t.argument,
                data: t.parameter.GetCustomAttribute<AllureParameterAttribute>()))
            .Where(static (t) => t.data?.Ignore != true)
            .Select((t) => new Parameter
            {
                Name = t.data?.Name ?? t.parameter.Name,
                Value = t.argument.Value,
                Mode = t.data?.Mode,
                Excluded = t.data?.Excluded ?? false,
            }),
    ];

    protected abstract void Run(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Action body
    );

    protected abstract T Run<T>(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    );

    protected abstract Task RunAsync(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    );

    protected abstract Task<T> RunAsync<T>(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    );

    interface IDispatcher
    {
        object Invoke(
            AllureOperationRouter router,
            IAllureApiEndpoint endpoint,
            string name,
            IEnumerable<Parameter> parameters,
            Func<object?[], object?> body,
            object?[] arguments
        );
    }

    sealed class AsyncDispatcher<TResult> : IDispatcher
    {
        public object Invoke(
            AllureOperationRouter router,
            IAllureApiEndpoint endpoint,
            string name,
            IEnumerable<Parameter> parameters,
            Func<object?[], object?> body,
            object?[] arguments
        ) =>
            router.RunAsync(
                endpoint,
                name,
                parameters,
                () => (Task<TResult>)body(arguments)!,
                default
            );
    }

    sealed class SyncDispatcher<TResult> : IDispatcher
    {
        public object Invoke(
            AllureOperationRouter router,
            IAllureApiEndpoint endpoint,
            string name,
            IEnumerable<Parameter> parameters,
            Func<object?[], object?> body,
            object?[] arguments
        ) =>
            router.Run(
                endpoint,
                name,
                parameters,
                () => (TResult)body(arguments)!
            )!;
    }

    static class DispatcherCache
    {
        static readonly ConcurrentDictionary<Type, IDispatcher> cache = [];

        public static IDispatcher Get(Type returnType) =>
            cache.GetOrAdd(returnType, Create);

        static IDispatcher Create(Type returnType)
        {
            if (IsGenericTask(returnType))
            {
                var resultType = returnType.GetGenericArguments()[0];
                var asyncDispatcherType =
                    typeof(AsyncDispatcher<>).MakeGenericType(resultType);
                return (IDispatcher)Activator.CreateInstance(asyncDispatcherType);
            }

            var syncDispatcherType =
                typeof(SyncDispatcher<>).MakeGenericType(returnType);
            return (IDispatcher)Activator.CreateInstance(syncDispatcherType);
        }

        static bool IsGenericTask(Type type) =>
            type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Task<>);
    }
}
