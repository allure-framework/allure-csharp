using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;

namespace Allure.Testing.Assertions.Model.AssertionTargets.Properties;

public interface IAllureArrayProperty<TElement, TFinal> : IAllureProperty<ImmutableArray<TElement?>, TFinal>
    where TFinal : IAllureArrayProperty<TElement, TFinal>
{
    static JsonType IAllureProperty<ImmutableArray<TElement?>, TFinal>.JsonType { get; } =
        JsonType.Array;

    static ImmutableArray<TElement?> IAllureProperty<ImmutableArray<TElement?>, TFinal>.GetValue(JsonElement json)
        => [..json.EnumerateArray().Select(TFinal.Factory)];

    static abstract Func<JsonElement, TElement> Factory { get; }
}
