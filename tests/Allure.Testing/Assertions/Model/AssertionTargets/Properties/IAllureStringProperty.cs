using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureStringProperty<TFinal> : IAllureProperty<string, TFinal>
    where TFinal : IAllureProperty<string, TFinal>
{
    static JsonType IAllureProperty<string, TFinal>.JsonType { get; } =
        JsonType.String;

    static string? IAllureProperty<string, TFinal>.GetValue(JsonElement json) =>
        json.GetString();
}
