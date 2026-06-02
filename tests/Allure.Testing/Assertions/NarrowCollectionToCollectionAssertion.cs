using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing.Assertions;

public class NarrowCollectionToCollectionAssertion<TCollection, TItemCollection, TItem>(
    AssertionContext<TCollection> context,
    string itemDescription
) :
    CollectionAssertionBase<TItemCollection, TItem>(
        NarrowingFunctions.MapToSingle<TCollection, TItemCollection>(context)
    )

    where TCollection : IEnumerable<TItemCollection>
    where TItemCollection : IEnumerable<TItem>

{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TItemCollection> metadata
    ) =>
        await NarrowingFunctions.CheckAsync(metadata);


    public new OrContinuation<TItemCollection> Or =>
        NarrowingFunctions.GetThrowingOr<TItemCollection>();

    protected override string GetExpectation() => $"a single {itemDescription}";
}