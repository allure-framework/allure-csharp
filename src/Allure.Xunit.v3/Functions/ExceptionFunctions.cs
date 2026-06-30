using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons;
using Xunit;
using Xunit.Sdk;

namespace Allure.Xunit.Functions;

static class ExceptionFunctions
{
    /// <summary>
    /// Returns an Allure test status for a given Xunit test result state.
    /// </summary>
    /// <param name="failExceptions">
    /// A list of exception types representing failed assertions. Each entry must be
    /// a full type name.
    /// </param>
    /// <param name="xunitTestState"></param>
    /// <returns>
    /// <see cref="Status.failed"/> if the state is marked by Xunit as representing a
    /// failed assertion, or at least one exception type is present in <paramref name="failExceptions"/>.
    /// Otherwise, returns <see cref="Status.broken"/>.
    /// </returns>
    public static Status ResolveStatus(
        IEnumerable<string> failExceptions,
        TestResultState xunitTestState
    ) =>
        xunitTestState.FailureCause == FailureCause.Assertion
            || xunitTestState.ExceptionTypes?.Any(
                (type) => failExceptions.Contains(type, StringComparer.OrdinalIgnoreCase)
            ) == true
                ? Status.failed
                : Status.broken;
}