using System;
using Allure.Model;
using Allure.Runtime;
using Allure.Abstractions;

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
    public static void Step(string name) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], Status.Passed, null);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    public static void Step(string name, Status status) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], status, null);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    public static void Step(string name, Status status, StatusDetails statusDetails) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], status, statusDetails);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static void Step(string name, Action body) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], body);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static void Step(string name, Action<IAllureStepContext> body) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], body);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static TResult StepAsync<TResult>(string name, Func<TResult> body) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], body);

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static TResult StepAsync<TResult>(
        string name,
        Func<IAllureStepContext, TResult> body
    ) =>
        AllureFrontend.Client.TestApi.Sync.Step(name, [], body);
}
