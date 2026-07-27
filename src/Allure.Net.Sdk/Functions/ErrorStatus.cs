using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Model;
using Allure.Sdk.Internal;

namespace Allure.Sdk.Functions;

public static class ErrorStatus
{
    /// <summary>
    /// Checks if an exception type, one of its base types, or one of the
    /// interfaces it implements exists in the list of known execption types.
    /// </summary>
    /// <param name="knownErrorBases">The list of known exception types.</param>
    /// <param name="e">The exception to check.</param>
    public static bool IsKnown(IEnumerable<string> knownErrorBases, Exception e) =>
        knownErrorBases
            ?.Intersect(
                GetExceptionTypeNamesClosure(e)
            )
            ?.Any() == true;

    /// <summary>
    /// Returns a <see cref="Status.Failed"/> if a given exception represents
    /// an assertion error. Otherwise, returns <see cref="Status.Broken"/>.
    /// </summary>
    /// <param name="failExceptions">
    ///   The list of exception types. Exceptions of those types (including
    ///   subclasses) are considered assertion errors. This list typically comes
    ///   from the configuration associated with the current lifecycle instance.
    /// </param>
    /// <param name="e">The exception to convert.</param>
    /// <returns></returns>
    public static Status Resolve(
        IEnumerable<string> failExceptions,
        Exception e
    ) =>
        IsKnown(failExceptions, e)
            ? Status.Failed
            : Status.Broken;

    static IEnumerable<string> GetExceptionTypeNamesClosure(Exception e) =>
        TypeFunctions.GetTypeClosure(e.GetType())
            .Select(static (t) => t.FullName);
}
