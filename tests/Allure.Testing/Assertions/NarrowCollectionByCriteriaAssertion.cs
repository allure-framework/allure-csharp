using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class NarrowCollectionByCriteriaAssertion<TCollection, TItem> : Assertion<TItem>
    where TCollection : IReadOnlyList<TItem>
{
    readonly string itemDescription;
    readonly NarrowingFunctions.State<TItem> state;

    public NarrowCollectionByCriteriaAssertion(
        AssertionContext<TCollection> context,
        Func<IAssertionSource<TItem>, IAssertion?> criteria,
        string itemDescription
    ) : this(context, criteria, itemDescription, new())
    {
    }

    private NarrowCollectionByCriteriaAssertion(
        AssertionContext<TCollection> context,
        Func<IAssertionSource<TItem>, IAssertion?> criteria,
        string itemDescription,
        NarrowingFunctions.State<TItem> state
    ) : base(NarrowingFunctions.MapByCriteria(context, criteria, itemDescription, state))
    {
        this.itemDescription = itemDescription;
        this.state = state;
    }

    public new OrContinuation<TItem> Or =>
        NarrowingFunctions.GetThrowingOr<TItem>();

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TItem> metadata
    ) =>
        await NarrowingFunctions.CheckAsync(metadata);

    protected override string GetExpectation() =>
        NarrowingFunctions.GetByCriteriaExpectation(this.itemDescription, this.state);
}