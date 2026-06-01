using System;
using System.Reflection;
using System.Threading.Tasks;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing.Internal;

public static class AssertionFunctions
{
    public static async Task<AssertionResult> ExecuteInlineAssertionAsync<T>(
        T actualValue,
        string label,
        Func<IAssertionSource<T>, IAssertion?> assertion
    )
    {
        ValueAssertion<T> arg = new(actualValue, label);
        IAssertion? resultingAssertion = assertion(arg);
        if (resultingAssertion == null)
        {
            return AssertionResult.Passed;
        }

        try
        {
            await resultingAssertion.AssertAsync();
            return AssertionResult.Passed;
        }
        catch (Exception ex)
        {
            return AssertionResult.Failed(ex.Message);
        }
    }

    public static string? GetAssertionExpectation(IAssertion? assertion)
    {
        if (assertion is null)
        {
            return null;
        }

        var method =
            assertion
                .GetType()
                .GetMethod(
                    "GetExpectation",
                    BindingFlags.Instance | BindingFlags.NonPublic, []);

        return (string?)method?.Invoke(assertion, []);
    }

    public static string? GetAssertionFailureMessage(IAssertion? assertion, AssertionResult result)
    {
        if (assertion is null)
        {
            return null;
        }

        var method =
            assertion
                .GetType()
                .GetMethod(
                    "GetExpectation",
                    BindingFlags.Instance | BindingFlags.NonPublic, []);

        return (string?)method?.Invoke(assertion, []);
    }
}