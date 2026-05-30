using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

public interface IAllureArrayProperty<TElement, TSelf> : IAllureProperty<ImmutableArray<TElement>, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureArrayProperty<TElement, TSelf>
{
    static JsonType IAllureProperty<ImmutableArray<TElement>, TSelf>.JsonType { get; } =
        JsonType.Array;

    static AssertionResult<ImmutableArray<TElement>> IAllureProperty<ImmutableArray<TElement>, TSelf>.TryGetPropertyValue(
        JsonElement json) =>
            json
                .EnumerateArray()
                .Select((json, index) => TSelf.Factory(json) switch
                {
                    { IsPassed: true, Value: var value } =>
                        value is not null
                            ? AssertionResult<TElement>.Passed(value)
                            : AssertionResult.Failed($"  - {TSelf.PropertyName}[{index}]: the value was null"),
                    { Message: var error } => AssertionResult.Failed($"  - {TSelf.PropertyName}[{index}]:: {error}"),
                })
                .ToImmutableArray()
            switch
            {
                var array =>
                    array.Where((t) => !t.IsPassed).ToImmutableArray() is { Length: >0 } failures
                        ? AssertionResult.Failed(
                            $"\"{TSelf.PropertyName}\" had invalid elements:{Environment.NewLine}"
                                + string.Join(Environment.NewLine, failures.Select(f => f.Message))
                        )
                        : AssertionResult<ImmutableArray<TElement>>.Passed(
                            [..array.Where(t => t.IsPassed).Select(t => t.Value!)]),
            };

    protected static abstract Func<JsonElement, AssertionResult<TElement>> Factory { get; }
}
