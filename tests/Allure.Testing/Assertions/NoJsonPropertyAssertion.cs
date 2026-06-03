using System.Text.Json;
using System.Threading.Tasks;
using Allure.Testing.Assertions.Model;
using Allure.Testing.Internal;
using TUnit.Assertions.Conditions.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions;

public class NoJsonPropertyAssertion<TObject>(
    string propertyName,
    AssertionContext<TObject> context
) :
    Assertion<TObject>(context)

    where TObject : IAllureModelObject<TObject>
{
    protected override async Task<AssertionResult> CheckAsync(
        EvaluationMetadata<TObject> metadata
    ) =>
        await Task.FromResult(
            metadata switch
            {
                { Exception.Message: var message } =>
                    AssertionResult.Failed(message),

                { Value.Json: { ValueKind: var kind } json } =>
                    kind == JsonValueKind.Object
                        ? json.HasProperty(propertyName)
                            ? AssertionResult.Failed($"\"{propertyName}\" existed in the object")
                            : AssertionResult.Passed
                        : AssertionResult.Failed(
                            $"the object was a JSON {JsonFunctions.GetJsonKindTypeString(kind)}. Expected an object"),

                _ => AssertionResult.Failed("the object was null")
            }
        );

    protected override string GetExpectation() => $"\"{propertyName}\" being missing";
}
