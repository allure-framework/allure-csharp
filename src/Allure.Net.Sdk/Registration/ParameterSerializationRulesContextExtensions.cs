using System;
using System.Text.Json;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

public static class ParameterSerializationRulesContextExtensions
{
    extension (IParameterSerializationRulesContext context)
    {
        public void AddRule(IParameterSerializationRule rule) =>
            context.AddRules(rule);

        public void RemoveAllRules() => context.RemoveRules(static (_) => true);

        public void RemoveRules<TRule>()
            where TRule : IParameterSerializationRule
        => context.RemoveRules(static (rule) => rule is TRule);

        public void ReplaceRules<TRule>(
            Func<TRule, IParameterSerializationRule> ruleFactory
        )
            where TRule : IParameterSerializationRule
        =>
            context.ReplaceRules(static (rule) => rule is TRule, (rule) => ruleFactory((TRule)rule));

        public void ReplaceRule(
            Func<IParameterSerializationRule, bool> predicate,
            Func<IParameterSerializationRule, IParameterSerializationRule> ruleFactory
        )
        {
            bool replaced = false;
            context.ReplaceRules((rule) =>
            {
                if (!replaced && predicate(rule))
                {
                    replaced = true;
                    return true;
                }
                return false;
            }, ruleFactory);
        }

        public void ReplaceRule(
            Func<IParameterSerializationRule, bool> predicate,
            IParameterSerializationRule rule
        ) =>
            ReplaceRule(context, predicate, (_) => rule);

        public void ReplaceRule<TRule>(
            Func<TRule, IParameterSerializationRule> ruleFactory
        )
            where TRule : IParameterSerializationRule
        =>
            ReplaceRule(context, static (rule) => rule is TRule, (rule) => ruleFactory((TRule)rule));

        public void ReplaceRule<TRule>(
            IParameterSerializationRule rule
        )
            where TRule : IParameterSerializationRule
        =>
            ReplaceRule<TRule>(context, (_) => rule);

        public void UseJsonOptions(
            Func<JsonSerializerOptions> jsonOptionsFactory
        ) =>
            context.TransformJsonOptions((_) => jsonOptionsFactory());

        public void UseJsonOptions(
            JsonSerializerOptions jsonOptions
        ) =>
            context.TransformJsonOptions((_) => jsonOptions);

        public void AddDelegateRule<TValue>(
            Func<TValue, string> serialize
        ) =>
            context.AddRule(DelegateParameterSerializationRule.Create(serialize));

        public void AddJsonRule(JsonSerializerOptions jsonOptions) =>
            context.AddRule(new JsonParameterSerializationRule(jsonOptions));

        public void AddToStringRule() =>
            context.AddRule(ToStringParameterSerializationRule.Instance);
    }
}
