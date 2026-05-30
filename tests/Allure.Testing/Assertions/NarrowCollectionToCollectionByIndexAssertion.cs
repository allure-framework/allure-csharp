using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing.Assertions;

public class NarrowCollectionToCollectionByIndexAssertion<TCollection, TItemCollection, TItem>(
    AssertionContext<TCollection> context,
    int index,
    string itemDescription
) :
    CollectionAssertionBase<TItemCollection, TItem>(
        NarrowingFunctions.MapByIndex<TCollection, TItemCollection>(
            context,
            itemDescription,
            index))

    where TCollection : IReadOnlyList<TItemCollection>
    where TItemCollection : IReadOnlyList<TItem>

{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TItemCollection> metadata
    ) =>
        await NarrowingFunctions.CheckAsync(metadata);


    public new OrContinuation<TItemCollection> Or =>
        NarrowingFunctions.GetThrowingOr<TItemCollection>();

    protected override string GetExpectation() =>
        NarrowingFunctions.GetByIndexExpectation(
            itemDescription,
            index);
}