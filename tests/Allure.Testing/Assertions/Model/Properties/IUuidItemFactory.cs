using System;
using System.Text.Json;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IUuidItemFactory : IArrayItemFactory<Guid>
{
    static AssertionResult<Guid> IArrayItemFactory<Guid>.Create(JsonElement json) =>
        json.ValueKind is JsonValueKind.String
            ? json.GetString()! switch
            {
                var value =>
                    Guid.TryParse(json.GetString()!, out var uuid)
                        ? AssertionResult<Guid>.Passed(uuid)
                        : AssertionResult.Failed($"was \"{value}\", which is not a valid UUID")
            }
            : AssertionResult.Failed(
                $"was a JSON {JsonFunctions.GetJsonKindTypeString(json.ValueKind)}. Expected string");
}
