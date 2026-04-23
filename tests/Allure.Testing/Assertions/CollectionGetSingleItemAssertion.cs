using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class CollectionGetSingleItemAssertion<C, E, F>(
    AssertionContext<C> context,
    Func<E, F> mapper,
    Func<F, string?> validator,
    string itemDescription
) :
    Assertion<F>(context.Map(c => c switch
    {
        [var single] => mapper(single) switch
        {
            var mapped => validator(mapped) switch
            {
                null => mapped,
                { } error => throw new InvalidOperationException(error),
            },
        },
        [] => throw new InvalidOperationException("nothing was found"),
        not null => throw new InvalidOperationException($"{c.Count} were received"),
        null => throw new InvalidOperationException("the collection was null"),
    }))

    where C : IReadOnlyList<E>

{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<F> metadata
    ) =>
        metadata is { Exception.Message: var message }
            ? await Task.FromResult(AssertionResult.Failed(message))
            : await Task.FromResult(AssertionResult.Passed);

    protected override string GetExpectation() => $"a single {itemDescription} to exist";
}