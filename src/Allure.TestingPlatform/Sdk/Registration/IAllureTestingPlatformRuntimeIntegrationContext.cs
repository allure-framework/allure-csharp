using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Provides integration operations for an Allure Microsoft Testing Platform runtime with
/// specific runtime and registration-context types.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
/// <typeparam name="TContext">The registration context type.</typeparam>
public interface IAllureTestingPlatformRuntimeIntegrationContext<
    TConfiguration,
    out TRuntime,
    out TContext
> :
    IAllureTestingPlatformRuntimeIntegrationContextBase<TConfiguration, TRuntime>,
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        TContext
    >

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>;

/// <summary>
/// Provides integration operations for an Allure Microsoft Testing Platform runtime with
/// a specific runtime type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureTestingPlatformRuntimeIntegrationContext<
    TConfiguration,
    out TRuntime
> :
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

/// <summary>
/// Provides integration operations for the default runtime with a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration> :
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>,
        IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureTestingPlatformConfiguration;

/// <summary>
/// Provides integration and registration operations for the default Allure Microsoft Testing
/// Platform runtime.
/// </summary>
public interface IAllureTestingPlatformRuntimeIntegrationContext :
    IAllureTestingPlatformRuntimeIntegrationContext<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRuntimeRegistrationContext
    >,
    IAllureTestingPlatformRuntimeRegistrationContext;
