using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.Net.Sdk.Tests.Infrastructure;

namespace Allure.Net.Sdk.Tests.Registration;

public class RuntimeComponentRegistrationTests
{
    [Test]
    public async Task ShouldBuildRuntimeFromFinalConfigurationAndRegisteredComponents()
    {
        var configuration = new TestConfiguration();
        var serializer = new RecordingParameterSerializer(_ => "serialized");
        var destination = new InMemoryResultsDestination();

        var context = InterfaceStub.Create<IAllureExecutionContext>();
        var lifecycle = InterfaceStub.Create<IAllureLifecycleApi>();
        var model = InterfaceStub.Create<IAllureModelApi>();

        var dependencyInputs =
            new List<IAllureRegistrationDependencies<TestConfiguration>>();

        TestConfiguration? serializerConfiguration = null;
        TestConfiguration? destinationConfiguration = null;

        var builder = CreateBuilder();

        builder.UseConfiguration(configuration);
        builder.UseParameterSerializer(received =>
        {
            serializerConfiguration = received;
            return serializer;
        });
        builder.UseDestination(received =>
        {
            destinationConfiguration = received;
            return destination;
        });
        builder.UseContext(dependencies =>
        {
            dependencyInputs.Add(dependencies);
            return context;
        });
        builder.UseLifecycleApi(dependencies =>
        {
            dependencyInputs.Add(dependencies);
            return lifecycle;
        });
        builder.UseModelApi(dependencies =>
        {
            dependencyInputs.Add(dependencies);
            return model;
        });

        var runtime = builder.Build();

        await Assert.That(serializerConfiguration)
            .IsSameReferenceAs(configuration);
        await Assert.That(destinationConfiguration)
            .IsSameReferenceAs(configuration);
        await Assert.That(dependencyInputs.Count).IsEqualTo(3);
        await Assert.That(dependencyInputs.Distinct().Count()).IsEqualTo(1);
        await Assert.That(dependencyInputs[0].Configuration)
            .IsSameReferenceAs(configuration);
        await Assert.That(dependencyInputs[0].ParameterSerializer)
            .IsSameReferenceAs(serializer);
        await Assert.That(runtime.Configuration).IsSameReferenceAs(configuration);
        await Assert.That(runtime.ParameterSerializer).IsSameReferenceAs(serializer);
        await Assert.That(runtime.ResultsDestination).IsSameReferenceAs(destination);
        await Assert.That(runtime.ContextApi).IsSameReferenceAs(context);
        await Assert.That(runtime.LifecycleApi).IsSameReferenceAs(lifecycle);
        await Assert.That(runtime.ModelApi).IsSameReferenceAs(model);
        await Assert.That(dependencyInputs[0].RuntimeReference.Value)
            .IsSameReferenceAs(runtime);
    }

    [Test]
    public async Task ShouldThrowWhenRuntimeReferenceIsReadDirectlyFromFactory()
    {
        var builder = CreateBuilder();
        builder.UseConfiguration(new TestConfiguration());
        builder.UseContext(dependencies =>
        {
            _ = dependencies.RuntimeReference.Value;
            return InterfaceStub.Create<IAllureExecutionContext>();
        });

        await Assert.That(builder.Build)
            .Throws<InvalidOperationException>()
            .WithMessageContaining("has not been bound");
    }

    static AllureRuntimeBuilder<
        TestConfiguration,
        RecordingRuntimeHook<TestConfiguration>,
        RecordingEndpointHook<TestConfiguration>
    > CreateBuilder() =>
        new("component-tests");

    sealed record class TestConfiguration : AllureConfiguration;
}
