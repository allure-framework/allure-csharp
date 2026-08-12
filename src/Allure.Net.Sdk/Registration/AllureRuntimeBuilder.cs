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
/// <typeparam name="TRuntime">The type of runtime constructed by the builder.</typeparam>
/// <typeparam name="TRegistrationContext">The registration context type.</typeparam>
/// <typeparam name="TIntegrationContext">The integration context type.</typeparam>
/// <param name="runtimeName">The runtime name used to identify its in-process route.</param>
/// <param name="sessionFactory">
/// A factory that creates the single-use registration session configured by
/// <see cref="Prepare"/>.
/// </param>
public class AllureRuntimeBuilder<TConfiguration, TRuntime, TRegistrationContext, TIntegrationContext>(
    string runtimeName,
    Func<AllureRuntimeRegistrationSession<TConfiguration, TRuntime, TRegistrationContext, TIntegrationContext>> sessionFactory
)

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
    where TRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>
    where TIntegrationContext : IAllureRuntimeIntegrationContext<TConfiguration, TRuntime, TRegistrationContext>
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

        return new AllureRuntimeRegistrationPlan<TConfiguration, TRuntime>(
            preparedRegistration,
            this.runtimeReference
        );
    }

    const int STAGE_CREATED = 0;
    const int STAGE_CONSUMED = 1;
}

public class AllureRuntimeBuilder<TConfiguration, TRuntime, TRegistrationContext>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSession<
            TConfiguration,
            TRuntime,
            TRegistrationContext,
            IAllureRuntimeIntegrationContext<TConfiguration, TRuntime, TRegistrationContext>
        >
    > sessionFactory
) :
    AllureRuntimeBuilder<TConfiguration, TRuntime, TRegistrationContext, IAllureRuntimeIntegrationContext<TConfiguration, TRuntime, TRegistrationContext>>(
        runtimeName,
        sessionFactory
    )

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
    where TRegistrationContext : IAllureRuntimeRegistrationContext<TConfiguration>;

public class AllureRuntimeBuilder<TConfiguration, TRuntime>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSession<
            TConfiguration,
            TRuntime,
            IAllureRuntimeRegistrationContext<TConfiguration>,
            IAllureRuntimeIntegrationContext<TConfiguration, TRuntime>
        >
    > sessionFactory
) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        IAllureRuntimeRegistrationContext<TConfiguration>,
        IAllureRuntimeIntegrationContext<
            TConfiguration,
            TRuntime
        >
    >(
        runtimeName,
        sessionFactory
    )

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>;

public class AllureRuntimeBuilder<TConfiguration>(string runtimeName) :
    AllureRuntimeBuilder<
        TConfiguration,
        IAllureRuntime<TConfiguration>,
        IAllureRuntimeRegistrationContext<TConfiguration>,
        IAllureRuntimeIntegrationContext<TConfiguration>
    >(
        runtimeName,
        () => new AllureRuntimeRegistrationSession<TConfiguration>()
    )

    where TConfiguration : AllureConfiguration, new();

public class AllureRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        AllureConfiguration,
        IAllureRuntime<AllureConfiguration>,
        IAllureRuntimeRegistrationContext<AllureConfiguration>,
        IAllureRuntimeIntegrationContext
    >(
        runtimeName,
        () => new AllureRuntimeRegistrationSession()
    );
