using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class HasOneComparableItemAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    TItem other,
    IEqualityComparer<TItem> comparer,
    string itemDescription
) :
    Assertion<TCollection>(context)

    where TCollection : IEnumerable<TItem>
{
    readonly IEqualityComparer<TItem> comparer = comparer;

    readonly string itemDescription = itemDescription;

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TCollection> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: { } collection } => collection.ToList() switch
            {
                { Count: 0 } =>
                    AssertionResult.Failed("the collection was empty"),

                var list =>
                    list.Count(item => comparer.Equals(item, other)) switch
                    {
                        0 => AssertionResult.Failed("nothing was found"),
                        1 => AssertionResult.Passed,
                        var matchCount => AssertionResult.Failed($"{matchCount} {itemDescription}s were found"),
                    },
            },

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    protected override string GetExpectation() =>
        $"exactly one {itemDescription} equals to {other}";
}