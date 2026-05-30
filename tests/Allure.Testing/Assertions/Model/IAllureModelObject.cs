using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model;

public interface IAllureModelObject<TSelf>
    where TSelf: IAllureModelObject<TSelf>
{
    public JsonElement Json { get; }

    public static virtual AssertionResult<TSelf> Create(JsonElement json) =>
        TSelf.Validate(json) is { } error
            ? AssertionResult.Failed(error)
            : AssertionResult<TSelf>.Passed(TSelf.Constructor(json));

    protected static abstract TSelf Constructor(JsonElement json);

    protected static abstract string? Validate(JsonElement json);
}
