using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureObjectProperty<TObject, TSelf> : IAllureProperty<TObject, TSelf>
    where TObject : IAllureModelObject<TObject>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectProperty<TObject, TSelf>
{
    static JsonType IAllureProperty<TObject, TSelf>.JsonType { get; } =
        JsonType.Object;

    static AssertionResult<TObject> IAllureProperty<TObject, TSelf>.TryGetPropertyValue(JsonElement json) =>
        TObject.Create(json);
}
