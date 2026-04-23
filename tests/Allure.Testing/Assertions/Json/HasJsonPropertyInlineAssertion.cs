using System;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model.AssertionTargets.Properties;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Json;

public class HasJsonPropertyInlineAssertion<TValue, TProperty, TFinal>(
    AssertionContext<TFinal> context,
    Func<IAssertionSource<TValue>, Assertion<TValue>?> constraints
) :
    Assertion<TFinal>(context)

    where TProperty : IAllureProperty<TValue, TProperty>
    where TFinal : TProperty
{
    Assertion<TValue>? propertyValueAssertion;

    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TFinal> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: var item } =>
                await this.InvokePropertyValueAssertion(
                    JsonFunctions.AssertedGetPropertyValue<TValue, TProperty>(item, TProperty.PropertyName)
                ),
        };

    async Task<AssertionResult> InvokePropertyValueAssertion(TValue actual)
    {
        var (result, assertion)
            = await AssertionFunctions.ExecuteInlineAssertionAsync(actual, TProperty.PropertyName, constraints);
        this.propertyValueAssertion = assertion;
        return result;
    }

    protected override string GetExpectation() =>
        AssertionFunctions.GetAssertionExpectation(this.propertyValueAssertion) is { } expectation
            ? $"to have {TProperty.JsonTypeString} property \"{TProperty.PropertyName}\" {expectation}"
            : $"to have {TProperty.JsonTypeString} property \"{TProperty.PropertyName}\" satisfying the provided constraints";
}