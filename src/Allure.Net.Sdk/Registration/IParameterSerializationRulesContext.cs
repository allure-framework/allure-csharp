using System;
using System.Collections.Generic;
using System.Text.Json;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

public interface IParameterSerializationRulesContext
{
    void RemoveRules(Func<IParameterSerializationRule, bool> criteria);

    void AddRules(params IEnumerable<IParameterSerializationRule> rules);

    void ReplaceRule(
        Func<IParameterSerializationRule, bool> predicate,
        Func<IParameterSerializationRule, IParameterSerializationRule> ruleFactory
    );

    void TransformJsonOptions(
        Func<JsonSerializerOptions, JsonSerializerOptions> jsonOptionsTransformer
    );

    void UseFallback(Func<object, string> fallback);

    void UseNullRepresentation(string nullRepresentation);
}
