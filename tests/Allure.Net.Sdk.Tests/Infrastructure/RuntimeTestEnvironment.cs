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
            RecordingRuntimeHook<TConfiguration>,
            RecordingEndpointHook<TConfiguration>
        >>? configure = null
    )
    {
        var destination = new InMemoryResultsDestination();
        var builder = new AllureRuntimeBuilder<
            TConfiguration,
            RecordingRuntimeHook<TConfiguration>,
            RecordingEndpointHook<TConfiguration>
        >("sdk-test");

        builder.UseConfiguration(configuration ?? new TConfiguration());
        builder.UseDestination(_ => destination);
        configure?.Invoke(builder);

        return new(builder.Build(), destination);
    }
}

sealed class RuntimeTestEnvironment
{
    public static RuntimeTestEnvironment<AllureConfiguration> Create(
        AllureConfiguration? configuration = null,
        Action<AllureRuntimeBuilder<
            AllureConfiguration,
            RecordingRuntimeHook<AllureConfiguration>,
            RecordingEndpointHook<AllureConfiguration>
        >>? configure = null
    ) =>
        RuntimeTestEnvironment<AllureConfiguration>.Create(configuration, configure);
}

sealed class RecordingRuntimeHook<TConfiguration>(
    Action<IAllureRuntimeRegistrationContext<TConfiguration>>? setUp = null
) : IAllureRuntimeRegistrationHook<TConfiguration>
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
) : IAllureInProcessEndpointRegistrationHook<TConfiguration>
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
