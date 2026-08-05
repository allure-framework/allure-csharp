using System;
using System.Threading;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides the base implementation for builders that construct a custom Allure
/// runtime and its optional in-process endpoint.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TRuntimeIntegrationContext">The integration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
public class AllureRuntimeBuilder<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TRuntime
>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSession<
            TConfiguration,
            TRuntimeIntegrationContext,
            TRuntime
        >
    > sessionFactory
)

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntimeIntegrationContext : IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
    where TRuntime : IAllureRuntime<TConfiguration>
{
    int state = 0;

    readonly LateBoundReference<TRuntime> runtimeReference = new();

    /// <summary>
    /// A late bound reference to a runtime that becomes available after
    /// <see cref="IAllureRuntimeRegistrationPlan{TConfiguration, TRuntime}.Build"/>
    /// is called.
    /// </summary>
    public IReadOnlyLateBoundReference<TRuntime> RuntimeReference =>
        this.runtimeReference;

    /// <summary>
    /// Resolves configuration and runs registration hooks without constructing
    /// the runtime or installing its configured endpoint.
    /// </summary>
    /// <returns>
    /// A single-use plan that exposes the resolved configuration and can
    /// construct the runtime later.
    /// </returns>
    public IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime> Prepare(
        Action<TRuntimeIntegrationContext> registration
    )
    {
        if (Interlocked.CompareExchange(ref this.state, STAGE_BUILT, STAGE_CREATED) != STAGE_CREATED)
        {
            throw new InvalidOperationException(
                "The runtime has already been built."
            );
        }

        var session = sessionFactory();
        var preparedRegistration = session.Prepare(runtimeName, registration);

        return new AllureRuntimeRegistrationPlan<TConfiguration, TRuntime>(
            preparedRegistration,
            this.runtimeReference
        );
    }

    const int STAGE_CREATED = 0;
    const int STAGE_BUILT = 1;
}

public class AllureRuntimeBuilder<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext
>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSession<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext
        >
    > sessionFactory
) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntimeIntegrationContext,
        IAllureRuntime<TConfiguration>
    >(runtimeName, sessionFactory)

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
    where TRuntimeIntegrationContext : IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook
    >;

public class AllureRuntimeBuilder(string runtimeName) : AllureRuntimeBuilder<
    AllureConfiguration,
    IAllureRuntimeRegistrationContext,
    IAllureRuntimeRegistrationHook,
    IAllureInProcessEndpointRegistrationContext,
    IAllureInProcessEndpointRegistrationHook,
    IAllureRuntimeIntegrationContext
>(
    runtimeName,
    () => new AllureRuntimeRegistrationSession()
);
