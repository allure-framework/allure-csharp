using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model.AssertionTargets.Properties;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Json;

public class HasJsonPropertyEqualsInlineAssertion<TValue, TProperty, TFinal>(
    AssertionContext<TFinal> context,
    TValue expectedValue,
    IEqualityComparer<TValue> comparer
) :
    Assertion<TFinal>(context)

    where TProperty : IAllureProperty<TValue, TProperty>
    where TFinal : TProperty
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TFinal> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: var item } =>
                JsonFunctions.AssertedGetPropertyValue<TValue, TProperty>(
                    item,
                    TProperty.PropertyName
                ) switch
                {
                    var actual =>
                        comparer.Equals(actual, expectedValue)
                            ? await Task.FromResult(AssertionResult.Passed)
                            : await Task.FromResult(
                                AssertionResult.Failed($"received {FormatFunctions.FormatAsStringLiteral(actual)}"))
                },
        };

    protected override string GetExpectation() =>
        $"to have {TProperty.JsonTypeString} property \"{TProperty.PropertyName}\""
            + $" being equal to {FormatFunctions.FormatAsStringLiteral(expectedValue)}";
}
