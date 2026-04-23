using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets;

public interface IAllureJsonObject
{
    public JsonElement Json { get; }
}
