using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using TUnit.Assertions.Core;
using Allure.Testing.Internal;
using System.Linq;

namespace Allure.Testing.Assertions;

public class HasItemByCriteriaAssertion<TCollection, TItem>(
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

            { Value: { } collection } => collection.ToList() switch
            {
                { Count: 0 } =>
                    AssertionResult.Failed("the collection was empty"),

                var list =>
                    await this.ApplyCriteriaAsync(list) switch
                    {
                        (0, var errors) => AssertionResult.Failed(
                            $"no {itemDescription} matched the criteria:{Environment.NewLine}"
                                + NarrowingFunctions.FormatMismatches(itemDescription, errors)),
                        _ => AssertionResult.Passed,
                    },
            },

            _ => await Task.FromResult(AssertionResult.Failed("the collection was null")),
        };

    async Task<(int, List<NarrowingFunctions.CriteriaMatchFailure>)> ApplyCriteriaAsync(List<TItem> items)
    {
        int matches = 0;
        List<NarrowingFunctions.CriteriaMatchFailure> errors = [];
        for (int i = 0; i < items.Count; i++)
        {
            var item = items[i];
            var result = await AssertionFunctions.ExecuteInlineAssertionAsync(
                item,
                $"{this.itemDescription}s[{i}]",
                this.criteria
            );

            if (result.IsPassed)
            {
                matches++;
            }
            else
            {
                var (expected, actual) = NarrowingFunctions.ExtractExpectedAndActual(result.Message, 0);
                if (this.expectation is null || expected is not null && expected.Length > this.expectation.Length)
                {
                    this.expectation = expected;
                }

                errors.Add(new(expected, actual, i + 1));
            }
        }

        return (matches, errors);
    }

    protected override string GetExpectation() =>
        this.expectation is not null
            ? $"at least one {itemDescription} with {this.expectation}"
            : $"at least one {itemDescription} matching the provided criteria";
}