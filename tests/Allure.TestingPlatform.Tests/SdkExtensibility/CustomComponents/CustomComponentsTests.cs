using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.SdkExtensibility.CustomComponents;

public class CustomComponentsTests
{
    [Test]
    public async Task ShouldReplaceAndConfigureRuntimeComponents()
    {
        var configuration = new AllureTestingPlatformConfiguration
        {
            IsProcessWatchdogEnabled = false,
        };
        MyResultsDestination destination = null;
        var builder = await ExtensibilityTestApplication.CreateBuilderAsync();

        var registration = builder.AddEmbeddedAllure(
            "custom-components",
            (context, _) =>
            {
                context.UseConfiguration(configuration);
                context.UseDestination(resolved =>
                {
                    destination = new(resolved);
                    return destination;
                });
                context.ConfigureSerialization(
                    rules => rules.UseNullRepresentation("<null>")
                );
            }
        );
        ExtensibilityTestApplication.RegisterTestFramework(builder);

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();

        await Assert.That(exitCode).IsEqualTo(8);
        await Assert.That(destination).IsNotNull();
        await Assert.That(destination.Configuration).IsSameReferenceAs(configuration);
        await Assert.That(registration.RuntimeReference.Value.ResultsDestination)
            .IsSameReferenceAs(destination);
        await Assert.That(registration.RuntimeReference.Value.ParameterSerializer.Serialize(null))
            .IsEqualTo("<null>");
    }

    sealed class MyResultsDestination(AllureTestingPlatformConfiguration configuration) :
        InMemoryResultsDestination
    {
        public AllureTestingPlatformConfiguration Configuration { get; } = configuration;
    }
}
