using System;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Defines the base contract for a single-use runtime registration session.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
/// <typeparam name="TIntegrationContext">
/// The runtime integration context type passed to the registration action.
/// </typeparam>
public abstract class AllureRuntimeRegistrationSessionBase<TConfiguration, TRuntime, TIntegrationContext>

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
    where TIntegrationContext : IAllureRuntimeIntegrationContextBase<TConfiguration, TRuntime>
{
    internal abstract IPreparedRuntimeRegistration<TConfiguration, TRuntime> Prepare(
        string runtimeName,
        Action<TIntegrationContext> registration
    );
}
