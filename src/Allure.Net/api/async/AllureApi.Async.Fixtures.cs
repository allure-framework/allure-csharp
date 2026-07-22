using System;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Runs an asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(string name, Func<Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, default)
            : body();

    /// <summary>
    /// Runs an asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body();

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(string name, Func<IAllureAsyncFixtureContext, Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(
        string name,
        Func<IAllureAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a setup fixture.
    /// </summary>
    public static Task SetUpAsync(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs an asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, default)
            : body();

    /// <summary>
    /// Runs an asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(string name, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body();

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(string name, Func<IAllureAsyncFixtureContext, Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a setup fixture and returns its result.
    /// </summary>
    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.SetUpAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs an asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(string name, Func<Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, default)
            : body();

    /// <summary>
    /// Runs an asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body();

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(string name, Func<IAllureAsyncFixtureContext, Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<IAllureAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a teardown fixture.
    /// </summary>
    public static Task TearDownAsync(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Runs an asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, default)
            : body();

    /// <summary>
    /// Runs an asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body();

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(string name, Func<IAllureAsyncFixtureContext, Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a cancellable, context-aware asynchronous function as a teardown fixture and returns its result.
    /// </summary>
    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.TearDownAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);
}
