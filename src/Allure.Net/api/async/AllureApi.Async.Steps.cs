using System;
using Allure.Model;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

/// <summary>
/// A facade that provides the API for test authors to enhance the Allure
/// report.
/// </summary>
public static partial class AllureApi
{
    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    public static Task StepAsync(string name) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], Status.Passed, null, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], Status.Passed, null, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    public static Task StepAsync(string name, Status status) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], status, null, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Status status, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], status, null, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    public static Task StepAsync(string name, Status status, StatusDetails statusDetails) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], status, statusDetails, default)
            ?? Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Status status, StatusDetails statusDetails, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], status, statusDetails, cancellationToken)
            ?? Task.CompletedTask;

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, default)
            ?? body();

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, cancellationToken)
            ?? body();

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<IAllureAsyncStepContext, Task> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, default)
            ?? body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureAsyncStepContext, Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureAsyncStepContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, default)
            ?? body();

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(string name, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, cancellationToken)
            ?? body();

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(
        string name,
        Func<IAllureAsyncStepContext, Task<TResult>> body
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, default)
            ?? body(NullOperationContext.Instance);


    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(
        string name,
        Func<IAllureAsyncStepContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(
        string name,
        Func<IAllureAsyncStepContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Client.ResolveCurrentScope()?.Operations.Async.StepAsync(name, [], body, cancellationToken)
            ?? body(NullOperationContext.Instance, cancellationToken);
}
