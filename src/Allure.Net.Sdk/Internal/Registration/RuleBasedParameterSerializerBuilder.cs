using System;
using System.Collections.Generic;
using System.Text.Json;
using Allure.Sdk.Registration;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Internal.Registration;

sealed class RuleBasedParameterSerializerBuilder : IParameterSerializerRegistrationContext
{
    readonly List<IParameterSerializationRule> rules = [];

    bool addDefaultRules = true;

    Func<object, string> currentFallback = static (o) => o.ToString();

    string currentNullRepresentation = "null";

    Func<JsonSerializerOptions, JsonSerializerOptions>? currentOptionsFactory = null;

    public void AddRule(IParameterSerializationRule rule)
    {
        this.rules.Add(rule);
    }

    public void UseJsonOptions(
        Func<JsonSerializerOptions, JsonSerializerOptions> jsonOptionsFactory
    )
    {
        this.currentOptionsFactory = jsonOptionsFactory;
    }

    public void DoNotUseDefaultRules()
    {
        this.addDefaultRules = false;
    }

    public void UseFallback(Func<object, string> fallback)
    {
        this.currentFallback = fallback;
    }

    public void UseNullRepresentation(string nullRepresentation)
    {
        this.currentNullRepresentation = nullRepresentation;
    }

    internal RuleBasedParameterSerializer Build()
    {
        IEnumerable<IParameterSerializationRule> resolvedRules =
            this.addDefaultRules
                ? [..this.rules, this.CreateJsonRule(), new ToStringParameterSerializationRule()]
                : this.rules;
        return new([.. resolvedRules], this.currentFallback, this.currentNullRepresentation);
    }

    JsonParameterSerializationRule CreateJsonRule()
    {
        var defaultOptions = JsonParameterSerializationRule.CreateDefaultJsonOptions();
        var finalOptions = this.currentOptionsFactory?.Invoke(defaultOptions) ?? defaultOptions;
        return new(finalOptions);
    }
}
