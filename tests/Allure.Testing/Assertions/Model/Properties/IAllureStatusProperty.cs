using System.Collections.Immutable;
using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureStatusProperty<TSelf> : IAllureProperty<AllureStatus, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureStatusProperty<TSelf>
{
    static JsonType IAllureProperty<AllureStatus, TSelf>.JsonType { get; } =
        JsonType.String;

    static AssertionResult<AllureStatus> IAllureProperty<AllureStatus, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json.GetString() switch
        {
            "passed" => AssertionResult<AllureStatus>.Passed(AllureStatus.Passed),
            "failed" => AssertionResult<AllureStatus>.Passed(AllureStatus.Failed),
            "broken" => AssertionResult<AllureStatus>.Passed(AllureStatus.Broken),
            "skipped" => AssertionResult<AllureStatus>.Passed(AllureStatus.Skipped),
            "unknown" => AssertionResult<AllureStatus>.Passed(AllureStatus.Unknown),
            var value => AssertionResult.Failed(
                $"had an unknown value {value}. "
                    + "One of \"passed\", \"failed\", \"broken\", \"skipped\", or \"unknown\" was expected"),
        };
}
