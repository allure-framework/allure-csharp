using System;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Assertions.Model.Properties;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class JsonPropertyCriteriaAssertion<TObject, TProperty, TValue>(
    AssertionContext<TObject> context,
    Func<IAssertionSource<TValue>, IAssertion> constraints
) :
    Assertion<TObject>(context)

    where TObject : IAllureModelObject<TObject>, TProperty
    where TProperty : IAllureProperty<TValue, TObject>
{
    string? expected;

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
                        value is not null
                            ? await this.InvokePropertyValueAssertion(value)
                            : await Task.FromResult(AssertionResult.Failed("the value was null")),

                    { Message: var message } =>
                        await Task.FromResult(AssertionResult.Failed(message)),
                },
        };

    async Task<AssertionResult> InvokePropertyValueAssertion(TValue actual)
    {
        var (result, _)
            = await AssertionFunctions.ExecuteInlineAssertionAsync(actual, TProperty.PropertyName, constraints);

        if (!result.IsPassed)
        {
            var (expected, _) = NarrowingFunctions.ExtractExpectedAndActual(result.Message, 0);
            this.expected = expected;
        }

        return result;
    }

    protected override string GetExpectation() =>
        this.expected is { } expectation
            ? $"\"{TProperty.PropertyName}\" {expectation}"
            : $"\"{TProperty.PropertyName}\" satisfying the provided constraints";
}