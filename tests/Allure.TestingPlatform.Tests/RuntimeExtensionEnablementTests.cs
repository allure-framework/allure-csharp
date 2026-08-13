using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests;

public class RuntimeExtensionEnablementTests
{
    [Test]
    public async Task ShouldReportConfiguredRuntimeEnablement()
    {
        AllureTestingPlatformConfiguration enabledConfig = new() { IsEnabled = true };
        AllureTestingPlatformConfiguration disabledConfig = new() { IsEnabled = false };
        var enabledPlan = CreatePlan(enabledConfig);
        var disabledPlan = CreatePlan(disabledConfig);
        enabledPlan.Build();
        disabledPlan.Build();

        ExtensionProbe enabled = new(enabledConfig, enabledPlan.RuntimeReference);
        ExtensionProbe disabled = new(disabledConfig, disabledPlan.RuntimeReference);

        await Assert.That(enabled.IsEnabledAsync()).IsTrue();
        await Assert.That(disabled.IsEnabledAsync()).IsFalse();
    }

    [Test]
    public async Task ShouldExposeServicesFromRegisteredRuntime()
    {
        var config = new AllureTestingPlatformConfiguration();
        var logger = new LoggerSpy();
        var writer = new InMemoryResultsDestination();
        var correlation = new TestingPlatformSessionUidCorrelationStrategy();
        var plan = CreatePlan(config, logger, writer, correlation);
        plan.Build();
        ExtensionProbe extension = new(config, plan.RuntimeReference);

        await Assert.That(extension.GetConfiguration()).IsSameReferenceAs(config);
        await Assert.That(extension.GetLogger()).IsSameReferenceAs(logger);
        await Assert.That(extension.GetResultsDestination()).IsSameReferenceAs(writer);
        await Assert.That(extension.GetCorrelationStrategy()).IsSameReferenceAs(correlation);
        await Assert.That(extension.GetLifecycleApi()).IsNotNull();
        await Assert.That(extension.GetModelApi()).IsNotNull();
    }

    [Test]
    public async Task ShouldBuildControllerRuntimeOnDemandOnlyOnce()
    {
        var plan = CreatePlan(new());
        RuntimeControllerExtensionProbe extension = new(plan);

        await Assert.That(extension.IsEnabledAsync()).IsTrue();
        var first = extension.EnsureStarted();
        var second = extension.EnsureStarted();

        await Assert.That(first).IsSameReferenceAs(second);
        await Assert.That(plan.RuntimeReference.Value).IsSameReferenceAs(first.Runtime);
    }

    static IAllureRuntimeRegistrationPlan<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    > CreatePlan(
        AllureTestingPlatformConfiguration config,
        ILogger logger = null,
        IAllureResultsDestination writer = null,
        ICorrelationStrategy correlation = null
    )
    {
        var builder = new AllureTestingPlatformRuntimeBuilder("extension-test");
        return builder.Prepare(context =>
        {
            context.UseConfigurationSource(
                () => DelegateConfigurationSource.Create("test", () => config)
            );
            if (logger is not null)
            {
                context.UseLogger(_ => logger);
            }
            if (writer is not null)
            {
                context.UseDestination(_ => writer);
            }
            if (correlation is not null)
            {
                context.UseCorrelationStrategy(_ => correlation);
            }
        });
    }

    sealed class ExtensionProbe(
        AllureTestingPlatformConfiguration configuration,
        IReadOnlyLateBoundReference<
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        > runtimeReference
    ) :
        AllureTestingPlatformExtension<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        >(
            "extension-probe",
            "Extension probe",
            "Test extension probe",
            configuration,
            runtimeReference
        )
    {
        public AllureTestingPlatformConfiguration GetConfiguration() => this.Configuration;

        public ICorrelationStrategy GetCorrelationStrategy() => this.CorrelationStrategy;

        public IAllureLifecycleApi GetLifecycleApi() => this.LifecycleApi;

        public ILogger GetLogger() => this.Logger;

        public IAllureModelApi GetModelApi() => this.ModelApi;

        public IAllureResultsDestination GetResultsDestination() => this.ResultsDestination;
    }

    sealed class RuntimeControllerExtensionProbe(
        IAllureRuntimeRegistrationPlan<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        > plan
    ) :
        AllureTestingPlatformRuntimeControllerExtension<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        >(
            "controller-extension-probe",
            "Controller extension probe",
            "Test controller extension probe",
            plan
        )
    {
        public IAllureRuntimeRegistration<
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        > EnsureStarted() => this.EnsureRuntimeStarted();
    }
}
