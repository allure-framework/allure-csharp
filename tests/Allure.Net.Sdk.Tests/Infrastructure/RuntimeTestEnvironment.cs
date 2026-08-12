using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Net.Sdk.Tests.Infrastructure;

sealed class RuntimeTestEnvironment<TConfiguration> : IDisposable
    where TConfiguration : AllureConfiguration, new()
{
    readonly IAllureRuntimeRegistration<IAllureRuntime<TConfiguration>> registration;
    RuntimeTestEnvironment(
        IAllureRuntimeRegistration<IAllureRuntime<TConfiguration>> registration,
        InMemoryResultsDestination destination
    )
    {
        this.registration = registration;
        this.Destination = destination;
    }

    public IAllureRuntime<TConfiguration> Runtime => this.registration.Runtime;

    public InMemoryResultsDestination Destination { get; }

    public static RuntimeTestEnvironment<TConfiguration> Create(
        TConfiguration? configuration = null,
        Action<
            IAllureRuntimeIntegrationContext<TConfiguration>
        >? configure = null
    )
    {
        var destination = new InMemoryResultsDestination();
        var builder = new AllureRuntimeBuilder<TConfiguration>("sdk-test");
        var plan = builder.Prepare((ctx) =>
        {
            ctx.UseConfiguration(configuration ?? new TConfiguration());
            ctx.UseDestination(_ => destination);
            configure?.Invoke(ctx);
        });

        return new(plan.Build(), destination);
    }

    public void Dispose()
    {
        this.registration?.Dispose();
    }
}

sealed class RuntimeTestEnvironment
{
    public static RuntimeTestEnvironment<AllureConfiguration> Create(
        AllureConfiguration? configuration = null,
        Action<IAllureRuntimeIntegrationContext<AllureConfiguration>>? configure = null
    ) =>
        RuntimeTestEnvironment<AllureConfiguration>.Create(configuration, configure);
}

sealed class RecordingRuntimeHook<TConfiguration>(
    Action<IAllureRuntimeRegistrationContext<TConfiguration>>? setUp = null
) : IAllureRegistrationHook<IAllureRuntimeRegistrationContext<TConfiguration>>
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

sealed class RecordingEndpointHook<TConfiguration, TRuntime>(
    Action<IAllureInProcessEndpointRegistrationContext<TRuntime>>? setUp = null
) : IAllureRegistrationHook<IAllureInProcessEndpointRegistrationContext<TRuntime>>
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    public int CallCount { get; private set; }

    public void SetUp(
        IAllureInProcessEndpointRegistrationContext<TRuntime> context
    )
    {
        this.CallCount++;
        setUp?.Invoke(context);
    }
}
