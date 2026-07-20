using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using Allure.Runtime;

namespace Allure.Internal;

class AllureSetUpRouter : AllureOperationRouter
{
    protected override void Run(
        string name,
        IEnumerable<Parameter> parameters,
        Action body
    ) =>
        AllureFrontend.Client.TestApi.Sync.SetUp(name, parameters, body);

    protected override T Run<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    ) =>
        AllureFrontend.Client.TestApi.Sync.SetUp(name, parameters, body);

    protected override Task RunAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken);

    protected override Task<T> RunAsync<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.TestApi.Async.SetUpAsync(name, parameters, body, cancellationToken);

    public static AllureSetUpRouter Instance { get; } = new();
}
