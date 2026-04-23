using System;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model.AssertionTargets.Properties;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Json;

public class HasJsonPropertyTransformingAssertion<TValue, TProperty, TFinal>(
    AssertionContext<TFinal> context
) :
    Assertion<TValue>(context.Map(Mapper))

    where TProperty : IAllureProperty<TValue, TProperty>
    where TFinal : TProperty
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TValue> metadata
    ) =>
        metadata is { Exception.Message: var message }
            ? await Task.FromResult(AssertionResult.Failed(message))
            : await Task.FromResult(AssertionResult.Passed);

    protected override string GetExpectation() =>
        $"to have {TProperty.JsonTypeString} property \"{TProperty.PropertyName}\" defined";

    public static Func<TFinal?, TValue?> Mapper { get;} =
        item =>
            JsonFunctions.AssertedGetPropertyValue<TValue, TProperty>(
                item,
                TProperty.PropertyName);
}