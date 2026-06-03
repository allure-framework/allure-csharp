using System;
using System.Collections.Generic;
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
    ) =>
        await ExecuteInlineAssertionAsyncInternal<ValueAssertion<T>, T>(
            assertion,
            new ValueAssertion<T>(actualValue, label));

    public static async Task<AssertionResult> ExecuteInlineAssertionAsync<TItem>(
        IEnumerable<TItem> actualValue,
        string label,
        Func<CollectionAssertion<TItem>, IAssertion?> assertion
    ) =>
        await ExecuteInlineAssertionAsyncInternal<CollectionAssertion<TItem>, IEnumerable<TItem>>(
            assertion,
            new CollectionAssertion<TItem>(actualValue, label));

    static async Task<AssertionResult> ExecuteInlineAssertionAsyncInternal<TSource, TValue>(
        Func<TSource, IAssertion?> assertion,
        TSource source
    )
        where TSource : IAssertionSource<TValue>
    {
        IAssertion? resultingAssertion = assertion(source);
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