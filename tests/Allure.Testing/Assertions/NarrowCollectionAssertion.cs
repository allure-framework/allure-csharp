using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class NarrowCollectionAssertion<TCollection, TItem>(
    AssertionContext<TCollection> context,
    string itemDescription
) :
    Assertion<TItem>(
        NarrowingFunctions.MapToSingle<TCollection, TItem>(context)
    )

    where TCollection : IReadOnlyList<TItem>

{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TItem> metadata
    ) =>
        await NarrowingFunctions.CheckAsync(metadata);


    public new OrContinuation<TItem> Or => NarrowingFunctions.GetThrowingOr<TItem>();

    protected override string GetExpectation() =>
        NarrowingFunctions.GetSingleExpectation(itemDescription);
}