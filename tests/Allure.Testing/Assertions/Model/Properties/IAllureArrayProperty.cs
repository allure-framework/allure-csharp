using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureArrayProperty<TElement, TFactory, TSelf> : IAllureProperty<ImmutableArray<TElement>, TSelf>
    where TFactory : IArrayItemFactory<TElement>
    where TSelf : IAllureModelObject<TSelf>, IAllureArrayProperty<TElement, TFactory, TSelf>
{
    static JsonType IAllureProperty<ImmutableArray<TElement>, TSelf>.JsonType { get; } =
        JsonType.Array;

    static AssertionResult<ImmutableArray<TElement>> IAllureProperty<ImmutableArray<TElement>, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json
            .EnumerateArray()
            .Select((json, index) => TFactory.Create(json) switch
            {
                { IsPassed: true, Value: var value } =>
                    value is not null
                        ? AssertionResult<TElement>.Passed(value)
                        : AssertionResult.Failed($"  - [{index}]: the value was null"),
                { Message: var error } => AssertionResult.Failed($"  - [{index}]: {error}"),
            })
            .ToImmutableArray()
        switch
        {
            var array =>
                array.Where((t) => !t.IsPassed).ToImmutableArray() is { Length: >0 } failures
                    ? AssertionResult.Failed(
                        $"had invalid elements:{Environment.NewLine}"
                            + string.Join(Environment.NewLine, failures.Select(f => f.Message))
                    )
                    : AssertionResult<ImmutableArray<TElement>>.Passed(
                        [..array.Where(t => t.IsPassed).Select(t => t.Value!)]),
        };
}
