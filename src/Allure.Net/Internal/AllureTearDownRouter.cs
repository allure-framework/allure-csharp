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
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Action body
    ) =>
        endpoint.Operations.Sync.TearDown(name, parameters, (_) => body());

    protected override T Run<T>(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    ) =>
        endpoint.Operations.Sync.TearDown(name, parameters, (_) => body());

    protected override Task RunAsync(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        endpoint.Operations.Async.TearDownAsync(name, parameters, (_, _) => body(), cancellationToken);

    protected override Task<T> RunAsync<T>(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    ) =>
        endpoint.Operations.Async.TearDownAsync(name, parameters, (_, _) => body(), cancellationToken);

    public static AllureTearDownRouter Instance { get; } = new();
}
