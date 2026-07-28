using System;
using System.Text.Json;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

public interface IParameterSerializationRulesContext
{
    void DoNotUseDefaultRules();

    void AddRule(IParameterSerializationRule rule);

    void UseJsonOptions(
        Func<JsonSerializerOptions, JsonSerializerOptions> jsonOptionsFactory
    );

    void UseFallback(Func<object, string> fallback);

    void UseNullRepresentation(string nullRepresentation);
}
