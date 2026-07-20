using System;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;

namespace Allure;

public static partial class AllureApi
{
    public static Task SetUp(string name, Func<Task> body) =>
        AllureFrontend.Runtime.TestApi.Async.SetUp(name, [], body, default);

    public static Task SetUp(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.SetUp(name, [], body, cancellationToken);

    public static Task SetUp(string name, Func<IAllureFixtureContextAsync, Task> body) =>
        AllureFrontend.Runtime.TestApi.Async.SetUp(name, [], body, default);

    public static Task SetUp(
        string name,
        Func<IAllureFixtureContextAsync, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.SetUp(name, [], body, cancellationToken);

    public static Task SetUp(
        string name,
        Func<IAllureFixtureContextAsync, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.SetUp(name, [], body, cancellationToken);

    public static Task TearDown(string name, Func<Task> body) =>
        AllureFrontend.Runtime.TestApi.Async.TearDown(name, [], body, default);

    public static Task TearDown(
        string name,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.TearDown(name, [], body, cancellationToken);

    public static Task TearDown(string name, Func<IAllureFixtureContextAsync, Task> body) =>
        AllureFrontend.Runtime.TestApi.Async.TearDown(name, [], body, default);

    public static Task TearDown(
        string name,
        Func<IAllureFixtureContextAsync, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.TearDown(name, [], body, cancellationToken);

    public static Task TearDown(
        string name,
        Func<IAllureFixtureContextAsync, CancellationToken, Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.TearDown(name, [], body, cancellationToken);
}
