using System;
using System.Text.Json;
using TUnit.Assertions.Core;

namespace Allure.Testing.Assertions.Model.Properties;

[GenerateAllureAssertions]
public interface IAllureUuidProperty<TSelf> : IAllureProperty<Guid, TSelf>
    where TSelf : IAllureModelObject<TSelf>, IAllureUuidProperty<TSelf>
{
    static JsonType IAllureProperty<Guid, TSelf>.JsonType { get; } =
        JsonType.String;

    static AssertionResult<Guid> IAllureProperty<Guid, TSelf>.TryConvertToPropertyValue(
        JsonElement json
    ) =>
        json.GetString()! switch
        {
            var text =>
                Guid.TryParse(text, out var uuid)
                    ? AssertionResult<Guid>.Passed(uuid)
                    : AssertionResult.Failed($"was not a valid uuid: \"{text}\""),
        };
}
