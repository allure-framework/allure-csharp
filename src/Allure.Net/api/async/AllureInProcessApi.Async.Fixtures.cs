using System;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

public static partial class AllureInProcessApi
{
    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(string name, Func<IAllureAsyncInProcessFixtureContext, Task> body) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.SetUpAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(string name, Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.SetUpAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(string name, Func<IAllureAsyncInProcessFixtureContext, Task> body) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.TearDownAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(string name, Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.TearDownAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureAsyncInProcessFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.AsyncInProcessApi is IAllureAsyncInProcessOperations api
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);
}
