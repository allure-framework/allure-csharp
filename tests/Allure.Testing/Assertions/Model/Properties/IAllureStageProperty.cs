using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureStageProperty<TSelf> : IAllureProperty<AllureStage, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureStageProperty<TSelf>
{
    static JsonType IAllureProperty<AllureStage, TSelf>.JsonType { get; } =
        JsonType.String;

    static AssertionResult<AllureStage> IAllureProperty<AllureStage, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json.GetString() switch
        {
            "scheduled" => AssertionResult<AllureStage>.Passed(AllureStage.Scheduled),
            "running" => AssertionResult<AllureStage>.Passed(AllureStage.Running),
            "finished" => AssertionResult<AllureStage>.Passed(AllureStage.Finished),
            "pending" => AssertionResult<AllureStage>.Passed(AllureStage.Pending),
            "interrupted" => AssertionResult<AllureStage>.Passed(AllureStage.Interrupted),
            var value => AssertionResult.Failed(
                $"had an unknown value {value}. "
                    + "One of \"scheduled\", \"running\", \"finished\", \"pending\", or \"interrupted\" was expected"),
        };
}
