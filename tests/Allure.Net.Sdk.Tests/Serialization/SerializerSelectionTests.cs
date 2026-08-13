using Allure.Abstractions;
using Allure.Net.Sdk.Tests.Infrastructure;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Net.Sdk.Tests.Serialization;

public class SerializerSelectionTests
{
    [Test]
    public async Task ShouldUseExplicitParameterSerializer()
    {
        var expected = new RecordingParameterSerializer(_ => "explicit");
        using var registration = PrepareBuilder(builder =>
            builder.UseParameterSerializer(() => expected)
        ).Build();

        await Assert.That(registration.Runtime.ParameterSerializer)
            .IsSameReferenceAs(expected);
        await Assert.That(registration.Runtime.ParameterSerializer.Serialize(17))
            .IsEqualTo("explicit");
    }

    [Test]
    public async Task ShouldUseExplicitSerializerWhenConfiguredAfterRules()
    {
        var expected = new RecordingParameterSerializer(_ => "explicit");
        using var registration = PrepareBuilder(builder =>
        {
            builder.ConfigureSerialization(
                context => context.AddDelegateRule<int>(_ => "rule")
            );
            builder.UseParameterSerializer(() => expected);
        }).Build();

        await Assert.That(registration.Runtime.ParameterSerializer)
            .IsSameReferenceAs(expected);
        await Assert.That(registration.Runtime.ParameterSerializer.Serialize(17))
            .IsEqualTo("explicit");
    }

    [Test]
    public async Task ShouldRestoreRuleBasedSerializerWhenRulesAreConfiguredAfterExplicitSerializer()
    {
        var explicitSerializer =
            new RecordingParameterSerializer(_ => "explicit");
        using var registration = PrepareBuilder(builder =>
        {
            builder.UseParameterSerializer(() => explicitSerializer);
            builder.ConfigureSerialization(
                context => context.AddDelegateRule<int>(_ => "rule")
            );
        }).Build();

        await Assert.That(registration.Runtime.ParameterSerializer)
            .IsNotSameReferenceAs(explicitSerializer);
        await Assert.That(registration.Runtime.ParameterSerializer.Serialize(17))
            .IsEqualTo("rule");
        await Assert.That(explicitSerializer.Values).IsEmpty();
    }

    [Test]
    public async Task ShouldClearPreviousRulesWhenExplicitSerializerIsSelected()
    {
        var explicitSerializer =
            new RecordingParameterSerializer(_ => "explicit");
        using var registration = PrepareBuilder(builder =>
        {
            builder.ConfigureSerialization(
                context => context.AddDelegateRule<string>(_ => "old-rule")
            );
            builder.UseParameterSerializer(() => explicitSerializer);
            builder.ConfigureSerialization(
                context => context.AddDelegateRule<int>(_ => "new-rule")
            );
        }).Build();

        await Assert.That(registration.Runtime.ParameterSerializer.Serialize(17))
            .IsEqualTo("new-rule");
        await Assert.That(registration.Runtime.ParameterSerializer.Serialize("text"))
            .IsEqualTo("\"text\"");
        await Assert.That(explicitSerializer.Values).IsEmpty();
    }

    [Test]
    public async Task ShouldAccumulateSerializationConfigurationsInOrder()
    {
        var firstCalls = 0;
        var secondCalls = 0;
        using var registration = PrepareBuilder(builder =>
        {
            builder.ConfigureSerialization(context =>
                context.AddDelegateRule<int>(_ =>
                {
                    firstCalls++;
                    return "first";
                })
            );
            builder.ConfigureSerialization(context =>
                context.AddDelegateRule<int>(_ =>
                {
                    secondCalls++;
                    return "second";
                })
            );
        }).Build();

        await Assert.That(registration.Runtime.ParameterSerializer.Serialize(17))
            .IsEqualTo("second");
        await Assert.That(firstCalls).IsEqualTo(0);
        await Assert.That(secondCalls).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldSelectRuntimeAndEndpointSerializersIndependently()
    {
        var runtimeSerializer =
            new RecordingParameterSerializer(_ => "runtime");
        var endpointSerializer =
            new RecordingParameterSerializer(_ => "endpoint");
        IAllureParameterSerializer? selectedEndpointSerializer = null;
        IAllureRuntime<AllureConfiguration>? endpointRuntime = null;
        var endpointFactoryCalls = 0;

        using var registration = PrepareBuilder(builder =>
        {
            builder.UseParameterSerializer(() => runtimeSerializer);
            builder.RegisterInProcessEndpoint(
                $"serializer-selection-{Guid.NewGuid():N}",
                (receivedRuntime, endpoint) =>
                {
                    endpoint.SetAvailabilityPredicate(_ => false);
                    endpoint.UseCurrentScopePredicate(_ => false);
                    endpoint.UseGlobalScopePredicate(_ => false);
                    endpoint.UseParameterSerializer(runtime =>
                    {
                        endpointFactoryCalls++;
                        endpointRuntime = runtime;
                        selectedEndpointSerializer = endpointSerializer;
                        return endpointSerializer;
                    });
                }
            );
        }).Build();

        await Assert.That(registration.Runtime.ParameterSerializer)
            .IsSameReferenceAs(runtimeSerializer);
        await Assert.That(endpointFactoryCalls).IsEqualTo(1);
        await Assert.That(registration.Runtime).IsSameReferenceAs(endpointRuntime);
        await Assert.That(selectedEndpointSerializer)
            .IsSameReferenceAs(endpointSerializer);
        await Assert.That(selectedEndpointSerializer!.Serialize(17))
            .IsEqualTo("endpoint");
        await Assert.That(registration.Runtime.ParameterSerializer.Serialize(17))
            .IsEqualTo("runtime");
    }

    static IAllureRuntimeRegistrationPlan<AllureConfiguration, IAllureRuntime<AllureConfiguration>> PrepareBuilder(
        Action<IAllureRuntimeIntegrationContext<AllureConfiguration>> configure
    )
    {
        var builder = new AllureRuntimeBuilder<AllureConfiguration>("serializer-selection");
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(new AllureConfiguration());
            configure(ctx);
        });
        return plan;
    }
}
