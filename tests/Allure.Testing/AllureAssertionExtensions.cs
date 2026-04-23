using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Runtime.CompilerServices;
using System.Text.Json;
using Allure.Testing.Assertions;
using Allure.Testing.Assertions.Json;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.AssertionTargets;
using Allure.Testing.Assertions.Model.AssertionTargets.Properties;
using TUnit.Assertions.Core;

namespace Allure.Testing;

public static partial class AllureAssertionExtensions
{
    extension (IAssertionSource<ImmutableArray<AllureTestResult>> source)
    {
        public CollectionGetSingleItemAssertion<ImmutableArray<AllureTestResult>, AllureTestResult, AllureTestResult> SingleTestResultExists()
        {
            source.Context.ExpressionBuilder.Append(".SingleTestResultExists()");
            return new(source.Context, static e => e, static e => e.Validate(), "test result");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult, AllureTestResult> SingleTestResultExists(
            Func<IAssertionSource<AllureTestResult>, Assertion<AllureTestResult>> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".SingleTestResultExists({expression})");
            return new(source.Context, criteria, static e => e, static e => e.Validate(), "test result");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<AllureTestResult>, AllureTestResult, AllureTestResult> SingleTestResultExists(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".SingleTestResultExists({expression})");
            return new(source.Context, tr => tr.HasName(name), static e => e, static e => e.Validate(), "test result");
        }
    }

    extension (IAssertionSource<ImmutableArray<AllureStepResult>> source)
    {
        public CollectionGetSingleItemAssertion<ImmutableArray<AllureStepResult>, AllureStepResult, AllureStepResult> SingleStepExists()
        {
            source.Context.ExpressionBuilder.Append(".SingleStepExists()");
            return new(source.Context, static e => e, static e => null, "step result");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<AllureStepResult>, AllureStepResult, AllureStepResult> SingleStepExists(
            Func<IAssertionSource<AllureStepResult>, Assertion<AllureStepResult>> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".SingleStepExists({expression})");
            return new(source.Context, criteria, static e => e, static e => null, "step result");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<AllureStepResult>, AllureStepResult, AllureStepResult> SingleStepExists(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".SingleStepExists({expression})");
            return new(source.Context, s => s.HasName(name), static e => e, static e => null, "step result");
        }
    }

    extension (IAssertionSource<AllureResults2> source)
    {
        public CollectionGetSingleItemAssertion<ImmutableArray<JsonElement>, JsonElement, AllureTestResult> SingleTestResultExists()
        {
            source.Context.ExpressionBuilder.Append(".SingleTestResultExists()");
            return new(
                source.Context.Map(static async (ar) => ar?.TestResults ?? []),
                static e => new AllureTestResult(e),
                static tr => tr.Validate(),
                "test result");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<JsonElement>, JsonElement, AllureTestResult> SingleTestResultExists(
            Func<IAssertionSource<AllureTestResult>, Assertion<AllureTestResult>> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".SingleTestResultExists({expression})");
            return new(
                source.Context.Map(static async (ar) => ar?.TestResults ?? []),
                criteria,
                static e => new AllureTestResult(e),
                static e => e.Validate(),
                "test result");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<JsonElement>, JsonElement, AllureTestResult> SingleTestResultExists(
            string name,
            [CallerArgumentExpression(nameof(name))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".SingleTestResultExists({expression})");
            return new(
                source.Context.Map(static async (ar) => ar?.TestResults ?? []),
                tr => tr.HasName(name),
                static e => new AllureTestResult(e),
                static e => e.Validate(),
                "test result");
        }
    }

    extension<T> (IAssertionSource<T> source)
        where T : IAllureStepsProperty
    {
        public HasJsonPropertyTransformingAssertion<ImmutableArray<AllureStepResult>, IAllureStepsProperty, T> HasStepsDefined()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasStepsDefined)}()");

