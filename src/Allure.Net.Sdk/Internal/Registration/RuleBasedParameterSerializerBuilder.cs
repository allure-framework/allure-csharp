using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.Json;
using Allure.Sdk.Registration;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Internal.Registration;

sealed class RuleBasedParameterSerializerBuilder : IParameterSerializationRulesContext
{
    readonly List<RuleAction> actions = [];

    Func<object, string> currentFallback = static (o) => o.ToString();

    string currentNullRepresentation = "null";

    List<Func<JsonSerializerOptions, JsonSerializerOptions>> currentJsonOptionTransformers = [];

    public RuleBasedParameterSerializerBuilder()
    {
        this.actions = [
            new AddRuleAction(ToStringParameterSerializationRule.Instance),
            new AddJsonRuleAction(ResolveJsonOptions),
        ];
    }

    public void AddRules(IEnumerable<IParameterSerializationRule> rules)
    {
        this.actions.AddRange(rules.Select(static (rule) => new AddRuleAction(rule)));
    }

    public void RemoveRules(Func<IParameterSerializationRule, bool> criteria)
    {
        this.actions.Add(new RemoveRulesAction(criteria));
    }

    public void ReplaceRule(
        Func<IParameterSerializationRule, bool> predicate,
        Func<IParameterSerializationRule, IParameterSerializationRule> ruleFactory
    )
    {
        this.actions.Add(new ReplaceRuleAction(predicate, ruleFactory));
    }

    public void TransformJsonOptions(
        Func<JsonSerializerOptions, JsonSerializerOptions> jsonOptionsTransformer
    )
    {
        this.currentJsonOptionTransformers.Add(jsonOptionsTransformer);
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
        List<IParameterSerializationRule> rules = [];
        foreach (var action in this.actions)
        {
            action.ApplyTo(rules);
        }
        rules.Reverse();

        return new([.. rules], this.currentFallback, this.currentNullRepresentation);
    }

    JsonSerializerOptions ResolveJsonOptions()
    {
        var options = JsonParameterSerializationRule.CreateDefaultJsonOptions();
        foreach (var transform in this.currentJsonOptionTransformers)
        {
            options = transform(options);
        }
        return options;
    }

    abstract record class RuleAction
    {
        internal abstract void ApplyTo(List<IParameterSerializationRule> rules);
    }

    record class AddRuleAction(IParameterSerializationRule Rule) : RuleAction
    {
        internal override void ApplyTo(List<IParameterSerializationRule> rules)
        {
            rules.Add(this.Rule);
        }
    }

    record class AddJsonRuleAction(Func<JsonSerializerOptions> OptionsFactory) : RuleAction
    {
        internal override void ApplyTo(List<IParameterSerializationRule> rules)
        {
            rules.Add(new JsonParameterSerializationRule(this.OptionsFactory()));
        }
    }

    record class ReplaceRuleAction(
        Func<IParameterSerializationRule, bool> Predicate,
        Func<IParameterSerializationRule, IParameterSerializationRule> Factory
    ) : RuleAction
    {
        internal override void ApplyTo(List<IParameterSerializationRule> rules)
        {
            foreach (var index in Enumerable.Range(0, rules.Count))
            {
                var rule = rules[index];
                if (this.Predicate(rule))
                {
                    rules[index] = this.Factory(rule);
                }
            }
        }
    }

    record class RemoveRulesAction(
        Func<IParameterSerializationRule, bool> Criteria
    ) : RuleAction
    {
        internal override void ApplyTo(List<IParameterSerializationRule> rules)
        {
            rules.RemoveAll((rule) => this.Criteria(rule));
        }
    }
}
