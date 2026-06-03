using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class CollectionItemConstraintsAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    Func<IAssertionSource<TItem>, IAssertion?>?[] itemConstraints,
    string itemDescription
) :
    Assertion<TCollection>(context)

    where TCollection : IEnumerable<TItem>
{
    readonly Func<IAssertionSource<TItem>, IAssertion?>?[] itemConstraints = itemConstraints;
    readonly string itemDescription = itemDescription;
    readonly string itemDescriptionPlural =
        itemConstraints.Length == 1
            ? itemDescription
            : $"{itemDescription}s";

    (int, string?)? lastExpectation = null;

    int ExpectedCount => this.itemConstraints.Length;

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TCollection> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: { } items } =>
                await this.ApplyItemConstraints(items),

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    protected override string GetExpectation() =>
        this.lastExpectation is var (index, expectation)
            ? expectation is not null
                ? $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} and {this.itemDescription} at {index} {expectation}"
                : $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} and {this.itemDescription} at {index} satisfying the corresponding constraints"
            : $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} each satisfying the corresponding constraints";

    async Task<AssertionResult> ApplyItemConstraints(IEnumerable<TItem> items)
    {
        var list = items.ToList();
        var actualCount = list.Count;

        if (actualCount != this.ExpectedCount)
        {
            return AssertionResult.Failed(
                $"the collection had {actualCount} {this.itemDescriptionPlural}"
            );
        }

        for (var i = 0; i < actualCount; i++)
        {
            var item = list[i];
            var constraint = this.itemConstraints[i];

            var result = constraint is not null
                ? await AssertionFunctions.ExecuteInlineAssertionAsync(
                    item,
                    $"{this.itemDescription}s[{i}]",
                    constraint
                )
                : AssertionResult.Passed;


            if (!result.IsPassed)
            {
                var (expected, actual) = NarrowingFunctions.ExtractExpectedAndActual(result.Message, 0);

                this.lastExpectation = (i, expected);
                return AssertionResult.Failed(
                    $"{this.itemDescription} at {i} {actual}"
                );
            }
        }

        return AssertionResult.Passed;
    }
}