using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class CollectionFindSingleItemByCriteriaAssertion<C, E, F> : Assertion<F>
    where C : IReadOnlyList<E>
{
    readonly string itemDescription;
    readonly State state;

    class State
    {
        public Assertion<F>? lastAssertion = null;

        public AssertionResult TakeAssertion((AssertionResult Result, Assertion<F>? Assertion) output)
        {
            var (result, assertion) = output;
            this.lastAssertion = assertion;
            return result;
        }
    }

    public CollectionFindSingleItemByCriteriaAssertion(
        AssertionContext<C> context,
        Func<IAssertionSource<F>, Assertion<F>?> criteria,
        Func<E, F> mapper,
        Func<F, string?> validator,
        string itemDescription
    ) : this(context, criteria, mapper, validator, itemDescription, new())
    {
    }

    private CollectionFindSingleItemByCriteriaAssertion(
        AssertionContext<C> context,
        Func<IAssertionSource<F>, Assertion<F>?> criteria,
        Func<E, F> mapper,
        Func<F, string?> validator,
        string itemDescription,
        State state
    ) : base(context.Map(async c => await MapToSingleItemAsync(c, criteria, mapper, validator, itemDescription, state)))
    {
        this.itemDescription = itemDescription;
        this.state = state;
    }

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<F> metadata
    ) =>
        metadata is { Exception.Message: var message }
            ? await Task.FromResult(AssertionResult.Failed(message))
            : await Task.FromResult(AssertionResult.Passed);

    protected override string GetExpectation() =>
        AssertionFunctions.GetAssertionExpectation(this.state.lastAssertion) is { } elementExpectation
            ? $"to have a single {this.itemDescription} {elementExpectation}"
            : $"to have a single {this.itemDescription} matching the provided criteria";

    static async Task<F> MapToSingleItemAsync(
        IReadOnlyList<E>? sequence,
        Func<IAssertionSource<F>, Assertion<F>?> criteria,
        Func<E, F> mapper,
        Func<F, string?> validator,
        string itemDescription,
        State state
    ) =>
        sequence switch
        {
            [] => throw new InvalidOperationException("the collection was empty"),
            null => throw new InvalidOperationException("the collection was null"),
            _ => await FindMatchesAsync(
                sequence.Select(e => mapper(e)),
                CreateMatchCriteriaPredicate(criteria, itemDescription, state)
            ) switch
            {
                (var (item, index), null) => validator(item) switch
                {
                    null => item,
                    { } error => throw new InvalidOperationException(error),
                },

                var ((item1, index1), (item2, index2)) =>
                    throw new InvalidOperationException(
                        $"{itemDescription}s at {index1} and {index2} both matched the criteria"),

                _ =>
                    throw new InvalidOperationException($"no {itemDescription} matched the criteria"),
            },
        };

    static async Task<((F, int)? FirstMatch, (F, int)? SecondMatch)> FindMatchesAsync(
        IEnumerable<F> sequence,
        Func<F, int, Task<bool>> asyncPredicate
    )
    {
        (F, int)? firstMatch = default;
        (F, int)? secondMatch = default;

        int i = 0;
        foreach (var element in sequence)
        {
            if (await asyncPredicate(element, i++))
            {
                if (firstMatch.HasValue)
                {
                    if (secondMatch.HasValue)
                    {
                        break;
                    }

                    secondMatch = (element, i);
                }
                else
                {
                    firstMatch = (element, i);
                }
            }
        }

        return (firstMatch, secondMatch);
    }

    static Func<F, int, Task<bool>> CreateMatchCriteriaPredicate(
        Func<IAssertionSource<F>, Assertion<F>?> criteria,
        string itemDescription,
        State state
    ) =>
        async (testResult, i) =>
            state.TakeAssertion(
                await AssertionFunctions.ExecuteInlineAssertionAsync(
                    testResult,
                    $"{itemDescription} at {i}",
                    criteria)
            ).IsPassed;
}