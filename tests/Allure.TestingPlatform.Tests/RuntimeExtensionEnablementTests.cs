using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests;

public class RuntimeExtensionEnablementTests
{
    [Test]
    public async Task ShouldReportConfiguredRuntimeEnablement()
    {
        ExtensionProbe enabled = new(RuntimeReference(EnabledRuntime()));
        ExtensionProbe disabled = new(RuntimeReference(DisabledRuntime()));
        ExtensionProbe suppressed = new(RuntimeReference(new SuppressedAllureTestingPlatformRuntime(
            AllureTestingPlatformRegistrationMode.Standalone
        )));

        await Assert.That(enabled.IsEnabledAsync()).IsTrue();
        await Assert.That(disabled.IsEnabledAsync()).IsFalse();
        await Assert.That(suppressed.IsEnabledAsync()).IsFalse();
    }

    [Test]
    public async Task ShouldThrowWhenRuntimeIsNotInitialized()
    {
        ExtensionProbe extension = new(RuntimeReference(new()));

        await Assert.That(extension.IsEnabledAsync)
            .Throws<InvalidOperationException>()
            .WithMessage("Unexpected error: Allure.TestingPlatform runtime is not configured.");
    }

    [Test]
    public async Task ShouldExposeConfiguredRuntimeServices()
    {
        var logger = new LoggerSpy();
        AllureConfiguration configuration = new();
        ExtensionProbe extension = new(RuntimeReference(EnabledRuntime(logger, configuration)));

        await Assert.That(extension.GetLogger()).IsSameReferenceAs(logger);
        await Assert.That(extension.GetConfiguration()).IsSameReferenceAs(configuration);
    }

