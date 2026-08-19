using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Tests.SdkExtensibility.BrandedRegistrationHooks;

public class BrandedRegistrationHookTests
{
    [Test]
    public async Task ShouldApplyBrandedRegistrationHook()
    {
        MyFrameworkAllureRegistrationHook.Calls = 0;
        var configuration = new MyFrameworkAllureConfiguration
        {
            CaptureFrameworkOutput = true,
            IsProcessWatchdogEnabled = false,
            RuntimeRegistrationHook =
                typeof(MyFrameworkAllureRegistrationHook).AssemblyQualifiedName,
        };
        var destination = new InMemoryResultsDestination();
        var correlationContext = new MyFrameworkCorrelationContext();
        var executionStateContext = new MyFrameworkExecutionStateContext();
        var builder = await ExtensibilityTestApplication.CreateBuilderAsync();

        var registration = builder.AddEmbeddedAllure(
            "branded-registration-hook",
            () => new MyFrameworkAllureRegistrationSession(),
            (context, _) =>
            {
                context.UseConfiguration(configuration);
                context.UseCorrelationContext(_ => correlationContext);
                context.UseDestination(_ => destination);
                context.UseExecutionStateContext(_ => executionStateContext);
            }
        );
        ExtensibilityTestApplication.RegisterTestFramework(builder);

        using var app = await builder.BuildAsync();
        var exitCode = await app.RunAsync();

        await Assert.That(exitCode).IsEqualTo(8);
        await Assert.That(MyFrameworkAllureRegistrationHook.Calls).IsEqualTo(1);
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

    public interface IMyFrameworkAllureRegistrationContext :
        IAllureTestingPlatformRegistrationContext<MyFrameworkAllureConfiguration>
    {
        void UseOutputCapture(
            Func<MyFrameworkAllureConfiguration, FrameworkOutputCapture> factory
        );
    }

    public interface IMyFrameworkAllureRegistrationHook :
        IAllureTestingPlatformRegistrationHook<IMyFrameworkAllureRegistrationContext>;

    public sealed class MyFrameworkAllureRegistrationHook :
        IMyFrameworkAllureRegistrationHook
    {
        public static int Calls { get; set; }

        public void SetUp(IMyFrameworkAllureRegistrationContext context)
        {
            Calls++;
            context.UseOutputCapture(
                _ => new FrameworkOutputCapture(enabled: false)
            );
        }
    }

    sealed class MyFrameworkAllureRuntime(
        RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
        AllureTestingPlatformRuntimeArguments platform,
        FrameworkOutputCapture outputCapture
    ) : AllureTestingPlatformRuntime<MyFrameworkAllureConfiguration>(common, platform)
    {
        public FrameworkOutputCapture OutputCapture { get; } = outputCapture;
    }

    sealed class MyFrameworkCorrelationContext : ICorrelationContext
    {
        public CorrelationUid CurrentCorrelationUid { get; } =
            new("branded-registration-hook");
    }

    sealed class MyFrameworkExecutionStateContext : ExecutionStateContext
    {
        public override ScopeExecutionStateUid? CurrentScopeUid => null;

        public override TestExecutionStateUid? CurrentTestUid => null;

        protected override FixtureExecutionStateUid? CurrentFrameworkFixtureUid => null;

        protected override StepExecutionStateUid? CurrentFrameworkStepUid => null;
    }

    sealed class MyFrameworkAllureRegistrationSession :
        AllureTestingPlatformRegistrationSession<
            MyFrameworkAllureConfiguration,
            MyFrameworkAllureRuntime,
            IMyFrameworkAllureRegistrationContext,
            IAllureTestingPlatformIntegrationContext<
                MyFrameworkAllureConfiguration,
                MyFrameworkAllureRuntime,
                IMyFrameworkAllureRegistrationContext
            >
        >,
        IMyFrameworkAllureRegistrationContext
    {
        Func<MyFrameworkAllureConfiguration, FrameworkOutputCapture> outputCaptureFactory =
            configuration => new(configuration.CaptureFrameworkOutput);

        protected override IMyFrameworkAllureRegistrationContext RegistrationContext => this;

        protected override IAllureTestingPlatformIntegrationContext<
            MyFrameworkAllureConfiguration,
            MyFrameworkAllureRuntime,
            IMyFrameworkAllureRegistrationContext
        > IntegrationContext => this;

        public void UseOutputCapture(
            Func<MyFrameworkAllureConfiguration, FrameworkOutputCapture> factory
        ) =>
            this.Modify(() => this.outputCaptureFactory = factory);

        protected override MyFrameworkAllureRuntime CreateRuntime(
            RuntimeCreationArguments<MyFrameworkAllureConfiguration> common,
            AllureTestingPlatformRuntimeArguments platform
        ) =>
            new(common, platform, this.outputCaptureFactory(common.Configuration));
    }
}
