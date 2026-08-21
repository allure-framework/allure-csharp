using System;
using System.Collections.Generic;
using System.Linq;
using Xunit.Sdk;

namespace Allure.Xunit.Internal.Functions;

static class XunitTestFailures
{
    public static bool IsAssertionFailure(
        IEnumerable<string> failExceptions,
        ITestFailed testFailed
    ) =>
        testFailed.ExceptionTypes.Any(
            (exceptionType) => failExceptions.Contains(
                exceptionType,
                StringComparer.OrdinalIgnoreCase
            )
        );
}
