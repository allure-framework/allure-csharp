using System;
using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureObjectArrayProperty<TElement, TSelf> : IAllureArrayProperty<TElement, TSelf>
    where TElement : IAllureModelObject<TElement>
    where TSelf : IAllureModelObject<TSelf>, IAllureObjectArrayProperty<TElement, TSelf>
{
    static Func<JsonElement, AssertionResult<TElement>> IAllureArrayProperty<TElement, TSelf>.Factory { get; }
        = TElement.Create;
}
