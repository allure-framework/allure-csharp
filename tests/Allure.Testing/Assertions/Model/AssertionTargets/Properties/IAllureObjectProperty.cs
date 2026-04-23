using System;
using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureObjectProperty<TObject, TFinal> : IAllureProperty<TObject, TFinal>
    where TObject : IAllureJsonObject
    where TFinal : IAllureObjectProperty<TObject, TFinal>
{
    static JsonType IAllureProperty<TObject, TFinal>.JsonType { get; } =
        JsonType.Object;

    static TObject IAllureProperty<TObject, TFinal>.GetValue(JsonElement json) =>
        TFinal.Factory(json);

    static abstract Func<JsonElement, TObject> Factory { get; }
}
