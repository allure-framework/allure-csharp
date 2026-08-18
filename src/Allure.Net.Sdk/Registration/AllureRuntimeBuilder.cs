using System;
using System.Threading;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Prepares registration of a custom Allure runtime and its optional in-process endpoint.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
/// <typeparam name="TIntegrationContext">
/// The integration context type passed to the registration action.
/// </typeparam>
/// <param name="runtimeName">
/// The runtime name assigned to its in-process endpoint.
/// </param>
/// <param name="sessionFactory">
/// A factory that creates the single-use registration session configured by
/// <see cref="Prepare"/>.
/// </param>
public class AllureRuntimeBuilder<TConfiguration, TRuntime, TIntegrationContext>(
    string runtimeName,
    Func<AllureRuntimeRegistrationSessionBase<TConfiguration, TRuntime, TIntegrationContext>> sessionFactory
)

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
    where TIntegrationContext : IAllureRuntimeIntegrationContextBase<TConfiguration, TRuntime>
{
    int state = 0;

    readonly LateBoundReference<TConfiguration> configurationReference = new();

    readonly LateBoundReference<TRuntime> runtimeReference = new();

    /// <summary>
    /// Gets a late-bound reference to the configuration that becomes available after
    /// <see cref="Prepare"/> is called.
    /// </summary>
    public IReadOnlyLateBoundReference<TConfiguration> ConfigurationReference =>
        this.configurationReference;

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
        Action<TIntegrationContext> registration
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

        this.configurationReference.Bind(preparedRegistration.Configuration);

        return new AllureRuntimeRegistrationPlan<TConfiguration, TRuntime>(
            preparedRegistration,
            this.runtimeReference
        );
    }

    const int STAGE_CREATED = 0;
    const int STAGE_CONSUMED = 1;
}

/// <summary>
/// Prepares registration of a custom Allure runtime using the standard
/// integration and registration contexts.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
/// <param name="runtimeName">
/// The runtime name assigned to its in-process endpoint.
/// </param>
/// <param name="sessionFactory">A factory that creates a registration session.</param>
public class AllureRuntimeBuilder<TConfiguration, TRuntime>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSession<
            TConfiguration,
            TRuntime
        >
    > sessionFactory
) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        IAllureRuntimeIntegrationContext<TConfiguration, TRuntime>
    >(
        runtimeName,
        sessionFactory
    )

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>;

/// <summary>
/// Prepares registration of a standard Allure runtime with a custom
/// configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="runtimeName">
/// The runtime name assigned to its in-process endpoint.
/// </param>
public class AllureRuntimeBuilder<TConfiguration>(string runtimeName) :
    AllureRuntimeBuilder<
        TConfiguration,
        IAllureRuntime<TConfiguration>,
        IAllureRuntimeIntegrationContext<TConfiguration>
    >(
        runtimeName,
        () => new AllureRuntimeRegistrationSession<TConfiguration>()
    )

    where TConfiguration : AllureConfiguration, new();

/// <summary>
/// Prepares registration of a standard Allure runtime and its optional
/// in-process endpoint.
/// </summary>
/// <param name="runtimeName">
/// The runtime name assigned to its in-process endpoint.
/// </param>
public class AllureRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        AllureConfiguration,
        IAllureRuntime,
        IAllureRuntimeIntegrationContext
    >(
        runtimeName,
        () => new AllureRuntimeRegistrationSession()
    )
{
    /// <summary>
    /// Creates the runtime builder from the name and session factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
    /// <typeparam name="TIntegrationContext">
    /// The integration context type passed to the registration action.
    /// </typeparam>
    /// <param name="runtimeName">
    /// The runtime name assigned to its in-process endpoint.
    /// </param>
    /// <param name="sessionFactory">
    /// A factory that creates the single-use registration session configured by
    /// <see cref="AllureRuntimeBuilder{TConfiguration,TRuntime,TIntegrationContext}.Prepare(Action{TIntegrationContext})"/>.
    /// </param>
    /// <returns>A single-use builder that constructs the runtime.</returns>
    public static AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        TIntegrationContext
    > Create<TConfiguration, TRuntime, TIntegrationContext>(
        string runtimeName,
        Func<
            AllureRuntimeRegistrationSessionBase<
                TConfiguration,
                TRuntime,
                TIntegrationContext
            >
        > sessionFactory
    )
        where TConfiguration : AllureConfiguration, new()
        where TRuntime : IAllureRuntime<TConfiguration>
        where TIntegrationContext : IAllureRuntimeIntegrationContextBase<TConfiguration, TRuntime>
    =>
        new(runtimeName, sessionFactory);

    /// <summary>
    /// Creates the runtime builder from the name and session factory.
    /// </summary>
    /// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
    /// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
    /// <param name="runtimeName">
    /// The runtime name assigned to its in-process endpoint.
    /// </param>
    /// <param name="sessionFactory">
    /// A factory that creates the single-use registration session configured by
    /// <see cref="AllureRuntimeBuilder{TConfiguration,TRuntime,TIntegrationContext}.Prepare(Action{TIntegrationContext})"/>.
    /// </param>
    /// <returns>A single-use builder for the standard context that constructs the runtime.</returns>
    public static AllureRuntimeBuilder<TConfiguration, TRuntime> Create<TConfiguration, TRuntime>(
        string runtimeName,
        Func<
            AllureRuntimeRegistrationSession<
                TConfiguration,
                TRuntime
            >
        > sessionFactory
    )
        where TConfiguration : AllureConfiguration, new()
        where TRuntime : IAllureRuntime<TConfiguration>
    =>
        new(runtimeName, sessionFactory);
}
