using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions(PropertyName = "Mode")]
public interface IAllureParameterModeProperty<TSelf> : IAllureProperty<AllureParameterMode, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureParameterModeProperty<TSelf>
{
    static JsonType IAllureProperty<AllureParameterMode, TSelf>.JsonType { get; } =
        JsonType.String;

    static AssertionResult<AllureParameterMode> IAllureProperty<AllureParameterMode, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json.GetString() switch
        {
            "default" => AssertionResult<AllureParameterMode>.Passed(AllureParameterMode.Default),
            "masked" => AssertionResult<AllureParameterMode>.Passed(AllureParameterMode.Masked),
            "hidden" => AssertionResult<AllureParameterMode>.Passed(AllureParameterMode.Hidden),
            var value => AssertionResult.Failed(
                $"had an unknown value {value}. "
                    + "One of \"default\", \"masked\", or \"hidden\" was expected"),
        };
}
