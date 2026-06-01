using System;
using System.Collections.Immutable;
using System.Text.Json;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureTitlePathProperty<TSelf> : IAllureArrayProperty<string, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureTitlePathProperty<TSelf>
{
    static Func<JsonElement, AssertionResult<string>> IAllureArrayProperty<string, TSelf>.Factory { get; }
        = json => json.ValueKind is JsonValueKind.String
            ? AssertionResult<string>.Passed(json.GetString()!)
            : AssertionResult.Failed(
                $"the value was {JsonFunctions.GetJsonKindTypeString(json.ValueKind)} instead of string");
}
