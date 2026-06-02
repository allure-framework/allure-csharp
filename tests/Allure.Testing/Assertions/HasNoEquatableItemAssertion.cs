using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class HasNoEquatableItemAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    IEquatable<TItem>? other,
    string itemDescription
) :
    Assertion<TCollection>(context)

    where TCollection : IEnumerable<TItem>
{
    readonly IEquatable<TItem>? other = other;

    readonly string itemDescription = itemDescription;

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TCollection> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: { } items } =>
                items.Count(item => other?.Equals(item) ?? item is null) switch
                {
                    0 => AssertionResult.Passed,
                    _ => AssertionResult.Failed($"a matching {itemDescription} was found"),
                },

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    protected override string GetExpectation() =>
        $"no {itemDescription} equals to {other}";
}