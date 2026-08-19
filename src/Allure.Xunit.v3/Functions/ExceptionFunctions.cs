using System;
using System.Linq;
using Allure.Xunit.Internal.Registration;
using Xunit.Sdk;

namespace Allure.Xunit.Functions;

static class ExceptionFunctions
{
    public static bool IsConfiguredAssertionFailure(ITestFailed testFailed) =>
        AllureXunitRegistration.Current is
        {
            ConfigurationReference.Value.FailExceptions: { Count: >0 } failExceptions
        }
            && testFailed.ExceptionTypes.Any(
                (e) => failExceptions.Contains(e, StringComparer.OrdinalIgnoreCase)
            );
}
