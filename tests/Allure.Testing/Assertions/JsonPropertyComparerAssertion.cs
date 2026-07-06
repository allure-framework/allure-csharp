using System.Collections.Generic;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class JsonPropertyComparerAssertion<TObject, TProperty, TValue>(
    string propertyName,
    AssertionContext<TObject> context,
    TValue expectedValue,
    IEqualityComparer<TValue> comparer
) :
    Assertion<TObject>(context)

    where TObject : IAllureModelObject<TObject>, TProperty
    where TProperty : IAllureProperty<TValue, TObject>
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TObject> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: var item } =>
                TProperty.GetValue(item, propertyName) switch
                {
                    { IsPassed: true, Value: var value } =>
                        comparer.Equals(value, expectedValue)
                            ? await Task.FromResult(AssertionResult.Passed)
                            : await Task.FromResult(
                                AssertionResult.Failed($"received {propertyName} {FormatFunctions.FormatAsStringLiteral(value)}")),

                    { Message: var message } =>
                        await Task.FromResult(AssertionResult.Failed(message)),
                },
        };

    protected override string GetExpectation() =>
        $"\"{propertyName}\""
            + $" being equal to {FormatFunctions.FormatAsStringLiteral(expectedValue)}";
}
