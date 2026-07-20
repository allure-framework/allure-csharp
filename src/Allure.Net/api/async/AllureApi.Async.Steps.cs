using System;
using Allure.Model;
using Allure.Runtime;
using Allure.Abstractions;
using System.Threading.Tasks;
using System.Threading;

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
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], Status.Passed, null, default);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], Status.Passed, null, cancellationToken);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    public static Task StepAsync(string name, Status status) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], status, null, default);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Status status, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], status, null, cancellationToken);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    public static Task StepAsync(string name, Status status, StatusDetails statusDetails) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], status, statusDetails, default);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Status status, StatusDetails statusDetails, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], status, statusDetails, cancellationToken);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<Task> body) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, default);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, cancellationToken);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static Task StepAsync(string name, Func<IAllureStepContextAsync, Task> body) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, default);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureStepContextAsync, Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, cancellationToken);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    public static Task StepAsync(string name, Func<IAllureStepContextAsync, CancellationToken, Task> body, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, cancellationToken);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(string name, Func<Task<TResult>> body) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, default);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(string name, Func<Task<TResult>> body, CancellationToken cancellationToken) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, cancellationToken);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static Task<TResult> StepAsync<TResult>(
        string name,
        Func<IAllureStepContextAsync, Task<TResult>> body
    ) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, default);


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
        Func<IAllureStepContextAsync, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, cancellationToken);

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
        Func<IAllureStepContextAsync, CancellationToken, Task<TResult>> body,
        CancellationToken cancellationToken
    ) =>
        AllureFrontend.Runtime.TestApi.Async.Step(name, [], body, cancellationToken);
}
