using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using Allure.Testing.Assertions;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal.TUnitAccessors;
using TUnit.Assertions.Conditions;
using TUnit.Assertions.Core;
using TUnit.Assertions.Sources;

namespace Allure.Testing;

public static partial class AllureAssertionExtensions
{
    extension (IAssertionSource<AllureResults> source)
    {
        /// <summary>
        /// Checks if the exact number of test results were written to the output and each result satisfies the corresponding
        /// constraints.
        /// </summary>
        /// <remarks>
        /// Pass <c>null</c> or a function returning <c>null</c> for a no-op constraint.
        /// </remarks>
        public MemberAssertionResult<AllureResults> HasTestResults(
            Func<IAssertionSource<AllureTestResult>, IAssertion?>?[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasTestResults)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new CollectionItemConstraintsAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    constraints,
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains exactly one test result that matches the provided criteria.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasOnlyOneTestResult(
            Func<IAssertionSource<AllureTestResult>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasOnlyOneTestResult)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasOneItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    criteria,
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains exactly one test result with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasOnlyOneTestResult(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasOnlyOneTestResult)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasOneItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    tr => tr.HasName(name),
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains exactly one test result with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasOnlyOneTestResult(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append(
                $".{nameof(HasOnlyOneTestResult)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasOneItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    tr => tr.HasName(name, comparer),
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains at least one test result that matches the provided criteria.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasTestResult(
            Func<IAssertionSource<AllureTestResult>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasTestResult)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    criteria,
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains at least one test result with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasTestResult(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasTestResult)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    tr => tr.HasName(name),
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains at least one test result with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasTestResult(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append(
                $".{nameof(HasTestResult)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    tr => tr.HasName(name, comparer),
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Passes if no test result matches the provided criteria.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasNoTestResult(
            Func<IAssertionSource<AllureTestResult>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasNoTestResult)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasNoItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    criteria,
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Passes if no test result has the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasNoTestResult(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasNoTestResult)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasNoItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    tr => tr.HasName(name),
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Passes if no test result has the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasNoTestResult(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append(
                $".{nameof(HasNoTestResult)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.TestResults,
                tr => new HasNoItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult>(
                    tr.Context,
                    tr => tr.HasName(name, comparer),
                    "test result"));

            expressionBuilder.Length = length;

            return assertion;
        }

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
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasSingleTestResult(
            Func<IAssertionSource<AllureTestResult>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleTestResult)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.TestResults), criteria, "test result");
        }

        /// <summary>
        /// Checks if exactly one test result has the provided name and narrows the assertion chain to that result.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasSingleTestResult(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleTestResult)}({expression ?? "..."})");

            return new(
                source.Context.Map(
                    ctx => ctx!.TestResults),
                    (tr) => tr.HasName(name),
                    "test result");
        }

        /// <summary>
        /// Checks if exactly one test result has the provided name and narrows the assertion chain to that result.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult> HasSingleTestResult(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleTestResult)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

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
        /// Checks if the exact number of containers were written to the output and each container satisfies
        /// the corresponding constraints.
        /// </summary>
        /// <remarks>
        /// Pass <c>null</c> or a function returning <c>null</c> for a no-op constraint.
        /// </remarks>
        public MemberAssertionResult<AllureResults> HasContainers(
            Func<IAssertionSource<AllureContainer>, IAssertion?>?[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasContainers)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new CollectionItemConstraintsAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    constraints,
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains exactly one container that matches the provided criteria.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasOnlyOneContainer(
            Func<IAssertionSource<AllureContainer>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasOnlyOneContainer)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasOneItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    criteria,
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains exactly one container with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasOnlyOneContainer(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasOnlyOneContainer)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasOneItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    c => c.HasName(name),
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains exactly one container with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasOnlyOneContainer(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append(
                $".{nameof(HasOnlyOneContainer)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasOneItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    c => c.HasName(name, comparer),
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains at least one container that matches the provided criteria.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasContainer(
            Func<IAssertionSource<AllureContainer>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasContainer)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    criteria,
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains at least one container with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasContainer(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasContainer)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    c => c.HasName(name),
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Checks if the output contains at least one container with the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasContainer(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append(
                $".{nameof(HasContainer)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    c => c.HasName(name, comparer),
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Passes if no container matches the provided criteria.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasNoContainer(
            Func<IAssertionSource<AllureContainer>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasNoContainer)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasNoItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    criteria,
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Passes if no container has the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasNoContainer(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append($".{nameof(HasNoContainer)}({expression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasNoItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    c => c.HasName(name),
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
        }

        /// <summary>
        /// Passes if no container has the provided name.
        /// </summary>
        public MemberAssertionResult<AllureResults> HasNoContainer(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            var expressionBuilder = ctx.ExpressionBuilder;
            expressionBuilder.Append(
                $".{nameof(HasNoContainer)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");
            var length = expressionBuilder.Length;

            var assertion = source.Member(
                s => s.Containers,
                ca => new HasNoItemByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer>(
                    ca.Context,
                    c => c.HasName(name, comparer),
                    "container"));

            expressionBuilder.Length = length;

            return assertion;
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
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasSingleContainer(
            Func<IAssertionSource<AllureContainer>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleContainer)}({expression ?? "..."})");

            return new(source.Context.Map(ctx => ctx!.Containers), criteria, "container");
        }

        /// <summary>
        /// Checks if exactly one container has the provided name and narrows the assertion chain to that container.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasSingleContainer(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleContainer)}({expression ?? "..."})");

            return new(
                source.Context.Map(
                    ctx => ctx!.Containers),
                    (tr) => tr.HasName(name),
                    "container");
        }

        /// <summary>
        /// Checks if exactly one container has the provided name and narrows the assertion chain to that container.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<ImmutableArray<AllureContainer>, AllureContainer> HasSingleContainer(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleContainer)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

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
    }

    extension<TItem> (IAssertionSource<IList<TItem>> source)
    {
        /// <summary>
        /// Checks if the collection contains the exact number of items and each item satisfies the corresponding
        /// constraints.
        /// </summary>
        /// <remarks>
        /// Pass <c>null</c> or a function returning <c>null</c> for a no-op constraint.
        /// </remarks>
        public CollectionItemConstraintsAssertion<IList<TItem>, TItem> HasItems(
            Func<IAssertionSource<TItem>, IAssertion?>?[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasItems)}({expression ?? "..."})");

            return new(ctx, constraints, "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one item that matches the provided criteria.
        /// </summary>
        public HasOneItemByCriteriaAssertion<IList<TItem>, TItem> HasOnlyOneItem(
            Func<IAssertionSource<TItem>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasOnlyOneItem)}({expression ?? "..."})");

            return new(ctx, criteria, "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one instance of the provided item.
        /// </summary>
        public HasOneComparableItemAssertion<IList<TItem>, TItem> HasOnlyOneItem(
            TItem expectedItem,
            [CallerArgumentExpression(nameof(expectedItem))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasOnlyOneItem)}({expression ?? "..."})");

            return new(ctx, expectedItem, EqualityComparer<TItem>.Default, "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one instance of the provided item.
        /// </summary>
        public HasOneComparableItemAssertion<IList<TItem>, TItem> HasOnlyOneItem(
            TItem expectedItem,
            IEqualityComparer<TItem> comparer,
            [CallerArgumentExpression(nameof(expectedItem))] string? expectedItemExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append(
                $".{nameof(HasOnlyOneItem)}({expectedItemExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(ctx, expectedItem, comparer, "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one instance of the provided item.
        /// </summary>
        public HasOneEquatableItemAssertion<IList<TItem>, TItem> HasOnlyOneItem(
            IEquatable<TItem> expectedItem,
            [CallerArgumentExpression(nameof(expectedItem))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasOnlyOneItem)}({expression ?? "..."})");

            return new(ctx, expectedItem, "item");
        }

        /// <summary>
        /// Checks if the collection contains at least one item that matches the provided criteria.
        /// </summary>
        public HasItemByCriteriaAssertion<IList<TItem>, TItem> HasItem(
            Func<IAssertionSource<TItem>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasItem)}({expression ?? "..."})");

            return new(ctx, criteria, "item");
        }

        /// <summary>
        /// Checks if the collection contains at least one instance of the provided item.
        /// </summary>
        public HasComparableItemAssertion<IList<TItem>, TItem> HasItem(
            TItem expectedItem,
            [CallerArgumentExpression(nameof(expectedItem))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasItem)}({expression ?? "..."})");

            return new(ctx, expectedItem, EqualityComparer<TItem>.Default, "item");
        }

        /// <summary>
        /// Checks if the collection contains at least one instance of the provided item.
        /// </summary>
        public HasComparableItemAssertion<IList<TItem>, TItem> HasItem(
            TItem expectedItem,
            IEqualityComparer<TItem> comparer,
            [CallerArgumentExpression(nameof(expectedItem))] string? expectedItemExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append(
                $".{nameof(HasItem)}({expectedItemExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(ctx, expectedItem, comparer, "item");
        }

        /// <summary>
        /// Checks if the collection contains at least one instance of the provided item.
        /// </summary>
        public HasEquatableItemAssertion<IList<TItem>, TItem> HasItem(
            IEquatable<TItem> expectedItem,
            [CallerArgumentExpression(nameof(expectedItem))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasItem)}({expression ?? "..."})");

            return new(ctx, expectedItem, "item");
        }

        /// <summary>
        /// Passes if the collection does not contain an item that matches the provided criteria.
        /// </summary>
        public HasNoItemByCriteriaAssertion<IList<TItem>, TItem> HasNoItem(
            Func<IAssertionSource<TItem>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasNoItem)}({expression ?? "..."})");

            return new(ctx, criteria, "item");
        }

        /// <summary>
        /// Passes if the collection does not contain the provided item.
        /// </summary>
        public HasNoComparableItemAssertion<IList<TItem>, TItem> HasNoItem(
            TItem expectedItem,
            [CallerArgumentExpression(nameof(expectedItem))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasNoItem)}({expression ?? "..."})");

            return new(ctx, expectedItem, EqualityComparer<TItem>.Default, "item");
        }

        /// <summary>
        /// Passes if the collection does not contain the provided item.
        /// </summary>
        public HasNoComparableItemAssertion<IList<TItem>, TItem> HasNoItem(
            TItem expectedItem,
            IEqualityComparer<TItem> comparer,
            [CallerArgumentExpression(nameof(expectedItem))] string? expectedItemExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append(
                $".{nameof(HasNoItem)}({expectedItemExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(ctx, expectedItem, comparer, "item");
        }

        /// <summary>
        /// Passes if the collection does not contain the provided item.
        /// </summary>
        public HasNoEquatableItemAssertion<IList<TItem>, TItem> HasNoItem(
            IEquatable<TItem> expectedItem,
            [CallerArgumentExpression(nameof(expectedItem))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasNoItem)}({expression ?? "..."})");

            return new(ctx, expectedItem, "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one item that matches the provided criteria
        /// and narrows the assertion chain to that item.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<IList<TItem>, TItem> HasSingleItem(
            Func<IAssertionSource<TItem>, IAssertion> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleItem)}({expression ?? "..."})");

            return new(source.Context, criteria, "item");
        }

        /// <summary>
        /// Checks if the collection contains enough items and narrows the assertion chain to the item at
        /// the specified index.
        /// </summary>
        public NarrowCollectionByIndexAssertion<IList<TItem>, TItem> HasItemAt(
            int index,
            [CallerArgumentExpression(nameof(index))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasItemAt)}({expression ?? "..."})");

            return new(source.Context, index, "item");
        }
    }

