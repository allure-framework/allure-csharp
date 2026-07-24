using System;
using System.Collections.Concurrent;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Sdk.Serialization;

namespace Allure.Sdk.Internal;

sealed class RuleBasedParameterSerializer(
    ImmutableArray<IParameterSerializationRule> rules,
    Func<object, string> fallback,
    string nullRepresentation
) :
    IAllureParameterSerializer
{
    readonly ConcurrentDictionary<Type, bool> fallbackedTypes = [];
    readonly ConcurrentDictionary<Type, IParameterSerializationRule> matchedRulesCache = [];

    public ImmutableArray<IParameterSerializationRule> Rules => rules;

    public Func<object, string> FallbackAlgorithm => fallback;

    public string NullRepresentation => nullRepresentation;

    public string Serialize(object? value)
    {
        if (value is null)
        {
            return nullRepresentation;
        }

        var type = value.GetType();
        if (this.IsMatchedToFallback(type))
        {
            return fallback(value);
        }

        if (this.TrySerializeFromCache(value, out var text))
        {
            return text;
        }

        foreach (var rule in rules)
        {
            if (rule.TrySerialize(value, out text))
            {
                this.matchedRulesCache[type] = rule;
                return text;
            }
        }

        this.fallbackedTypes[type] = true;
        return fallback(value);
    }

    public bool TryGetCachedRule(Type type, [NotNullWhen(true)] out IParameterSerializationRule rule) =>
        this.matchedRulesCache.TryGetValue(type, out rule);

    public bool IsMatchedToFallback(Type type) => this.fallbackedTypes.ContainsKey(type);

    bool TrySerializeFromCache(object value, [MaybeNullWhen(false)] out string text)
    {
        if (this.TryGetCachedRule(value.GetType(), out var cachedRule)
            && cachedRule.TrySerialize(value, out text))
        {
            return true;
        }

        text = null;
        return false;
    }
}
