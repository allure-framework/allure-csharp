using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureBoolProperty<TSelf> : IAllureProperty<bool, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureBoolProperty<TSelf>
{
    static JsonType IAllureProperty<bool, TSelf>.JsonType { get; } =
        JsonType.Boolean;

    static AssertionResult<bool> IAllureProperty<bool, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json.ValueKind switch
        {
            JsonValueKind.True => AssertionResult<bool>.Passed(true),
            JsonValueKind.False => AssertionResult<bool>.Passed(false),
            _ => AssertionResult.Failed("was not a boolean"),
        };
}