    extension<TCollection, TItem> (IAssertionSource<IList<TItem>> source)
        where TItem: IAllureModelObject<TItem>, IAllureNameProperty<TItem>
    {
        /// <summary>
        /// Checks if the collection contains exactly one object with the provided name.
        /// </summary>
        public HasOneItemByCriteriaAssertion<IList<TItem>, TItem> HasOnlyOneItem(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasOnlyOneItem)}({expression ?? "..."})");

            return new(ctx, obj => obj.HasName(name), "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one object with the provided name.
        /// </summary>
        public HasOneItemByCriteriaAssertion<IList<TItem>, TItem> HasOnlyOneItem(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append(
                $".{nameof(HasOnlyOneItem)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(ctx, obj => obj.HasName(name, comparer), "item");
        }

        /// <summary>
        /// Checks if the collection contains at least one object with the provided name.
        /// </summary>
        public HasItemByCriteriaAssertion<IList<TItem>, TItem> HasItem(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasItem)}({expression ?? "..."})");

            return new(ctx, obj => obj.HasName(name), "item");
        }

        /// <summary>
        /// Checks if the collection contains at least one object with the provided name.
        /// </summary>
        public HasItemByCriteriaAssertion<IList<TItem>, TItem> HasItem(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append(
                $".{nameof(HasItem)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(ctx, obj => obj.HasName(name, comparer), "item");
        }

        /// <summary>
        /// Passes if the collection does not contain an object with the provided name.
        /// </summary>
        public HasNoItemByCriteriaAssertion<IList<TItem>, TItem> HasNoItem(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append($".{nameof(HasNoItem)}({expression ?? "..."})");

            return new(ctx, obj => obj.HasName(name), "item");
        }

        /// <summary>
        /// Passes if the collection does not contain an object with the provided name.
        /// </summary>
        public HasNoItemByCriteriaAssertion<IList<TItem>, TItem> HasNoItem(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            var ctx = source.Context;
            ctx.ExpressionBuilder.Append(
                $".{nameof(HasNoItem)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(ctx, obj => obj.HasName(name, comparer), "item");
        }


        /// <summary>
        /// Checks if the collection contains exactly one item with the provided name
        /// and narrows the assertion chain to that item.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<IList<TItem>, TItem> HasSingleItem(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasSingleItem)}({expression ?? "..."})");

            return new(source.Context, (tr) => tr.HasName(name), "item");
        }

        /// <summary>
        /// Checks if the collection contains exactly one item with the provided name
        /// and narrows the assertion chain to that item.
        /// </summary>
        public NarrowCollectionByCriteriaAssertion<IList<TItem>, TItem> HasSingleItem(
            string name,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(name))] string? nameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append(
                $".{nameof(HasSingleItem)}({nameExpression ?? "..."}, {comparerExpression ?? "..."})");

            return new(source.Context, (tr) => tr.HasName(name, comparer), "item");
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
        where TValue : IEnumerable<TItem>
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
        where TCollection : IEnumerable<TItem>
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

        where TCollection : IEnumerable<TItemCollection>
        where TItemCollection : IEnumerable<TItem>
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
        where TCollection : IEnumerable<TItem>
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

        where TCollection : IEnumerable<TItemCollection>
        where TItemCollection : IEnumerable<TItem>
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
        where TCollection : IEnumerable<TItem>
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

        where TCollection : IEnumerable<TItemCollection>
        where TItemCollection : IEnumerable<TItem>
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
