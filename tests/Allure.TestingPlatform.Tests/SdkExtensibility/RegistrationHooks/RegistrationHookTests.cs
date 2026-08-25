using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Tests.SdkExtensibility.RegistrationHooks;

public class RegistrationHookTests
{
    [Test]
    public async Task ShouldApplyConfiguredRegistrationHook()
    {
        MyAllureRegistrationHook.Calls = 0;
        var configuration = new AllureTestingPlatformConfiguration
        {
            IsProcessWatchdogEnabled = false,
            RuntimeRegistrationHook = typeof(MyAllureRegistrationHook).AssemblyQualifiedName,
        };
        var builder = await ExtensibilityTestApplication.CreateBuilderAsync();

        var registration = builder.AddEmbeddedAllure(
            "registration-hook",
            (context, _) => context.UseConfiguration(configuration)
        );
        ExtensibilityTestApplication.RegisterTestFramework(builder);

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();

        await Assert.That(exitCode).IsEqualTo(8);
        await Assert.That(MyAllureRegistrationHook.Calls).IsEqualTo(1);
        await Assert.That(registration.RuntimeReference.Value.ParameterSerializer.Serialize(null))
            .IsEqualTo("<null>");
    }

    [Test]
    public async Task ShouldConfigureEndpointFromRegistrationHook()
    {
        EndpointConfiguringRegistrationHook.SuppressedRouteIdsFactoryCalls = 0;
        var configuration = new AllureTestingPlatformConfiguration
        {
            IsProcessWatchdogEnabled = false,
            RuntimeRegistrationHook =
                typeof(EndpointConfiguringRegistrationHook).AssemblyQualifiedName,
        };
        var builder = await ExtensibilityTestApplication.CreateBuilderAsync();

        builder.AddEmbeddedAllure(
            "endpoint-configuring-registration-hook",
            (context, _) => context.UseConfiguration(configuration)
        );
        ExtensibilityTestApplication.RegisterTestFramework(builder);

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();

        await Assert.That(exitCode).IsEqualTo(8);
        await Assert.That(EndpointConfiguringRegistrationHook.SuppressedRouteIdsFactoryCalls)
            .IsEqualTo(1);
    }

    public sealed class MyAllureRegistrationHook :
        IAllureTestingPlatformRegistrationHook
    {
        public static int Calls { get; set; }

        public void SetUp(IAllureTestingPlatformRegistrationContext context)
        {
            Calls++;
            context.ConfigureSerialization(
                rules => rules.UseNullRepresentation("<null>")
            );
        }
    }

    public sealed class EndpointConfiguringRegistrationHook :
        IAllureTestingPlatformRegistrationHook
    {
        public static int SuppressedRouteIdsFactoryCalls { get; set; }

        public void SetUp(IAllureTestingPlatformRegistrationContext context)
        {
            context.ConfigureEndpoint((_, endpoint) =>
                endpoint.SuppressRoutes(() =>
                {
                    SuppressedRouteIdsFactoryCalls++;
                    return ["general-adapter"];
                })
            );
        }
    }
}
