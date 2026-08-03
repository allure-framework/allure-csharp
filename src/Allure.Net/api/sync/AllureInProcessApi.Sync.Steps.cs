using System;
using Allure.Abstractions;
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
    public static void Step(string name, Action<IAllureInProcessSyncStepContext> body)
    {
        if (ResolveOperations() is { Sync: var api })
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
    public static TResult Step<TResult>(
        string name,
        Func<IAllureInProcessSyncStepContext, TResult> body
    ) =>
        ResolveOperations() is { Sync: var api }
            ? api.Step(name, [], body)
            : body(NullOperationContext.Instance);
}
