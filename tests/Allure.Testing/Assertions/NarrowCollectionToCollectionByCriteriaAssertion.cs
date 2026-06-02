using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing.Assertions;

public class NarrowCollectionToCollectionByCriteriaAssertion<TCollection, TItemCollection, TItem>
    : CollectionAssertionBase<TItemCollection, TItem>

    where TCollection : IEnumerable<TItemCollection>
    where TItemCollection : IEnumerable<TItem>
{
    readonly string itemDescription;
    readonly NarrowingFunctions.State<TItemCollection> state;

    public NarrowCollectionToCollectionByCriteriaAssertion(
        AssertionContext<TCollection> context,
        Func<IAssertionSource<TItemCollection>, IAssertion?> criteria,
        string itemDescription
    ) : this(context, criteria, itemDescription, new())
    {
    }

    private NarrowCollectionToCollectionByCriteriaAssertion(
        AssertionContext<TCollection> context,
        Func<IAssertionSource<TItemCollection>, IAssertion?> criteria,
        string itemDescription,
        NarrowingFunctions.State<TItemCollection> state
    ) : base(NarrowingFunctions.MapByCriteria(context, criteria, itemDescription, state))
    {
        this.itemDescription = itemDescription;
        this.state = state;
    }

    public new OrContinuation<TItemCollection> Or =>
        NarrowingFunctions.GetThrowingOr<TItemCollection>();

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TItemCollection> metadata
    ) =>
        await NarrowingFunctions.CheckAsync(metadata);

    protected override string GetExpectation() =>
        NarrowingFunctions.GetByCriteriaExpectation(this.itemDescription, this.state);
}