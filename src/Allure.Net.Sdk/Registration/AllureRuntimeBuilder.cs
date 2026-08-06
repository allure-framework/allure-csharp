using System;
using System.Threading;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Prepares registration of a custom Allure runtime and its optional in-process endpoint.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntimeIntegrationContext">The integration context type.</typeparam>
/// <typeparam name="TIntegrationSnapshot">The integration snapshot type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
/// <param name="runtimeName">The runtime name used to identify its in-process route.</param>
/// <param name="sessionFactory">
/// A factory that creates the single-use registration session configured by
/// <see cref="Prepare"/>.
/// </param>
public class AllureRuntimeBuilder<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TIntegrationSnapshot,
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
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntime
    >
    where TRuntime : IAllureRuntime<TConfiguration>
{
    int state = 0;

    readonly LateBoundReference<TRuntime> runtimeReference = new();

    /// <summary>
    /// Gets a late-bound reference to the runtime that becomes available after
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
    /// <param name="registration">An action that configures the registration session.</param>
    /// <exception cref="InvalidOperationException">
    /// This builder has already been used.
    /// </exception>
    public IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime> Prepare(
        Action<TRuntimeIntegrationContext> registration
    )
    {
        if (Interlocked.CompareExchange(ref this.state, STAGE_CONSUMED, STAGE_CREATED) != STAGE_CREATED)
        {
            throw new InvalidOperationException(
                "The builder was already used."
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
    const int STAGE_CONSUMED = 1;
}

/// <summary>
/// Prepares registrations for a standard Allure runtime with custom configuration,
/// registration, and endpoint types.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntimeRegistrationContext">The runtime registration context type.</typeparam>
/// <typeparam name="TRuntimeHook">The runtime registration hook type.</typeparam>
/// <typeparam name="TEndpointRegistrationContext">The endpoint registration context type.</typeparam>
/// <typeparam name="TEndpointHook">The endpoint registration hook type.</typeparam>
/// <typeparam name="TRuntimeIntegrationContext">The integration context type.</typeparam>
/// <typeparam name="TIntegrationSnapshot">The integration snapshot type.</typeparam>
/// <param name="runtimeName">The runtime name used to identify its in-process route.</param>
/// <param name="sessionFactory">A factory that creates a registration session.</param>
public class AllureRuntimeBuilder<
    TConfiguration,
    TRuntimeRegistrationContext,
    TRuntimeHook,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntimeIntegrationContext,
    TIntegrationSnapshot
>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSession<
            TConfiguration,
            TRuntimeRegistrationContext,
            TRuntimeHook,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntimeIntegrationContext,
            TIntegrationSnapshot
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
        TIntegrationSnapshot,
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
    >
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook
    >;

/// <summary>
/// Prepares registrations for a standard Allure runtime and its optional in-process
/// endpoint.
/// </summary>
/// <param name="runtimeName">The runtime name used to identify its in-process route.</param>
public class AllureRuntimeBuilder(string runtimeName) : AllureRuntimeBuilder<
    AllureConfiguration,
    IAllureRuntimeRegistrationContext,
    IAllureRuntimeRegistrationHook,
    IAllureInProcessEndpointRegistrationContext,
    IAllureInProcessEndpointRegistrationHook,
    IAllureRuntimeIntegrationContext,
    IAllureRuntimeIntegrationSnapshot
>(
    runtimeName,
    () => new AllureRuntimeRegistrationSession()
);
