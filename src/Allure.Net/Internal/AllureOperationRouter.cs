using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
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
        if (!AllureFrontend.IsAvailableInCurrentScope)
        {
            return body(arguments);
        }

        var parameters = method.ConstructAllureParameters(arguments);
        var name = method.ConstructAllureName<AllureOperationAttribute>(parameters)
            ?? (method.IsConstructor && methodName is ".ctor" or ".cctor"
                ? $"{method.DeclaringType.Name}{methodName}"
                : methodName);

        if (returnType == typeof(void))
        {
            this.Run(name, parameters, () => { body(arguments); });
            return null;
        }

        if (returnType == typeof(Task))
        {
            return this.RunAsync(name, parameters, () => (Task)body(arguments)!, default);
        }

        var dispatcher = DispatcherCache.Get(returnType);
        return dispatcher.Invoke(this, name, parameters, body, arguments);
    }

    protected abstract void Run(string name, IEnumerable<Parameter> parameters, Action body);

    protected abstract T Run<T>(string name, IEnumerable<Parameter> parameters, Func<T> body);

    protected abstract Task RunAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    );

    protected abstract Task<T> RunAsync<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    );

    interface IDispatcher
    {
        object Invoke(
            AllureOperationRouter router,
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
            string name,
            IEnumerable<Parameter> parameters,
            Func<object?[], object?> body,
            object?[] arguments
        ) =>
            router.RunAsync(
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
            string name,
            IEnumerable<Parameter> parameters,
            Func<object?[], object?> body,
            object?[] arguments
        ) =>
            router.Run(
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
