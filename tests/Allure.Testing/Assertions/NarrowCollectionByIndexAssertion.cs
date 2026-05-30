using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class NarrowCollectionByIndexAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    int index,
    string itemDescription
) :
    Assertion<TItem>(context.Map(c => c switch
    {
        null => throw new InvalidOperationException("the collection was null"),
        { Count: var count } =>
            count > index
                ? c[index]
                : throw new InvalidOperationException(
                    $"the collection has only {count} {itemDescription}s"),
    }))

    where TCollection : IReadOnlyList<TItem>

{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TItem> metadata
    ) =>
        await NarrowingFunctions.CheckAsync(metadata);


    public new OrContinuation<TItem> Or => NarrowingFunctions.GetThrowingOr<TItem>();

    protected override string GetExpectation() =>
        NarrowingFunctions.GetByIndexExpectation(itemDescription, index);
}