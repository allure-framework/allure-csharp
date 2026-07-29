using System;
using Allure.Model;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    public static Task StepAsync(string name) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], Status.Passed, null, default)
            : Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], Status.Passed, null, cancellationToken)
            : Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    public static Task StepAsync(string name, Status status) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], status, null, default)
            : Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Status status, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], status, null, cancellationToken)
            : Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    public static Task StepAsync(string name, Status status, StatusDetails statusDetails) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], status, statusDetails, default)
            : Task.CompletedTask;

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Status status, StatusDetails statusDetails, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], status, statusDetails, cancellationToken)
            : Task.CompletedTask;

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<Task> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (_, _) => body(), default)
            : body();

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (_, _) => body(), cancellationToken)
            : body();

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<IAllureAsyncStepContext, Task> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (ctx, _) => body(ctx), default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureAsyncStepContext, Task> body, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (ctx, _) => body(ctx), cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureAsyncStepContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (_, _) => body(), default)
            : body();

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(string name, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (_, _) => body(), cancellationToken)
            : body();

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
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (ctx, _) => body(ctx), default)
            : body(NullOperationContext.Instance);


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
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], (ctx, _) => body(ctx), cancellationToken)
            : body(NullOperationContext.Instance);

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
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Async: var api }
            ? api.StepAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);
}
