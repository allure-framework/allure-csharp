using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureStringProperty<TSelf> : IAllureProperty<string, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureProperty<string, TSelf>
{
    static JsonType IAllureProperty<string, TSelf>.JsonType { get; } =
        JsonType.String;

    static AssertionResult<string> IAllureProperty<string, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        AssertionResult<string>.Passed(json.GetString()!);
}