            return new(source.Context);
        }

        public HasJsonPropertyInlineAssertion<ImmutableArray<AllureStepResult>, IAllureStepsProperty, T> HasSteps(
            Func<IAssertionSource<AllureStepResult>, Assertion<AllureStepResult>?>[] constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".HasSteps({expression})");

            return new (
                source.Context,
                (steps) =>
                    new CollectionMatchesItemConstraintsAssertion<ImmutableArray<AllureStepResult>, AllureStepResult>(
                        steps.Context,
                        constraints,
                        "step"
                    )
            );
        }

        public CollectionGetSingleItemAssertion<ImmutableArray<AllureStepResult>, AllureStepResult, AllureStepResult> HasSingleStepDefined()
        {
            source.Context.ExpressionBuilder.Append($".HasSingleStepDefined()");

            var getStepsAssertion =
                new HasJsonPropertyTransformingAssertion<ImmutableArray<AllureStepResult>, IAllureStepsProperty, T>(
                    source.Context);

            return new(getStepsAssertion.And.Context, static e => e, static e => null, "step");
        }

        public CollectionFindSingleItemByCriteriaAssertion<ImmutableArray<AllureStepResult>, AllureStepResult, AllureStepResult> HasSingleStep(
            Func<IAssertionSource<AllureStepResult>, Assertion<AllureStepResult>> criteria,
            [CallerArgumentExpression(nameof(criteria))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".HasSingleStep({expression})");

            var getStepsAssertion =
                new HasJsonPropertyTransformingAssertion<ImmutableArray<AllureStepResult>, IAllureStepsProperty, T>(
                    source.Context);
            return new(getStepsAssertion.And.Context, criteria, static e => e, static e => null, "step");
        }
    }

    extension<T> (IAssertionSource<T> source)
        where T : IAllureNameProperty
    {
        public HasJsonPropertyTransformingAssertion<string, IAllureNameProperty, T> HasNameDefined()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasNameDefined)}()");

            return new(source.Context);
        }

        public HasJsonPropertyInlineAssertion<string, IAllureNameProperty, T> HasName(
            Func<IAssertionSource<string>, Assertion<string>?> constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasName)}({expression})");

            return new(source.Context, constraints);
        }

        public HasJsonPropertyEqualsInlineAssertion<string, IAllureNameProperty, T> HasName(
            string expectedName,
            [CallerArgumentExpression(nameof(expectedName))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasName)}({expression})");

            return new(source.Context, expectedName, StringComparer.Ordinal);
        }

        public HasJsonPropertyEqualsInlineAssertion<string, IAllureNameProperty, T> HasName(
            string expectedName,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(expectedName))] string? expectedNameExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append(
                $".{nameof(HasName)}({expectedNameExpression}, {comparerExpression})"
            );

            return new(source.Context, expectedName, comparer);
        }
    }

    extension<T> (IAssertionSource<T> source)
        where T : IAllureMessageProperty
    {
        public HasJsonPropertyTransformingAssertion<string, IAllureMessageProperty, T> HasMessageDefined()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasMessageDefined)}()");

            return new(source.Context);
        }

        public HasJsonPropertyInlineAssertion<string, IAllureMessageProperty, T> HasMessage(
            Func<IAssertionSource<string>, Assertion<string>?> constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasMessage)}({expression})");

            return new(source.Context, constraints);
        }

        public HasJsonPropertyEqualsInlineAssertion<string, IAllureMessageProperty, T> HasMessage(
            string expectedMessage,
            [CallerArgumentExpression(nameof(expectedMessage))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasMessage)}({expression})");

            return new(source.Context, expectedMessage, StringComparer.Ordinal);
        }

        public HasJsonPropertyEqualsInlineAssertion<string, IAllureMessageProperty, T> HasMessage(
            string expectedMessage,
            IEqualityComparer<string> comparer,
            [CallerArgumentExpression(nameof(expectedMessage))] string? expectedMessageExpression = null,
            [CallerArgumentExpression(nameof(comparer))] string? comparerExpression = null
        )
        {
            source.Context.ExpressionBuilder.Append(
                $".{nameof(HasMessage)}({expectedMessageExpression}, {comparerExpression})"
            );

            return new(source.Context, expectedMessage, comparer);
        }
    }

    extension<T> (IAssertionSource<T> source)
        where T : IAllureStatusDetailsProperty
    {
        public HasJsonPropertyTransformingAssertion<AllureStatusDetails, IAllureStatusDetailsProperty, T> HasStatusDetailsDefined()
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasStatusDetailsDefined)}()");

            return new(source.Context);
        }

        public HasJsonPropertyInlineAssertion<AllureStatusDetails, IAllureStatusDetailsProperty, T> HasStatusDetails(
            Func<IAssertionSource<AllureStatusDetails>, Assertion<AllureStatusDetails>?> constraints,
            [CallerArgumentExpression(nameof(constraints))] string? expression = null
        )
        {
            source.Context.ExpressionBuilder.Append($".{nameof(HasStatusDetails)}({expression})");

            return new(source.Context, constraints);
        }
    }
}
