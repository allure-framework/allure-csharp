using System;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;
using Allure.Internal;

namespace Allure;

public static partial class AllureInProcessApi
{
    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<IAllureInProcessAsyncStepContext, Task> body) =>
        ResolveOperations() is { Async: var api }
            ? api.StepAsync(name, [], body, default)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureInProcessAsyncStepContext, Task> body, CancellationToken cancellationToken) =>
        ResolveOperations() is { Async: var api }
            ? api.StepAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureInProcessAsyncStepContext, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        ResolveOperations() is { Async: var api }
            ? api.StepAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(
        string name,
        Func<IAllureInProcessAsyncStepContext, Task<TResult>> body
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.StepAsync(name, [], body, default)
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
        Func<IAllureInProcessAsyncStepContext, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.StepAsync(name, [], body, cancellationToken)
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
        Func<IAllureInProcessAsyncStepContext, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        ResolveOperations() is { Async: var api }
            ? api.StepAsync(name, [], body, cancellationToken)
            : body(NullOperationContext.Instance, cancellationToken);
}
