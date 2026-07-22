using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.Internal;

class AllureStepRouter : AllureOperationRouter
{
    protected override void Run(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Action body
    ) =>
        endpoint.Operations.Sync.Step(name, parameters, body);

    protected override T Run<T>(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<T> body
    ) =>
        endpoint.Operations.Sync.Step(name, parameters, body);

    protected override Task RunAsync(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task> body,
        CancellationToken cancellationToken
    ) =>
        endpoint.Operations.Async.StepAsync(name, parameters, body, cancellationToken);

    protected override Task<T> RunAsync<T>(
        IAllureRuntimeEndpoint endpoint,
        string name,
        IEnumerable<Parameter> parameters,
        Func<Task<T>> body,
        CancellationToken cancellationToken
    ) =>
        endpoint.Operations.Async.StepAsync(name, parameters, body, cancellationToken);

    public static AllureStepRouter Instance { get; } = new();
}
