using System.Text.Json;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IStringItemFactory : IArrayItemFactory<string>
{
    static AssertionResult<string> IArrayItemFactory<string>.Create(JsonElement json) =>
        json.ValueKind is JsonValueKind.String
            ? AssertionResult<string>.Passed(json.GetString()!)
            : AssertionResult.Failed(
                $"the value was {JsonFunctions.GetJsonKindTypeString(json.ValueKind)} instead of string");
}
