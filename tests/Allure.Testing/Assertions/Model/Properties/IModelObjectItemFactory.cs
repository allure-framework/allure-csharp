using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IModelObjectItemFactory<T> : IArrayItemFactory<T>
    where T : IAllureModelObject<T>
{
    static AssertionResult<T> IArrayItemFactory<T>.Create(JsonElement json) =>
        T.Create(json);
}