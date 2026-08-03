using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
using Allure.Sdk.Registration;
using Allure.Sdk.Serialization;

namespace Allure.Net.Sdk.Tests.Serialization;

public class SerializationRulePriorityTests
{
    [Test]
    public async Task ShouldPreferAddedRuleOverDefaultJsonRule()
    {
        var rule = AcceptingRule("custom");
        var serializer = CreateSerializer(context => context.AddRule(rule));

        await Assert.That(serializer.Serialize(17)).IsEqualTo("custom");
        await Assert.That(rule.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldPreferLastAddedMatchingRule()
    {
        var first = AcceptingRule("first");
        var last = AcceptingRule("last");
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(first);
            context.AddRule(last);
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("last");
        await Assert.That(last.CallCount).IsEqualTo(1);
        await Assert.That(first.CallCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldContinueToNextRuleWhenHigherPriorityRuleRejectsValue()
    {
        var lower = AcceptingRule("lower");
        var higher = RejectingRule();
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(lower);
            context.AddRule(higher);
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("lower");
        await Assert.That(higher.CallCount).IsEqualTo(1);
        await Assert.That(lower.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldUseToStringRuleWhenJsonRuleIsRemoved()
    {
        var serializer = CreateSerializer(
            context => context.RemoveRules<JsonParameterSerializationRule>()
        );

        await Assert.That(serializer.Serialize("text")).IsEqualTo("text");
    }

    [Test]
    public async Task ShouldUseFallbackWhenAllRulesAreRemoved()
    {
        var fallbackCalls = 0;
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.UseFallback(value =>
            {
                fallbackCalls++;
                return $"fallback:{value}";
            });
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("fallback:17");
        await Assert.That(fallbackCalls).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldApplyRuleAddedAfterAllExistingRulesAreRemoved()
    {
        var rule = AcceptingRule("added");
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(rule);
            context.UseFallback(_ => "fallback");
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("added");
        await Assert.That(rule.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldReplaceEveryMatchingRule()
    {
        var first = AcceptingRule("first");
        var second = AcceptingRule("second");
        var replacementCount = 0;
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(first);
            context.AddRule(second);
            context.ReplaceRules<RecordingRule>(_ =>
            {
                replacementCount++;
                return AcceptingRule("replacement");
            });
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("replacement");
        await Assert.That(replacementCount).IsEqualTo(2);
        await Assert.That(first.CallCount).IsEqualTo(0);
        await Assert.That(second.CallCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldReplaceOnlyFirstMatchingRule()
    {
        var first = AcceptingRule("first");
        var second = AcceptingRule("second");
        var replacedRules = new List<IParameterSerializationRule>();
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(first);
            context.AddRule(second);
            context.ReplaceRule<RecordingRule>(rule =>
            {
                replacedRules.Add(rule);
                return AcceptingRule("replacement");
            });
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("second");
        await Assert.That(replacedRules.Count).IsEqualTo(1);
        await Assert.That(replacedRules[0]).IsSameReferenceAs(first);
        await Assert.That(first.CallCount).IsEqualTo(0);
        await Assert.That(second.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldRetainReplacedRulePriority()
    {
        var lower = AcceptingRule("lower");
        var target = AcceptingRule("target");
        var replacement = AcceptingRule("replacement");
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(lower);
            context.AddRule(target);
            context.ReplaceRule(
                rule => ReferenceEquals(rule, target),
                replacement
            );
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("replacement");
        await Assert.That(replacement.CallCount).IsEqualTo(1);
        await Assert.That(lower.CallCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldPreferRuleAddedAfterReplacement()
    {
        var target = AcceptingRule("target");
        var replacement = AcceptingRule("replacement");
        var later = AcceptingRule("later");
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(target);
            context.ReplaceRule(
                rule => ReferenceEquals(rule, target),
                replacement
            );
            context.AddRule(later);
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("later");
        await Assert.That(later.CallCount).IsEqualTo(1);
        await Assert.That(replacement.CallCount).IsEqualTo(0);
    }

    static IAllureParameterSerializer CreateSerializer(
        Action<IParameterSerializationRulesContext> configure
    ) =>
        RuntimeTestEnvironment.Create(
            configure: builder => builder.ConfigureSerialization(configure)
        ).Runtime.ParameterSerializer;

    static RecordingRule AcceptingRule(string text) =>
        new((_, [NotNullWhen(true)] out result) =>
        {
            result = text;
            return true;
        });

    static RecordingRule RejectingRule() =>
        new((_, [NotNullWhen(true)] out result) =>
        {
            result = null;
            return false;
        });

    delegate bool TrySerializeDelegate(
        object value,
        [NotNullWhen(true)] out string? text
    );

    sealed class RecordingRule(TrySerializeDelegate trySerialize) :
        IParameterSerializationRule
    {
        public int CallCount { get; private set; }

        public bool TrySerialize(
            object value,
            [NotNullWhen(true)] out string? text
        )
        {
            this.CallCount++;
            return trySerialize(value, out text);
        }
    }
}
