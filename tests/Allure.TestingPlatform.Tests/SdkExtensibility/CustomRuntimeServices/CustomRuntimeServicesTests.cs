using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Tests.SdkExtensibility.CustomRuntimeServices;

public class CustomRuntimeServicesTests
{
    [Test]
    public async Task ShouldRegisterCustomRuntimeService()
    {
        var configuration = new MyFrameworkAllureConfiguration
        {
            CaptureFrameworkOutput = false,
            IsProcessWatchdogEnabled = false,
        };
        var correlationContext = new MyFrameworkCorrelationContext();
        var executionStateContext = new MyFrameworkExecutionStateContext();
        var destination = new InMemoryResultsDestination();
        var builder = await ExtensibilityTestApplication.CreateBuilderAsync();

        var registration = builder.AddEmbeddedAllure(
            "custom-runtime-services",
            () => new MyFrameworkAllureRegistrationSession(),
            (context, _) =>
            {
                context.UseConfiguration(configuration);
                context.UseCorrelationContext(_ => correlationContext);
                context.UseExecutionStateContext(_ => executionStateContext);
                context.UseDestination(_ => destination);
            }
        );
        ExtensibilityTestApplication.RegisterTestFramework(builder);

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();

        await Assert.That(exitCode).IsEqualTo(8);
        await Assert.That(registration.ConfigurationReference.Value)
            .IsSameReferenceAs(configuration);
        await Assert.That(registration.RuntimeReference.Value.Configuration)
            .IsSameReferenceAs(configuration);
        await Assert.That(registration.RuntimeReference.Value.OutputCapture.Enabled)
            .IsFalse();
        await Assert.That(registration.RuntimeReference.Value.CorrelationContext)
            .IsSameReferenceAs(correlationContext);
        await Assert.That(registration.RuntimeReference.Value.ExecutionStateContext)
            .IsSameReferenceAs(executionStateContext);
        await Assert.That(registration.RuntimeReference.Value.ResultsDestination)
            .IsSameReferenceAs(destination);
    }

    public sealed record MyFrameworkAllureConfiguration :
        AllureTestingPlatformConfiguration
    {
        public bool CaptureFrameworkOutput { get; init; } = true;
    }

    public sealed class FrameworkOutputCapture(bool enabled)
    {
        public bool Enabled { get; } = enabled;
    }

    sealed class MyFrameworkAllureRuntime(
        RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
        AllureTestingPlatformRuntimeArguments platform,
        FrameworkOutputCapture outputCapture = null
    ) : AllureTestingPlatformRuntime<MyFrameworkAllureConfiguration>(common, platform)
    {
        public FrameworkOutputCapture OutputCapture { get; } =
            outputCapture ?? new(common.Configuration.CaptureFrameworkOutput);
    }

    sealed class MyFrameworkAllureRegistrationSession :
        AllureTestingPlatformRuntimeRegistrationSession<
            MyFrameworkAllureConfiguration,
            MyFrameworkAllureRuntime
        >
    {
        protected override MyFrameworkAllureRuntime CreateRuntime(
            RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
            AllureTestingPlatformRuntimeArguments platform
        ) =>
            new(common, platform);
    }

    sealed class MyFrameworkCorrelationContext : ICorrelationContext
    {
        public CorrelationUid CurrentCorrelationUid { get; } = new("custom-runtime-services");
    }

    sealed class MyFrameworkExecutionStateContext : ExecutionStateContext
    {
        public override ScopeExecutionStateUid? CurrentScopeUid => null;

        public override TestExecutionStateUid? CurrentTestUid => null;

        protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid => null;

        protected override StepExecutionStateUid? CurrentFrameworkStepUid => null;
    }
}
