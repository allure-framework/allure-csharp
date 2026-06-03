using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureLongProperty<TSelf> : IAllureProperty<long, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureLongProperty<TSelf>
{
    static JsonType IAllureProperty<long, TSelf>.JsonType { get; } =
        JsonType.Number;

    static AssertionResult<long> IAllureProperty<long, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json.TryGetInt64(out var value)
            ? AssertionResult<long>.Passed(value)
            : AssertionResult.Failed("was not a valid integer");
}
