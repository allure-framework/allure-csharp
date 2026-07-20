using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using Allure.Runtime;

namespace Allure.Internal;

class AllureStepRouter : AllureOperationRouter
{
    protected override void Run(
        string name,
        IEnumerable<Parameter> parameters,
        Action body
    ) =>
        AllureFrontend.Runtime.TestApi.Sync.Step(name, parameters, body);

    protected override T Run<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    ) =>
        AllureFrontend.Runtime.TestApi.Sync.Step(name, parameters, body);

    protected override Task RunAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, parameters, body, cancellationToken);

    protected override Task<T> RunAsync<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, parameters, body, cancellationToken);

    public static AllureStepRouter Instance { get; } = new();
}
