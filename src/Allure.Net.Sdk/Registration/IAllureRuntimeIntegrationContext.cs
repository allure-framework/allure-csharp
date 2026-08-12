using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Configures an Allure runtime and its in-process endpoint integration.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The type of runtime constructed by the integration.</typeparam>
/// <typeparam name="TContext">The type of runtime integration context.</typeparam>
public interface IAllureRuntimeIntegrationContext<TConfiguration, TRuntime, out TContext> :
    IAllureRuntimeIntegrationContextBase<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
    where TContext : IAllureRuntimeRegistrationContext<TConfiguration>
{
    /// <summary>
    /// Configures the hooks invoked during runtime registration.
    /// </summary>
    /// <param name="hooksFactory">
    /// A factory that creates the hooks from the initially resolved configuration.
    /// </param>
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<IAllureRegistrationHook<TContext>?>> hooksFactory
    );
}

public interface IAllureRuntimeIntegrationContext<TConfiguration, TRuntime> :
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        IAllureRuntimeRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>;

/// <summary>
/// Configures a standard Allure runtime with a custom configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRuntimeIntegrationContext<TConfiguration> :
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        IAllureRuntime<TConfiguration>
    >

    where TConfiguration : AllureConfiguration;

/// <summary>
/// Configures a standard Allure runtime and its in-process endpoint integration.
/// </summary>
public interface IAllureRuntimeIntegrationContext :
    IAllureRuntimeIntegrationContext<
        AllureConfiguration,
        IAllureRuntime<AllureConfiguration>,
        IAllureRuntimeRegistrationContext
    >;
