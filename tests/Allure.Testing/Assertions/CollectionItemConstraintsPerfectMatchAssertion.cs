using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class CollectionItemConstraintsPerfectMatchAssertion<TCollection, TItem>(
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

    ImmutableArray<string?>? expectations = null;

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
        this.expectations is { } initiazedExpectations
            ? $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} "
                + $"perfectly matching the following constraints:"
                + string.Join(
                    "",
                    initiazedExpectations.Select((e, ei) =>
                        $"{Environment.NewLine}  {ei + 1}. {e ?? $"a constraint #{ei + 1}"}"
                    )
                )
            : $"to have {this.itemConstraints.Length} {this.itemDescriptionPlural} "
                + "perfectly matching the provided constraints";

    async Task<AssertionResult> ApplyItemConstraints(IEnumerable<TItem> items)
    {
        var list = items.ToImmutableArray();
        var actualCount = list.Length;

        if (actualCount != this.ExpectedCount)
        {
            return AssertionResult.Failed(
                $"the collection had {actualCount} {this.itemDescriptionPlural}"
            );
        }

        AssertionResult[,] matchMatrix = new AssertionResult[this.ExpectedCount, actualCount];

        for (int i = 0; i < actualCount; i++)
        {
            var item = list[i];
            for (int j = 0; j < this.ExpectedCount; j++)
            {
                var constraint = this.itemConstraints[j];

                matchMatrix[i, j] = constraint is not null
                    ? await AssertionFunctions.ExecuteInlineAssertionAsync(
                        item,
                        $"{this.itemDescription}s[{i}]",
                        constraint
                    )
                    : AssertionResult.Passed;
            }
        }

        if (TryFindUnmatchedItems(matchMatrix) is { Length: >0 } mismatches)
        {
            this.expectations = mismatches[0].failures
                .Select(e => e.expectation)
                .ToImmutableArray();

            return AssertionResult.Failed(
                $"Couldn't find a perfect match:"
                    + string.Join(
                        "",
                        mismatches.Select((m) =>
                            $"{Environment.NewLine}  - {this.itemDescription} #{m.index + 1} didn't match"
                                + string.Join(
                                    "",
                                    m.failures.Select((f, ci) =>
                                        $"{Environment.NewLine}    - constraint {ci + 1} because {f.actual}"
                                    )
                                )
                        )
                    )
            );
        }

        return AssertionResult.Passed;
    }

    static ImmutableArray<(int index, ImmutableArray<(string? expectation, string? actual)> failures)> TryFindUnmatchedItems(AssertionResult[,] matches)
    {
        var itemCount = matches.GetLength(0);
        var constraintCount = matches.GetLength(1);

        var itemByConstraint = Enumerable.Repeat(-1, constraintCount).ToArray();

        return [.. Enumerable.Range(0, itemCount)
            .Where((i) => !TryMatchItem(i, new bool[constraintCount]))
            .Select((i) => (i, Enumerable.Range(0, constraintCount)
                .Select((j) => NarrowingFunctions.ExtractExpectedAndActual(matches[i, j].Message, 2))
                .ToImmutableArray()))];

        bool TryMatchItem(int item, bool[] seenConstraints)
        {
            for (var constraint = 0; constraint < constraintCount; constraint++)
            {
                if (!matches[item, constraint].IsPassed || seenConstraints[constraint])
                {
                    continue;
                }

                seenConstraints[constraint] = true;

                var previousItem = itemByConstraint[constraint];

                if (previousItem == -1 || TryMatchItem(previousItem, seenConstraints))
                {
                    itemByConstraint[constraint] = item;
                    return true;
                }
            }

            return false;
        }
    }
}
