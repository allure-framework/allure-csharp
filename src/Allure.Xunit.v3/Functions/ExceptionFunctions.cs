using System;
using System.Linq;
using Allure.Xunit.Internal;
using Xunit.Sdk;

namespace Allure.Xunit.Functions;

static class ExceptionFunctions
{
    public static bool IsConfiguredAssertionFailure(ITestFailed testFailed)
    {
        var failExceptions =
            AllureTestingPlatformServices.AllureRuntime.Configuration.FailExceptions;
        return testFailed.ExceptionTypes.Any(
            (e) => failExceptions.Contains(e, StringComparer.OrdinalIgnoreCase)
        );
    }
}
