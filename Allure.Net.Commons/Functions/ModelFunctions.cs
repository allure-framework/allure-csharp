using System;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Allure.Net.Commons.Functions;

/// <summary>
/// Contains functions to help implementing Allure model-related conversions.
/// </summary>
public static class ModelFunctions
{
    /// <summary>
    /// Checks if an exception type, one of its base types, or one of the
    /// interfaces it implements exists in the list of known execption types.
    /// </summary>
    /// <param name="knownErrorBases">The list of known exception types.</param>
    /// <param name="e">The exception to check.</param>
    public static bool IsKnownError(IEnumerable<string> knownErrorBases, Exception e) =>
        knownErrorBases
            .Intersect(
                GetExceptionClassChain(e)
            )
            .Any();

    /// <summary>
    /// Returns a <see cref="Status.failed"/> if a given exception represents
    /// an assertion error. Otherwise, returns <see cref="Status.broken"/>.
    /// </summary>
    /// <param name="failExceptions">
    ///   The list of exception types. Exceptions of those types (including
    ///   subclasses) are considered assertion errors. This list typically comes
    ///   from the configuration associated with the current lifecycle instance.
    /// </param>
    /// <param name="e">The exception to convert.</param>
    /// <returns></returns>
    public static Status ResolveErrorStatus(
        IEnumerable<string> failExceptions,
        Exception e
    ) =>
        IsKnownError(failExceptions, e)
            ? Status.failed
            : Status.broken;

    /// <summary>
    /// Converts an exception to the status details.
    /// </summary>
    /// <param name="e">The exception to convert.</param>
    public static StatusDetails? ToStatusDetails(Exception? e) =>
        e is null
            ? null
            : new()
            {
                message = string.IsNullOrEmpty(e.Message)
                    ? e.GetType().Name
                    : e.Message,
                trace = e.ToString()
            };

    static IEnumerable<string> GetExceptionClassChain(Exception e)
    {
        for (var type = e.GetType(); type != null; type = type.BaseType)
        {
            yield return type.FullName;
        }
        foreach (var intetrface in e.GetType().GetInterfaces())
        {
            yield return intetrface.FullName;
        }
    }
}
