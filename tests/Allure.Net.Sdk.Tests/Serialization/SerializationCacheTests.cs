using System.Collections.Concurrent;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
using Allure.Sdk.Registration;
using Allure.Sdk.Serialization;

namespace Allure.Net.Sdk.Tests.Serialization;

public class SerializationCacheTests
{
    [Test]
    public async Task ShouldTryCachedRuleFirstForSubsequentValuesOfSameType()
    {
        var lower = Rule((_, out text) =>
        {
            text = "accepted";
            return true;
        });
        var higher = RejectingRule();
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(lower);
            context.AddRule(higher);
        });

        await Assert.That(serializer.Serialize(1)).IsEqualTo("accepted");
        await Assert.That(serializer.Serialize(2)).IsEqualTo("accepted");
        await Assert.That(higher.CallCount).IsEqualTo(1);
        await Assert.That(lower.CallCount).IsEqualTo(2);
    }

    [Test]
    public async Task ShouldCacheRulesByExactRuntimeType()
    {
        var accepting = Rule((_, out text) =>
        {
            text = "accepted";
            return true;
        });
        var higher = RejectingRule();
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(accepting);
            context.AddRule(higher);
        });

        await Assert.That(serializer.Serialize(new BaseValue()))
            .IsEqualTo("accepted");
        await Assert.That(serializer.Serialize(new DerivedValue()))
            .IsEqualTo("accepted");
        await Assert.That(serializer.Serialize(new BaseValue()))
            .IsEqualTo("accepted");
        await Assert.That(serializer.Serialize(new DerivedValue()))
            .IsEqualTo("accepted");

        await Assert.That(higher.CallCount).IsEqualTo(2);
        await Assert.That(accepting.CallCount).IsEqualTo(4);
    }

    [Test]
    public async Task ShouldSearchFullChainWhenCachedRuleRejectsLaterValue()
    {
        var lower = Rule((_, out text) =>
        {
            text = "lower";
            return true;
        });
        var higher = Rule((value, out text) =>
        {
            if ((int)value > 0)
            {
                text = "higher";
                return true;
            }

            text = null;
            return false;
        });
        var serializer = CreateSerializer(context =>
        {
            context.AddRule(lower);
            context.AddRule(higher);
        });

        await Assert.That(serializer.Serialize(1)).IsEqualTo("higher");
        await Assert.That(serializer.Serialize(-1)).IsEqualTo("lower");

        await Assert.That(higher.CallCount).IsEqualTo(3);
        await Assert.That(lower.CallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldReplaceCachedRuleWhenAnotherRuleLaterSucceeds()
    {
        var first = Rule((value, out text) =>
        {
            if ((int)value == 1)
            {
                text = "first";
                return true;
            }

            text = null;
            return false;
        });
        var second = Rule((value, out text) =>
        {
            if ((int)value == 2)
            {
                text = "second";
                return true;
            }

            text = null;
            return false;
        });
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(second);
            context.AddRule(first);
        });

        await Assert.That(serializer.Serialize(1)).IsEqualTo("first");
        await Assert.That(serializer.Serialize(2)).IsEqualTo("second");
        await Assert.That(serializer.Serialize(1)).IsEqualTo("first");

        await Assert.That(first.CallCount).IsEqualTo(4);
        await Assert.That(second.CallCount).IsEqualTo(2);
    }

    [Test]
    public async Task ShouldSkipRulesAfterRuntimeTypeIsRoutedToFallback()
    {
        var rejecting = RejectingRule();
        var fallbackCalls = 0;
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(rejecting);
            context.UseFallback(value =>
            {
                fallbackCalls++;
                return $"fallback:{value}";
            });
        });

        await Assert.That(serializer.Serialize("first"))
            .IsEqualTo("fallback:first");
        await Assert.That(serializer.Serialize("second"))
            .IsEqualTo("fallback:second");

        await Assert.That(rejecting.CallCount).IsEqualTo(1);
        await Assert.That(fallbackCalls).IsEqualTo(2);
    }

    [Test]
    public async Task ShouldMaintainIndependentCachedRulesForDifferentTypes()
    {
        var strings = Rule((value, out text) =>
        {
            if (value is string stringValue)
            {
                text = $"string:{stringValue}";
                return true;
            }

            text = null;
            return false;
        });
        var integers = Rule((value, out text) =>
        {
            if (value is int intValue)
            {
                text = $"int:{intValue}";
                return true;
            }

            text = null;
            return false;
        });
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(strings);
            context.AddRule(integers);
        });

        await Assert.That(serializer.Serialize("a")).IsEqualTo("string:a");
        await Assert.That(serializer.Serialize(1)).IsEqualTo("int:1");
        await Assert.That(serializer.Serialize("b")).IsEqualTo("string:b");
        await Assert.That(serializer.Serialize(2)).IsEqualTo("int:2");

        await Assert.That(strings.CallCount).IsEqualTo(2);
        await Assert.That(integers.CallCount).IsEqualTo(3);
    }

    [Test]
    public async Task ShouldSerializeConcurrentlyWithoutMixingTypeCaches()
    {
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddDelegateRule<string>(value => $"string:{value}");
            context.AddDelegateRule<int>(value => $"int:{value}");
        });
        var results = new ConcurrentBag<string>();

        await Parallel.ForEachAsync(
            Enumerable.Range(0, 1_000),
            async (value, _) =>
            {
                results.Add(serializer.Serialize(value));
                results.Add(serializer.Serialize(value.ToString()));
                await Task.Yield();
            }
        );

        await Assert.That(results.Count).IsEqualTo(2_000);
        await Assert.That(results.Count(
            result => result.StartsWith("int:", StringComparison.Ordinal)
        )).IsEqualTo(1_000);
        await Assert.That(results.Count(
            result => result.StartsWith("string:", StringComparison.Ordinal)
        )).IsEqualTo(1_000);
    }

    static IAllureParameterSerializer CreateSerializer(
        Action<IParameterSerializationRulesContext> configure
    ) =>
        RuntimeTestEnvironment.Create(
            configure: builder => builder.ConfigureSerialization(configure)
        ).Runtime.ParameterSerializer;

    static RecordingRule Rule(TrySerializeDelegate trySerialize) =>
        new(trySerialize);

    static RecordingRule RejectingRule() =>
        Rule((object _, out string? text) =>
        {
            text = null;
            return false;
        });

    delegate bool TrySerializeDelegate(
        object value,
        out string? text
    );

    sealed class RecordingRule(TrySerializeDelegate trySerialize) :
        IParameterSerializationRule
    {
        int callCount;

        public int CallCount => this.callCount;

        public bool TrySerialize(
            object value,
            [NotNullWhen(true)] out string? text
        )
        {
            Interlocked.Increment(ref this.callCount);
            return trySerialize(value, out text);
        }
    }

    class BaseValue;

    sealed class DerivedValue : BaseValue;
}
