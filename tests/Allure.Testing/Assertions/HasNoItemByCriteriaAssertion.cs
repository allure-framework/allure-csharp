using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions.Core;
using Allure.Testing.Internal;
using System.Linq;

namespace Allure.Testing.Assertions;

public class HasNoItemByCriteriaAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    Func<IAssertionSource<TItem>, IAssertion> criteria,
    string itemDescription
) :
    Assertion<TCollection>(context)

    where TCollection : IEnumerable<TItem>
{
    readonly Func<IAssertionSource<TItem>, IAssertion> criteria = criteria;

    readonly string itemDescription = itemDescription;

    string? expectation = null;

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TCollection> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: { } items } =>
                await this.ApplyCriteriaAsync(items) switch
                {
                    [] => AssertionResult.Passed,

                    { Count: var count } matches => AssertionResult.Failed(
                        $"{itemDescription}{(count == 1 ? "" : "s")} {FormatMatches(matches)} matched the criteria"),
                },

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    async Task<List<int>> ApplyCriteriaAsync(IEnumerable<TItem> items)
    {
        int i = -1;
        List<int> matches = [];

        foreach (var item in items)
        {
            i++;

            var result = await AssertionFunctions.ExecuteInlineAssertionAsync(
                item,
                $"{this.itemDescription}s[{i}]",
                this.criteria
            );

            if (result.IsPassed)
            {
                matches.Add(i + 1);
            }
            else
            {
                var (expected, actual) = NarrowingFunctions.ExtractExpectedAndActual(result.Message, 0);
                if (this.expectation is null || expected is not null && expected.Length > this.expectation.Length)
                {
                    this.expectation = expected;
                }
            }
        }

        return matches;
    }

    static string FormatMatches(List<int> matches) => matches switch
    {
        [] => "",
        [var single] => $"#{single}",
        [.. var head, var last] => $"{string.Join(", ", head.Select(m => $"#{m}"))} and #{last}",
    };

    protected override string GetExpectation() =>
        this.expectation is not null
            ? $"no {itemDescription} with {this.expectation}"
            : $"no {itemDescription} matching the provided criteria";
}