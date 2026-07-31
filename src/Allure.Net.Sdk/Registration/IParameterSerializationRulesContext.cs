using System;
using System.Collections.Generic;
using System.Text.Json;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures the ordered rules used to serialize Allure parameter values.
/// </summary>
public interface IParameterSerializationRulesContext
{
    /// <summary>
    /// Removes every rule that matches the specified predicate.
    /// </summary>
    void RemoveRules(Func<IParameterSerializationRule, bool> predicate);

    /// <summary>
    /// Appends serialization rules in enumeration order.
    /// </summary>
    void AddRules(params IEnumerable<IParameterSerializationRule> rules);

    /// <summary>
    /// Replaces every matching rule using the specified factory.
    /// </summary>
    void ReplaceRules(
        Func<IParameterSerializationRule, bool> predicate,
        Func<IParameterSerializationRule, IParameterSerializationRule> ruleFactory
    );

    /// <summary>
    /// Transforms the serializer options used by JSON serialization rules.
    /// </summary>
    void TransformJsonOptions(
        Func<JsonSerializerOptions, JsonSerializerOptions> jsonOptionsTransformer
    );

    /// <summary>
    /// Configures the fallback used when no serialization rule accepts a value.
    /// </summary>
    void UseFallback(Func<object, string> fallback);

    /// <summary>
    /// Configures the text used to represent <see langword="null"/>.
    /// </summary>
    void UseNullRepresentation(string nullRepresentation);
}
