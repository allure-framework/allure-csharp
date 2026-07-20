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
        AllureFrontend.Client.Operations.Sync.Step(name, parameters, body);

    protected override T Run<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    ) =>
        AllureFrontend.Client.Operations.Sync.Step(name, parameters, body);

    protected override Task RunAsync(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.Operations.Async.StepAsync(name, parameters, body, cancellationToken);

    protected override Task<T> RunAsync<T>(
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.Operations.Async.StepAsync(name, parameters, body, cancellationToken);

    public static AllureStepRouter Instance { get; } = new();
}
