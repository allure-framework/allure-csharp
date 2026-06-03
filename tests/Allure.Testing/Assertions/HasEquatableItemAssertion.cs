using System;
using System.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class HasEquatableItemAssertion<TCollection, TItem>(
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

            { Value: { } collection } => collection.ToList() switch
            {
                { Count: 0 } =>
                    AssertionResult.Failed("the collection was empty"),

                var list =>
                    list.Count(item => other?.Equals(item) ?? item is null) switch
                    {
                        0 => AssertionResult.Failed("nothing was found"),
                        _ => AssertionResult.Passed,
                    },
            },

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    protected override string GetExpectation() =>
        $"at least one {itemDescription} equals to {other}";
}