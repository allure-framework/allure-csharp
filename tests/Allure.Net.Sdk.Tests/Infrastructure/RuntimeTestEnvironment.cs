using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Net.Sdk.Tests.Infrastructure;

sealed class RuntimeTestEnvironment<TConfiguration>
    where TConfiguration : AllureConfiguration, new()
{
    RuntimeTestEnvironment(
        IAllureRuntime<TConfiguration> runtime,
        InMemoryResultsDestination destination
    )
    {
        this.Runtime = runtime;
        this.Destination = destination;
    }

    public IAllureRuntime<TConfiguration> Runtime { get; }

    public InMemoryResultsDestination Destination { get; }

    public static RuntimeTestEnvironment<TConfiguration> Create(
        TConfiguration? configuration = null,
        Action<AllureRuntimeBuilder<
            TConfiguration,
            IAllureRuntimeRegistrationContext<TConfiguration>,
            RecordingRuntimeHook<TConfiguration>,
            IAllureInProcessEndpointRegistrationContext<TConfiguration>,
            RecordingEndpointHook<TConfiguration>
        >>? configure = null
    )
    {
        var destination = new InMemoryResultsDestination();
        var builder = new TestRuntimeBuilder<TConfiguration>("sdk-test");

        builder.UseConfiguration(configuration ?? new TConfiguration());
        builder.UseDestination(_ => destination);
        configure?.Invoke(builder);

        return new(builder.Build(), destination);
    }
}

sealed class TestRuntimeBuilder<TConfiguration>(string runtimeName) : AllureRuntimeBuilder<
    TConfiguration,
    IAllureRuntimeRegistrationContext<TConfiguration>,
    RecordingRuntimeHook<TConfiguration>,
    IAllureInProcessEndpointRegistrationContext<TConfiguration>,
    RecordingEndpointHook<TConfiguration>
>(runtimeName)
    where TConfiguration : AllureConfiguration, new()
{
    protected override IAllureRuntimeRegistrationContext<TConfiguration> RegistrationContext => this;

    protected override AllureInProcessRouteBuilder<TConfiguration, IAllureInProcessEndpointRegistrationContext<TConfiguration>, RecordingEndpointHook<TConfiguration>> CreateRouteBuilder(
        AllureRouteBuilderArgs<TConfiguration> args
    )
    {
        return new TestEndpointRouteBuilder<TConfiguration>(args);
    }
}

sealed class TestEndpointRouteBuilder<TConfiguration>(AllureRouteBuilderArgs<TConfiguration> args) :
    AllureInProcessRouteBuilder<
        TConfiguration,
        IAllureInProcessEndpointRegistrationContext<TConfiguration>,
        RecordingEndpointHook<TConfiguration>
    >(args)

    where TConfiguration : AllureConfiguration
{
    protected override IAllureInProcessEndpointRegistrationContext<TConfiguration> RegistrationContext => this;
}

sealed class RuntimeTestEnvironment
{
    public static RuntimeTestEnvironment<AllureConfiguration> Create(
        AllureConfiguration? configuration = null,
        Action<AllureRuntimeBuilder<
            AllureConfiguration,
            IAllureRuntimeRegistrationContext<AllureConfiguration>,
            RecordingRuntimeHook<AllureConfiguration>,
            IAllureInProcessEndpointRegistrationContext<AllureConfiguration>,
            RecordingEndpointHook<AllureConfiguration>
        >>? configure = null
    ) =>
        RuntimeTestEnvironment<AllureConfiguration>.Create(configuration, configure);
}

sealed class RecordingRuntimeHook<TConfiguration>(
    Action<IAllureRuntimeRegistrationContext<TConfiguration>>? setUp = null
) : IAllureRuntimeRegistrationHook<TConfiguration, IAllureRuntimeRegistrationContext<TConfiguration>>
    where TConfiguration : AllureConfiguration, new()
{
    public int CallCount { get; private set; }

    public void SetUp(
        IAllureRuntimeRegistrationContext<TConfiguration> context
    )
    {
        this.CallCount++;
        setUp?.Invoke(context);
    }
}

sealed class RecordingEndpointHook<TConfiguration>(
    Action<IAllureInProcessEndpointRegistrationContext<TConfiguration>>? setUp = null
) : IAllureInProcessEndpointRegistrationHook<TConfiguration, IAllureInProcessEndpointRegistrationContext<TConfiguration>>
    where TConfiguration : AllureConfiguration
{
    public int CallCount { get; private set; }

    public void SetUp(
        IAllureInProcessEndpointRegistrationContext<TConfiguration> context
    )
    {
        this.CallCount++;
        setUp?.Invoke(context);
    }
}
