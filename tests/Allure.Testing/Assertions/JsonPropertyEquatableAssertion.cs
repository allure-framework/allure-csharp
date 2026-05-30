using System;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class JsonPropertyEquatableAssertion<TObject, TProperty, TValue, TOther>(
    AssertionContext<TObject> context,
    TOther expectedValue
) :
    Assertion<TObject>(context)

    where TObject : IAllureModelObject<TObject>, TProperty
    where TProperty : IAllureProperty<TValue, TObject>
    where TValue: IEquatable<TOther>
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TObject> metadata
    ) =>
        metadata switch
        {
            { Exception.Message: var message } =>
                await Task.FromResult(AssertionResult.Failed(message)),

            { Value: var item } =>
                TProperty.GetValue(item) switch
                {
                    { IsPassed: true, Value: var value } =>
                        value is not null && value.Equals(expectedValue)
                            ? await Task.FromResult(AssertionResult.Passed)
                            : await Task.FromResult(
                                AssertionResult.Failed($"received {FormatFunctions.FormatAsStringLiteral(value)}")),

                    { Message: var message } =>
                        await Task.FromResult(AssertionResult.Failed(message)),
                },
        };

    protected override string GetExpectation() =>
        $"\"{TProperty.PropertyName}\""
            + $" being equal to {FormatFunctions.FormatAsStringLiteral(expectedValue)}";
}
