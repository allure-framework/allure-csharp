using System;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    public static Task SetUpAsync(string name, Func<Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, default)
            ?? body();

    public static Task SetUpAsync(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, cancellationToken)
            ?? body();

    public static Task SetUpAsync(string name, Func<IAllureAsyncFixtureContext, Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, default)
            ?? body(NullOperationContext.Instance);

    public static Task SetUpAsync(
        string name,
        Func<IAllureAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public static Task SetUpAsync(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public static Task<TResult> SetUpAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, default)
            ?? body();

    public static Task<TResult> SetUpAsync<TResult>(string name, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, cancellationToken)
            ?? body();

    public static Task<TResult> SetUpAsync<TResult>(string name, Func<IAllureAsyncFixtureContext, Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, default)
            ?? body(NullOperationContext.Instance);

    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public static Task<TResult> SetUpAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.SetUpAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public static Task TearDownAsync(string name, Func<Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, default)
            ?? body();

    public static Task TearDownAsync(
        string name,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, cancellationToken)
            ?? body();

    public static Task TearDownAsync(string name, Func<IAllureAsyncFixtureContext, Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, default)
            ?? body(NullOperationContext.Instance);

    public static Task TearDownAsync(
        string name,
        Func<IAllureAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public static Task TearDownAsync(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    public static Task<TResult> TearDownAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, default)
            ?? body();

    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, cancellationToken)
            ?? body();

    public static Task<TResult> TearDownAsync<TResult>(string name, Func<IAllureAsyncFixtureContext, Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, default)
            ?? body(NullOperationContext.Instance);

    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    public static Task<TResult> TearDownAsync<TResult>(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.TearDownAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);
}
