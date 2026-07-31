using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using System.Text.Json.Serialization;
using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
using Allure.Sdk.Registration;
using Allure.Sdk.Serialization;

namespace Allure.Net.Sdk.Tests.Serialization;

public class SerializationFailureTests
{
    [Test]
    public async Task ShouldPropagateExceptionFromCustomRule()
    {
        var expected = new InvalidOperationException("rule failed");
        var serializer = CreateSerializer(context =>
            context.AddRule(new DelegateRule((_, out text) =>
            {
                text = null;
                throw expected;
            }))
        );

        await Assert.That(() => serializer.Serialize(17))
            .Throws<InvalidOperationException>()
            .WithMessage("rule failed");
    }

    [Test]
    public async Task ShouldUseToStringWhenJsonConverterThrows()
    {
        var serializer = CreateSerializer(context =>
            context.UseJsonOptions(new JsonSerializerOptions
            {
                Converters = { new ThrowingJsonConverter() },
            })
        );

        await Assert.That(serializer.Serialize(new ValueWithToString()))
            .IsEqualTo("fallback text");
    }

    [Test]
    public async Task ShouldPropagateExceptionFromFallback()
    {
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.UseFallback(_ =>
                throw new InvalidOperationException("fallback failed")
            );
        });

        await Assert.That(() => serializer.Serialize(17))
            .Throws<InvalidOperationException>()
            .WithMessage("fallback failed");
    }

    [Test]
    public async Task ShouldSerializeNullWithoutCallingRulesOrFallback()
    {
        var ruleCalls = 0;
        var fallbackCalls = 0;
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(new DelegateRule((_, out text) =>
            {
                ruleCalls++;
                text = null;
                throw new InvalidOperationException("rule called");
            }));
            context.UseFallback(_ =>
            {
                fallbackCalls++;
                throw new InvalidOperationException("fallback called");
            });
            context.UseNullRepresentation("<null>");
        });

        await Assert.That(serializer.Serialize(null)).IsEqualTo("<null>");
        await Assert.That(ruleCalls).IsEqualTo(0);
        await Assert.That(fallbackCalls).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldUseNullRepresentationWhenSuccessfulRuleProducesNull()
    {
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(new DelegateRule((_, out text) =>
            {
                text = null;
                return true;
            }));
            context.UseNullRepresentation("<null>");
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("<null>");
        await Assert.That(serializer.Serialize(18)).IsEqualTo("<null>");
    }

    [Test]
    public async Task ShouldIgnoreTextFromRuleThatRejectsValue()
    {
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.AddRule(new DelegateRule((_, out text) =>
            {
                text = "invalid";
                return false;
            }));
            context.UseFallback(_ => "fallback");
        });

        await Assert.That(serializer.Serialize(17)).IsEqualTo("fallback");
    }

    [Test]
    public async Task ShouldUseNullRepresentationWhenToStringRuleProducesNull()
    {
        var serializer = CreateSerializer(context =>
        {
            context.RemoveRules<JsonParameterSerializationRule>();
            context.UseNullRepresentation("<null>");
        });

        await Assert.That(serializer.Serialize(new NullToStringValue()))
            .IsEqualTo("<null>");
    }

    static IAllureParameterSerializer CreateSerializer(
        Action<IParameterSerializationRulesContext> configure
    ) =>
        RuntimeTestEnvironment.Create(
            configure: builder => builder.ConfigureSerialization(configure)
        ).Runtime.ParameterSerializer;

    delegate bool TrySerializeDelegate(object value, out string? text);

    sealed class DelegateRule(TrySerializeDelegate trySerialize) :
        IParameterSerializationRule
    {
        public bool TrySerialize(
            object value,
            [NotNullWhen(true)] out string? text
        ) =>
            trySerialize(value, out text);
    }

    sealed class ThrowingJsonConverter : JsonConverter<ValueWithToString>
    {
        public override ValueWithToString Read(
            ref Utf8JsonReader reader,
            Type typeToConvert,
            JsonSerializerOptions options
        ) =>
            throw new NotSupportedException();

        public override void Write(
            Utf8JsonWriter writer,
            ValueWithToString value,
            JsonSerializerOptions options
        ) =>
            throw new InvalidOperationException("JSON failed");
    }

    sealed class ValueWithToString
    {
        public override string ToString() => "fallback text";
    }

    sealed class NullToStringValue
    {
        public override string? ToString() => null;
    }
}
