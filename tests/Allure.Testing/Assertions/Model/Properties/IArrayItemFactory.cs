using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IArrayItemFactory<T>
{
    static abstract AssertionResult<T> Create(JsonElement json);
}
