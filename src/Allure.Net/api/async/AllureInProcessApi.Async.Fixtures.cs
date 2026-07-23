using System;
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
    public static Task SetUpAsync(string name, Func<IAllureInProcessAsyncFixtureContext, Task> body) =>
        ResolveOperations() is { Async: var api }
            ? api.SetUpAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(string name, Func<IAllureInProcessAsyncFixtureContext, Task<TResult>> body) =>
        ResolveOperations() is { Async: var api }
            ? api.SetUpAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(string name, Func<IAllureInProcessAsyncFixtureContext, Task> body) =>
        ResolveOperations() is { Async: var api }
            ? api.TearDownAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(string name, Func<IAllureInProcessAsyncFixtureContext, Task<TResult>> body) =>
        ResolveOperations() is { Async: var api }
            ? api.TearDownAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureInProcessAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);
}
