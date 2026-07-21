using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Internal;

class AllureTearDownRouter : AllureOperationRouter
{
    protected override void Run(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Action body
    ) =>
        endpoint.Operations.Sync.TearDown(name, parameters, body);

    protected override T Run<T>(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    ) =>
        endpoint.Operations.Sync.TearDown(name, parameters, body);

    protected override Task RunAsync(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        endpoint.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken);

    protected override Task<T> RunAsync<T>(
        IAllureApiEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    ) =>
        endpoint.Operations.Async.TearDownAsync(name, parameters, body, cancellationToken);

    public static AllureSetUpRouter Instance { get; } = new();
}
