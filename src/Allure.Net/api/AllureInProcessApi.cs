using System;
using Allure.Abstractions;
using Allure.Runtime;

namespace Allure;

/// <summary>
/// Provides direct access to Allure model objects when the API endpoint runs in the current process.
/// </summary>
/// <remarks>
/// These operations are unavailable for out-of-process Allure integrations.
/// </remarks>
public static partial class AllureInProcessApi
{
    static IAllureInProcessOperations? ResolveOperations()
    {
        var endpoint = AllureRuntimeRouter.ResolveCurrentScope();

        if (endpoint is null)
        {
            return null;
        }

        return endpoint is IAllureInProcessRuntimeEndpoint inProcessEndpoint
            ? inProcessEndpoint.InProcessOperations
            : throw new InvalidOperationException(
                $"The current Allure runtime endpoint '{endpoint?.Name ?? "<none>"}' "
                    + "does not support in-process model access."
            );
    }
}
