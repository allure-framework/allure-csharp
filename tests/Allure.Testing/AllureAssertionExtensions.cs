using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Allure.Testing.Assertions;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal.TUnitAccessors;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing;

public static partial class AllureAssertionExtensions
{
    extension (IAssertionSource<AllureResults2> source)
    {
        /// <summary>
        /// Checks if exactly one test result was written to the output and narrows the assertion chain to that result.
        /// </summary>
        public NarrowCollectionAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasSingleTestResult()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleTestResult)}()");

            return new(source.Context.Map(ctx => ctx!.TestResults), "test result");
        }

        /// <summary>
        /// Checks if exactly one test result matches the provided criteria and narrows the assertion chain to that result.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasOnlyOneTestResult(
            Func<IAssertionSource<AllureTestResult>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneTestResult)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), criteria, "test result");
        }

        /// <summary>
        /// Checks if exactly one test result has the provided name and narrows the assertion chain to that result.
        /// </summary>
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

        /// <summary>
        /// Checks if exactly one test result has the provided name and narrows the assertion chain to that result.
        /// </summary>
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

        /// <summary>
        /// Checks if enough test results were written to the output and narrows the assertion chain to the result
        /// at the specified index.
        /// </summary>
        public NarrowCollectionByIndexAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasTestResultAt(
            int index,
            [CallerArgumentExpression(nameof(index))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasTestResultAt)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), index, "test result");
        }

        /// <summary>
        /// Checks if the exact number of test results were written to the output and each result satisfies the corresponding
        /// constraints.
        /// </summary>
        /// <remarks>
        /// Pass <c>null</c> or a function returning <c>null</c> for a noop constraint.
        /// </remarks>
        public CollectionItemConstraintsAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasTestResults(
            Func<IAssertionSource<AllureTestResult>, IAssertion?>?[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasTestResults)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), constraints, "test result");
        }

        /// <summary>
        /// Checks if exactly one container was written to the output and narrows the assertion chain to that container.
        /// </summary>
        public NarrowCollectionAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasSingleContainer()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleContainer)}()");

            return new(source.Context.Map(ctx => ctx!.Containers), "container");
        }

        /// <summary>
        /// Checks if exactly one container matches the provided criteria and narrows the assertion chain to that container.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasOnlyOneContainer(
            Func<IAssertionSource<AllureContainer>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneContainer)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.Containers), criteria, "container");
        }

        /// <summary>
        /// Checks if exactly one container has the provided name and narrows the assertion chain to that container.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasOnlyOneContainer(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneContainer)}({expression ?? "..."})");

            return new(
                source.Context.Map(
                    ctx => ctx!.Containers),
                    (tr) => tr.HasName(name),
                    "container");
        }

        /// <summary>
        /// Checks if exactly one container has the provided name and narrows the assertion chain to that container.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasOnlyOneContainer(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasOnlyOneContainer)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(
                source.Context.Map(
                    ctx => ctx!.Containers),
                    (tr) => tr.HasName(name, comparer),
                    "container");
        }

        /// <summary>
        /// Checks if enough containers were written to the output and narrows the assertion chain to the container
        /// at the specified index.
        /// </summary>
        public NarrowCollectionByIndexAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasContainerAt(
            int index,
            [CallerArgumentExpression(nameof(index))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasContainerAt)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.Containers), index, "container");
        }

        /// <summary>
        /// Checks if the exact number of containers were written to the output and each container satisfies
        /// the corresponding constraints.
        /// </summary>
        /// <remarks>
        /// Pass <c>null</c> or a function returning <c>null</c> for a noop constraint.
        /// </remarks>
        public CollectionItemConstraintsAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasContainers(
            Func<IAssertionSource<AllureContainer>, IAssertion?>?[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasContainers)}({expression ?? "..."})");

            return new(source.Context.Map(static ctx => ctx!.Containers), constraints, "container");
        }
    }

    extension<TObject, TProperty, TValue> (NarrowToJsonPropertyAssertion<TObject, TProperty, TValue> source)
        where TObject : IAllureModelObject<TObject>, TProperty
        where TProperty: IAllureProperty<TValue, TObject>
    {
        /// <summary>
        /// A readability alias for <see cref="Assertion{TValue}.And"/>.
        /// </summary>
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
        /// <summary>
        /// A readability alias for <see cref="CollectionAssertionBase{TValue, TItem}.And"/>.
        /// </summary>
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
        /// <summary>
        /// A readability alias for <see cref="Assertion{TItem}.And"/>.
        /// </summary>
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
        /// <summary>
        /// A readability alias for <see cref="CollectionAssertionBase{TItemCollection, TItem}.And"/>.
        /// </summary>
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

    extension<TCollection, TItem> (NarrowCollectionByCriteriaAssertion<TCollection, TItem> source)
        where TCollection : IReadOnlyList<TItem>
    {
        /// <summary>
        /// A readability alias for <see cref="Assertion{TItem}.And"/>.
        /// </summary>
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
        NarrowCollectionToCollectionByCriteriaAssertion<TCollection, TItemCollection, TItem> source)

        where TCollection : IReadOnlyList<TItemCollection>
        where TItemCollection : IReadOnlyList<TItem>
    {
        /// <summary>
        /// A readability alias for <see cref="CollectionAssertionBase{TItemCollection, TItem}.And"/>.
        /// </summary>
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

    extension<TCollection, TItem> (NarrowCollectionByIndexAssertion<TCollection, TItem> source)
        where TCollection : IReadOnlyList<TItem>
    {
        /// <summary>
        /// A readability alias for <see cref="Assertion{TItem}.And"/>.
        /// </summary>
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
        NarrowCollectionToCollectionByIndexAssertion<TCollection, TItemCollection, TItem> source)

        where TCollection : IReadOnlyList<TItemCollection>
        where TItemCollection : IReadOnlyList<TItem>
    {
        /// <summary>
        /// A readability alias for <see cref="CollectionAssertionBase{TItemCollection, TItem}.And"/>.
        /// </summary>
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
        /// <summary>
        /// A readability alias for <see cref="Assertion{T}.And"/>.
        /// </summary>
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
