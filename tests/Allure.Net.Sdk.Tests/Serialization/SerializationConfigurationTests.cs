using System.Text.Json;
using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;

namespace Allure.Net.Sdk.Tests.Serialization;

public class SerializationConfigurationTests
{
    [Test]
    public async Task ShouldUseConfiguredNullRepresentation()
    {
        var serializer = CreateSerializer(
            context => context.UseNullRepresentation("<null>")
        );

        await Assert.That(serializer.Serialize(null)).IsEqualTo("<null>");
    }

    [Test]
    public async Task ShouldUseConfiguredFallback()
    {
        var serializer = CreateSerializer(context =>
        {
            context.RemoveAllRules();
            context.UseFallback(value => $"fallback:{value.GetType().Name}");
        });

        await Assert.That(serializer.Serialize(new PlainObject()))
            .IsEqualTo("fallback:PlainObject");
    }

    [Test]
    public async Task ShouldApplyDelegateRuleToCompatibleValues()
    {
        var serializer = CreateSerializer(
            context => context.AddDelegateRule<BaseValue>(
                value => $"custom:{value.Name}"
            )
        );

        await Assert.That(serializer.Serialize(new BaseValue("base")))
            .IsEqualTo("custom:base");
        await Assert.That(serializer.Serialize(new DerivedValue("derived")))
            .IsEqualTo("custom:derived");
    }

    [Test]
    public async Task ShouldApplyDelegateRuleToInterfaceImplementations()
    {
        var serializer = CreateSerializer(
            context => context.AddDelegateRule<INamedValue>(
                value => $"interface:{value.Name}"
            )
        );

        await Assert.That(serializer.Serialize(new NamedValue("value")))
            .IsEqualTo("interface:value");
    }

    [Test]
    public async Task ShouldApplyDelegateRuleThroughGenericVariance()
    {
        var serializer = CreateSerializer(
            context => context.AddDelegateRule<IConverter<string, object>>(
                value => $"variant:{value.Convert("value")}"
            )
        );

        await Assert.That(serializer.Serialize(new ObjectToStringConverter()))
            .IsEqualTo("variant:value");
    }

    [Test]
    public async Task ShouldReplaceDefaultJsonOptions()
    {
        var serializer = CreateSerializer(
            context => context.UseJsonOptions(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            })
        );
        var value = new JsonObject
        {
            PascalName = "value",
            OptionalValue = null,
        };

        await Assert.That(serializer.Serialize(value))
            .IsEqualTo(
                """{"pascalName":"value","optionalValue":null}"""
            );
        await Assert.That(serializer.Serialize(SampleEnum.Second))
            .IsEqualTo("1");
    }

    [Test]
    public async Task ShouldEvaluateJsonOptionsFactoryAtBuildTime()
    {
        var factoryCalls = 0;
        var options = new JsonSerializerOptions();

        var environment = RuntimeTestEnvironment.Create(
            configure: builder => builder.ConfigureSerialization(
                context => context.UseJsonOptions(() =>
                {
                    factoryCalls++;
                    return options;
                })
            )
        );

        await Assert.That(factoryCalls).IsEqualTo(1);
        await Assert.That(
            environment.Runtime.ParameterSerializer.Serialize(17)
        ).IsEqualTo("17");
        await Assert.That(factoryCalls).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldApplyJsonOptionTransformersInRegistrationOrder()
    {
        JsonSerializerOptions? firstResult = null;
        JsonSerializerOptions? secondInput = null;
        var serializer = CreateSerializer(context =>
        {
            context.TransformJsonOptions(options =>
            {
                firstResult = new(options)
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                };
                return firstResult;
            });
            context.TransformJsonOptions(options =>
            {
                secondInput = options;
                return new(options)
                {
                    DefaultIgnoreCondition =
                        System.Text.Json.Serialization.JsonIgnoreCondition.Never,
                };
            });
        });

        await Assert.That(serializer.Serialize(new JsonObject
        {
            PascalName = "value",
            OptionalValue = null,
        })).IsEqualTo(
            """{"pascalName":"value","optionalValue":null}"""
        );
        await Assert.That(secondInput).IsSameReferenceAs(firstResult);
    }

    [Test]
    public async Task ShouldPreferExplicitJsonRuleOverDefaultJsonRule()
    {
        var serializer = CreateSerializer(context =>
            context.AddJsonRule(new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            })
        );

        await Assert.That(serializer.Serialize(new JsonObject
        {
            PascalName = "value",
        })).IsEqualTo(
            """{"pascalName":"value","optionalValue":null}"""
        );
    }

    [Test]
    public async Task ShouldPassFinalConfigurationToSerializationRegistration()
    {
        var configuration = new TestConfiguration
        {
            NullRepresentation = "configured-null",
        };
        TestConfiguration? receivedConfiguration = null;

        var environment = RuntimeTestEnvironment<TestConfiguration>.Create(
            configuration,
            builder => builder.ConfigureSerialization(
                (received, context) =>
                {
                    receivedConfiguration = received;
                    context.UseNullRepresentation(
                        received.NullRepresentation
                    );
                }
            )
        );

        await Assert.That(receivedConfiguration)
            .IsSameReferenceAs(configuration);
        await Assert.That(
            environment.Runtime.ParameterSerializer.Serialize(null)
        ).IsEqualTo("configured-null");
    }

    static IAllureParameterSerializer CreateSerializer(
        Action<IParameterSerializationRulesContext> configure
    ) =>
        RuntimeTestEnvironment.Create(
            configure: builder => builder.ConfigureSerialization(configure)
        ).Runtime.ParameterSerializer;

    sealed class PlainObject;

    record class BaseValue(string Name);

    sealed record class DerivedValue(string Name) : BaseValue(Name);

    interface INamedValue
    {
        string Name { get; }
    }

    sealed record class NamedValue(string Name) : INamedValue;

    interface IConverter<in TInput, out TOutput>
    {
        TOutput Convert(TInput input);
    }

    sealed class ObjectToStringConverter : IConverter<object, string>
    {
        public string Convert(object input) => input.ToString()!;
    }

    sealed class JsonObject
    {
        public string PascalName { get; init; } = "";

        public string? OptionalValue { get; init; }
    }

    enum SampleEnum
    {
        First,
        Second,
    }

    sealed record class TestConfiguration : AllureConfiguration
    {
        public string NullRepresentation { get; init; } = "";
    }
}
