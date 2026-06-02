using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class HasNoComparableItemAssertion<TCollection, TItem>(
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

            { Value: { } items } =>
                items.Count(item => comparer.Equals(item, other)) switch
                {
                    0 => AssertionResult.Passed,
                    _ => AssertionResult.Failed($"a matching {itemDescription} was found"),
                },

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    protected override string GetExpectation() =>
        $"no {itemDescription} equals to {other}";
}