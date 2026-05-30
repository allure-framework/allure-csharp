using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Allure.Testing.Assertions;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal.TUnitAccessors;
using TUnit.Assertions.Core;

namespace Allure.Testing;

public static partial class AllureAssertionExtensions
{
    extension (IAssertionSource<AllureResults2> source)
    {
        public NarrowCollectionAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasSingleTestResult()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleTestResult)}()");

            return new(source.Context.Map(ctx => ctx!.TestResults), "test result");
        }

        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasOnlyOneTestResult(
            Func<IAssertionSource<AllureTestResult>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneTestResult)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), criteria, "test result");
        }

        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasOnlyOneTestResult(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneTestResult)}({expression ?? "..."})");

            return new(
                source.Context.Map(
                    ctx => ctx!.TestResults),
                    (tr) => tr.HasName(name),
                    "test result");
        }

        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasOnlyOneTestResult(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneTestResult)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(
                source.Context.Map(
                    ctx => ctx!.TestResults),
                    (tr) => tr.HasName(name, comparer),
                    "test result");
        }

        public NarrowCollectionByIndexAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasTestResultAt(
            int index,
            [CallerArgumentExpression(nameof(index))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasTestResultAt)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), index, "test result");
        }

        public CollectionItemConstraintsAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasTestResults(
            Func<IAssertionSource<AllureTestResult>, IAssertion>[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasTestResults)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), constraints, "test result");
        }
    }

    extension<TObject, TProperty, TValue> (NarrowToJsonPropertyAssertion<TObject, TProperty, TValue> source)
        where TObject : IAllureModelObject<TObject>, TProperty
        where TProperty: IAllureProperty<TValue, TObject>
    {
        public AndContinuation<TValue> That
        {
            get
            {
                var and = source.And;
                var expressionBuilder = and.Context.ExpressionBuilder;

                // .And -> .That
                expressionBuilder.Length -= 4;
                expressionBuilder.Append(".That");

                return and;
            }
        }
    }

    extension<TObject, TProperty, TValue, TItem> (NarrowToJsonCollectionPropertyAssertion<TObject, TProperty, TValue, TItem> source)
        where TObject : IAllureModelObject<TObject>, TProperty
        where TProperty: IAllureProperty<TValue, TObject>
        where TValue : IReadOnlyList<TItem>
    {
        public CollectionAndContinuation<TValue, TItem> That
        {
            get
            {
                var and = source.And!;

                var context = AssertionAccessors<TValue>.GetContext(and);

                var expressionBuilder = context.ExpressionBuilder;

                // .And -> .That
                expressionBuilder.Length -= 4;
                expressionBuilder.Append(".That");

                return and;
            }
        }
    }

    extension<TCollection, TItem> (NarrowCollectionAssertion<TCollection, TItem> source)
        where TCollection : IReadOnlyList<TItem>
    {
        public AndContinuation<TItem> That
        {
            get
            {
                var and = source.And!;
                var context = and.Context;

                var expressionBuilder = context.ExpressionBuilder;

                // .And -> .That
                expressionBuilder.Length -= 4;
                expressionBuilder.Append(".That");

                return and;
            }
        }
    }

    extension<TCollection, TItemCollection, TItem> (
        NarrowCollectionToCollectionAssertion<TCollection, TItemCollection, TItem> source)

        where TCollection : IReadOnlyList<TItemCollection>
        where TItemCollection : IReadOnlyList<TItem>
    {
        public CollectionAndContinuation<TItemCollection, TItem> That
        {
            get
            {
                var and = source.And!;

                var context = AssertionAccessors<TItemCollection>.GetContext(and);

                var expressionBuilder = context.ExpressionBuilder;

                // .And -> .That
                expressionBuilder.Length -= 4;
                expressionBuilder.Append(".That");

                return and;
            }
        }
    }

    extension<T> (Assertion<T> source)
        where T: IAllureModelObject<T>
    {
        public PropertyAssertionFactory<T> With
        {
            get
            {
                var and = source.And;
                var context = and.Context;
                var expressionBuilder = context.ExpressionBuilder;

                // .And -> .With
                expressionBuilder.Length -= 4;
                expressionBuilder.Append(".With");

                return new(context);
            }
        }
    }
}
