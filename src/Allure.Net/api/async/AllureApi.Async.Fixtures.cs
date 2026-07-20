using System;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;

namespace Allure;

public static partial class AllureApi
{
    public static Task SetUp(string name, Func<Task> body) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, [], body, default);

    public static Task SetUp(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, [], body, cancellationToken);

    public static Task SetUp(string name, Func<IAllureAsyncFixtureContext, Task> body) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, [], body, default);

    public static Task SetUp(
        string name,
        Func<IAllureAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, [], body, cancellationToken);

    public static Task SetUp(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, [], body, cancellationToken);

    public static Task TearDown(string name, Func<Task> body) =>
        AllureFrontend.Client.TestApi.Async.TearDownAsync(name, [], body, default);

    public static Task TearDown(
        string name,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.TearDownAsync(name, [], body, cancellationToken);

    public static Task TearDown(string name, Func<IAllureAsyncFixtureContext, Task> body) =>
        AllureFrontend.Client.TestApi.Async.TearDownAsync(name, [], body, default);

    public static Task TearDown(
        string name,
        Func<IAllureAsyncFixtureContext, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.TearDownAsync(name, [], body, cancellationToken);

    public static Task TearDown(
        string name,
        Func<IAllureAsyncFixtureContext, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.TearDownAsync(name, [], body, cancellationToken);
}