    [Test]
    public async Task ShouldThrowWhenConfiguredRuntimeServicesAreUnavailable()
    {
        ExtensionProbe extension = new(RuntimeReference(new SuppressedAllureTestingPlatformRuntime(
            AllureTestingPlatformRegistrationMode.Standalone
        )));

        await Assert.That(extension.GetLogger)
            .Throws<InvalidOperationException>()
            .WithMessage(
                "Allure configuration is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );
    }

    [Test]
    public async Task ShouldExposeLiveRuntimeServices()
    {
        var writer = new InMemoryResultsWriter();
        AllureLifecycle lifecycle = new(_ => writer);
        TestingPlatformSessionUidCorrelationStrategy correlationStrategy = new();
        ImmutableDictionary<Type, ITypeFormatter> typeFormatters =
            new Dictionary<Type, ITypeFormatter>
            {
                [typeof(string)] = new TypeFormatterStub<string>("stub"),
            }.ToImmutableDictionary();
        ExtensionProbe extension = new(RuntimeReference(LiveRuntime(
            writer,
            lifecycle,
            correlationStrategy,
            typeFormatters
        )));

        await Assert.That(extension.GetWriter()).IsSameReferenceAs(writer);
        await Assert.That(extension.GetLifecycle()).IsSameReferenceAs(lifecycle);
        await Assert.That(extension.GetCorrelationStrategy()).IsSameReferenceAs(correlationStrategy);
        await Assert.That(extension.GetTypeFormatters()).IsEquivalentTo(typeFormatters);
    }

    [Test]
    public async Task ShouldThrowWhenLiveRuntimeServicesAreUnavailable()
    {
        ExtensionProbe extension = new(RuntimeReference(EnabledRuntime()));

        await Assert.That(extension.GetWriter)
            .Throws<InvalidOperationException>()
            .WithMessage(
                "Allure runtime is unavailable. Check if Allure.TestingPlatform is initialized correctly."
            );
    }

    [Test]
    public async Task ShouldConfigureRuntimeControllerExtensionBeforeCheckingEnablement()
    {
        MutableRuntimeReference runtimeReference = new(new());
        RuntimeControllerSpy controller = new(
            runtimeReference,
            configureResult: EnabledRuntime(),
            startResult: LiveRuntime()
        );
        RuntimeControllerExtensionProbe extension = new(controller);

        await Assert.That(extension.IsEnabledAsync()).IsTrue();

        await Assert.That(controller.ConfigureCallCount).IsEqualTo(1);
        await Assert.That(controller.StartCallCount).IsEqualTo(0);
        await Assert.That(runtimeReference.CurrentRuntime.Phase)
            .IsEqualTo(AllureTestingPlatformRuntimePhase.Configured);
    }

    [Test]
    public async Task ShouldNotConfigureRuntimeControllerExtensionTwice()
    {
        MutableRuntimeReference runtimeReference = new(DisabledRuntime());
        RuntimeControllerSpy controller = new(
            runtimeReference,
            configureResult: EnabledRuntime(),
            startResult: LiveRuntime()
        );
        RuntimeControllerExtensionProbe extension = new(controller);

        await Assert.That(extension.IsEnabledAsync()).IsFalse();

        await Assert.That(controller.ConfigureCallCount).IsEqualTo(0);
        await Assert.That(controller.StartCallCount).IsEqualTo(0);
    }

    [Test]
    public async Task ShouldStartConfiguredRuntimeOnDemand()
    {
        var liveRuntime = LiveRuntime();
        MutableRuntimeReference runtimeReference = new(EnabledRuntime());
        RuntimeControllerSpy controller = new(
            runtimeReference,
            configureResult: EnabledRuntime(),
            startResult: liveRuntime
        );
        RuntimeControllerExtensionProbe extension = new(controller);

        var runtime = extension.EnsureStarted();

        await Assert.That(runtime).IsSameReferenceAs(liveRuntime);
        await Assert.That(runtimeReference.CurrentRuntime).IsSameReferenceAs(liveRuntime);
        await Assert.That(controller.StartCallCount).IsEqualTo(1);
    }

    [Test]
    public async Task ShouldNotStartRuntimeThatIsNotConfigured()
    {
        var disabledRuntime = DisabledRuntime();
        MutableRuntimeReference runtimeReference = new(disabledRuntime);
        RuntimeControllerSpy controller = new(
            runtimeReference,
            configureResult: EnabledRuntime(),
            startResult: LiveRuntime()
        );
        RuntimeControllerExtensionProbe extension = new(controller);

        var runtime = extension.EnsureStarted();

        await Assert.That(runtime).IsSameReferenceAs(disabledRuntime);
        await Assert.That(controller.StartCallCount).IsEqualTo(0);
    }

    static MutableRuntimeReference RuntimeReference(AllureTestingPlatformRuntimeState runtime) =>
        new(runtime);

    static EnabledAllureTestingPlatformRuntime EnabledRuntime(
        ILogger logger = null,
        AllureConfiguration configuration = null
    ) =>
        new(
            AllureTestingPlatformRegistrationMode.Standalone,
            logger ?? new LoggerSpy(),
            configuration ?? new()
        );

    static DisabledAllureTestingPlatformRuntime DisabledRuntime(
        ILogger logger = null,
        AllureConfiguration configuration = null
    ) =>
        new(
            AllureTestingPlatformRegistrationMode.Standalone,
            logger ?? new LoggerSpy(),
            configuration ?? new()
        );

    static LiveAllureTestingPlatformRuntime LiveRuntime(
        IAllureResultsWriter writer = null,
        AllureLifecycle lifecycle = null,
        ICorrelationStrategy correlationStrategy = null,
        ImmutableDictionary<Type, ITypeFormatter> typeFormatters = null
    )
    {
        writer ??= new InMemoryResultsWriter();

        return new(
            AllureTestingPlatformRegistrationMode.Standalone,
            new LoggerSpy(),
            new(),
            correlationStrategy ?? new TestingPlatformSessionUidCorrelationStrategy(),
            writer,
            typeFormatters ?? ImmutableDictionary<Type, ITypeFormatter>.Empty,
            lifecycle ?? new(_ => writer)
        );
    }

    sealed class ExtensionProbe(IAllureTestingPlatformRuntimeReference runtimeReference) :
        AllureTestingPlatformExtension(
            "extension-probe",
            "Extension probe",
            "Test extension probe",
            runtimeReference
        )
    {
        public ILogger GetLogger() => this.Logger;

        public AllureConfiguration GetConfiguration() => this.Configuration;

        public IAllureResultsWriter GetWriter() => this.Writer;

        public ImmutableDictionary<Type, ITypeFormatter> GetTypeFormatters() => this.TypeFormatters;

        public AllureLifecycle GetLifecycle() => this.Lifecycle;

        public ICorrelationStrategy GetCorrelationStrategy() => this.CorrelationStrategy;
    }

    sealed class RuntimeControllerExtensionProbe(IAllureTestingPlatformRuntimeController controller) :
        AllureTestingPlatformRuntimeControllerExtension(
            "controller-extension-probe",
            "Controller extension probe",
            "Test controller extension probe",
            controller
        )
    {
        public AllureTestingPlatformRuntimeState EnsureStarted() => this.EnsureRuntimeStarted();
    }

    sealed class RuntimeControllerSpy(
        MutableRuntimeReference runtimeReference,
        AllureTestingPlatformRuntimeState configureResult,
        AllureTestingPlatformRuntimeState startResult
    ) : IAllureTestingPlatformRuntimeController
    {
        public int ConfigureCallCount { get; private set; }

        public int StartCallCount { get; private set; }

        public IAllureTestingPlatformRuntimeReference RuntimeReference => runtimeReference;

        public AllureTestingPlatformRuntimeState Configure()
        {
            this.ConfigureCallCount++;
            runtimeReference.CurrentRuntime = configureResult;
            return configureResult;
        }

        public AllureTestingPlatformRuntimeState Start()
        {
            this.StartCallCount++;
            runtimeReference.CurrentRuntime = startResult;
            return startResult;
        }
    }

    sealed class MutableRuntimeReference(
        AllureTestingPlatformRuntimeState currentRuntime
    ) : IAllureTestingPlatformRuntimeReference
    {
        public AllureTestingPlatformRuntimeState CurrentRuntime { get; set; } = currentRuntime;
    }
}
