using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class CollectionMatchesItemConstraintsAssertion<C, E>(
    AssertionContext<C> context,
    Func<IAssertionSource<E>, Assertion<E>?>[] itemConstraints,
    string itemDescription
) :
    Assertion<C>(context)

    where C : IReadOnlyList<E>
{
    readonly Func<IAssertionSource<E>, Assertion<E>?>[] itemConstraints = itemConstraints;
    readonly string itemDescription = itemDescription;
    readonly string itemDescriptionPlural =
        itemConstraints.Length == 1
            ? itemDescription
            : $"{itemDescription}s";

    (int, Assertion<E>?)? lastCheckedItem = null;

    int ExpectedCount => this.itemConstraints.Length;

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<C> metadata
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
        this.lastCheckedItem is var (index, assertion)
            ? AssertionFunctions.GetAssertionExpectation(assertion) is { } itemExpectation
                ? $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} and {this.itemDescription} at {index} {itemExpectation}"
                : $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} and {this.itemDescription} at {index} satisfying the corresponding constraints"
            : $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} each satisfying the corresponding constraints";

    async Task<AssertionResult> ApplyItemConstraints(IReadOnlyList<E> items)
    {
        var actualCount = items.Count;

        if (actualCount == 0)
        {
            return AssertionResult.Failed($"the collection was empty");
        }

        if (actualCount != this.ExpectedCount)
        {
            return AssertionResult.Failed(
                $"the collection had {actualCount} {this.itemDescriptionPlural}"
            );
        }

        for (var i = 0; i < actualCount; i++)
        {
            var item = items[i];
            var constraint = this.itemConstraints[i];

            var (result, assertion) = await AssertionFunctions.ExecuteInlineAssertionAsync(
                item,
                $"{this.itemDescription} at {i}",
                constraint
            );

            this.lastCheckedItem = (i, assertion);

            if (!result.IsPassed)
            {
                return AssertionResult.Failed(
                    $"{this.itemDescription} at {i} {result.Message}"
                );
            }
        }

        return AssertionResult.Passed;
    }
}