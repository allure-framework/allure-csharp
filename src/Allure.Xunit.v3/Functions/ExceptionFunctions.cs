using System;
using System.Linq;
using Allure.Xunit.Internal;
using Xunit.Sdk;

namespace Allure.Xunit.Functions;

static class ExceptionFunctions
{
    public static bool IsConfiguredAssertionFailure(ITestFailed testFailed) =>
        AllureTestingPlatformServices.AllureRuntime is
        {
            Configuration.FailExceptions: { Count: >0 } failExceptions
        }
            && testFailed.ExceptionTypes.Any(
                (e) => failExceptions.Contains(e, StringComparer.OrdinalIgnoreCase)
            );
}
