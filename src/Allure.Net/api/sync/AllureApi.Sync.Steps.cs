using System;
using Allure.Model;
using Allure.Runtime;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    public static void Step(string name) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.Step(name, [], Status.Passed, null);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <remarks>If no test or fixture is running, does nothing.</remarks>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    public static void Step(string name, Status status) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.Step(name, [], status, null);

    /// <summary>
    /// Adds an empty step to the current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="status">A status of the step.</param>
    /// <param name="statusDetails">A status details of the step.</param>
    public static void Step(string name, Status status, StatusDetails statusDetails) =>
        AllureRuntimeRouter.ResolveCurrentScope()?.Operations.Sync.Step(name, [], status, statusDetails);

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static void Step(string name, Action body)
    {
        if (AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.Step(name, [], body);
        }
        else
        {
            body();
        }
    }

    /// <summary>
    /// Executes the action and reports the result as a new step of the current
    /// fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The code to run.</param>
    public static void Step(string name, Action<IAllureStepContext> body)
    {
        if (AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.Step(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static TResult Step<TResult>(string name, Func<TResult> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api}
            ? api.Step(name, [], body)
            : body();

    /// <summary>
    /// Executes the function and reports the result as a new step of the
    /// current fixture, test or step.
    /// </summary>
    /// <param name="name">The name of the step.</param>
    /// <param name="body">The function to run.</param>
    /// <returns>The original value returned by the function.</returns>
    public static TResult Step<TResult>(
        string name,
        Func<IAllureStepContext, TResult> body
    ) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api}
            ? api.Step(name, [], body)
            : body(NullOperationContext.Instance);
}
