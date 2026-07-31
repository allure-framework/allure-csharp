using System;
using System.Text.Json;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides convenient operations for configuring parameter serialization rules.
/// </summary>
public static class ParameterSerializationRulesContextExtensions
{
    extension (IParameterSerializationRulesContext context)
    {
        /// <summary>
        /// Appends a serialization rule.
        /// </summary>
        public void AddRule(IParameterSerializationRule rule) =>
            context.AddRules(rule);

        /// <summary>
        /// Removes all serialization rules.
        /// </summary>
        public void RemoveAllRules() => context.RemoveRules(static (_) => true);

        /// <summary>
        /// Removes all serialization rules assignable to <typeparamref name="TRule"/>.
        /// </summary>
        public void RemoveRules<TRule>()
            where TRule : IParameterSerializationRule
        => context.RemoveRules(static (rule) => rule is TRule);

        /// <summary>
        /// Replaces every rule assignable to <typeparamref name="TRule"/>.
        /// </summary>
        public void ReplaceRules<TRule>(
            Func<TRule, IParameterSerializationRule> ruleFactory
        )
            where TRule : IParameterSerializationRule
        =>
            context.ReplaceRules(static (rule) => rule is TRule, (rule) => ruleFactory((TRule)rule));

        /// <summary>
        /// Replaces the first rule matching the specified predicate.
        /// </summary>
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

        /// <summary>
        /// Replaces the first matching rule with the specified instance.
        /// </summary>
        public void ReplaceRule(
            Func<IParameterSerializationRule, bool> predicate,
            IParameterSerializationRule rule
        ) =>
            ReplaceRule(context, predicate, (_) => rule);

        /// <summary>
        /// Replaces the first rule assignable to <typeparamref name="TRule"/>
        /// using the specified factory.
        /// </summary>
        public void ReplaceRule<TRule>(
            Func<TRule, IParameterSerializationRule> ruleFactory
        )
            where TRule : IParameterSerializationRule
        =>
            ReplaceRule(context, static (rule) => rule is TRule, (rule) => ruleFactory((TRule)rule));

        /// <summary>
        /// Replaces the first rule assignable to <typeparamref name="TRule"/>
        /// with the specified instance.
        /// </summary>
        public void ReplaceRule<TRule>(
            IParameterSerializationRule rule
        )
            where TRule : IParameterSerializationRule
        =>
            ReplaceRule<TRule>(context, (_) => rule);

        /// <summary>
        /// Replaces JSON serializer options using a factory.
        /// </summary>
        public void UseJsonOptions(
            Func<JsonSerializerOptions> jsonOptionsFactory
        ) =>
            context.TransformJsonOptions((_) => jsonOptionsFactory());

        /// <summary>
        /// Replaces JSON serializer options with the specified instance.
        /// </summary>
        public void UseJsonOptions(
            JsonSerializerOptions jsonOptions
        ) =>
            context.TransformJsonOptions((_) => jsonOptions);

        /// <summary>
        /// Appends a delegate-backed rule for values of type
        /// <typeparamref name="TValue"/>.
        /// </summary>
        public void AddDelegateRule<TValue>(
            Func<TValue, string> serialize
        ) =>
            context.AddRule(DelegateParameterSerializationRule.Create(serialize));

        /// <summary>
        /// Appends a JSON serialization rule using the specified options.
        /// </summary>
        public void AddJsonRule(JsonSerializerOptions jsonOptions) =>
            context.AddRule(new JsonParameterSerializationRule(jsonOptions));

        /// <summary>
        /// Appends the shared <see cref="object.ToString"/> serialization rule.
        /// </summary>
        public void AddToStringRule() =>
            context.AddRule(ToStringParameterSerializationRule.Instance);
    }
}
