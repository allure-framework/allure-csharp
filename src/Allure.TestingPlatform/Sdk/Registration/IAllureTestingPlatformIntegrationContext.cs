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
public interface IAllureTestingPlatformIntegrationContext<
    TConfiguration,
    out TRuntime,
    out TContext
> :
    IAllureTestingPlatformIntegrationContextBase<TConfiguration, TRuntime>,
    IAllureRuntimeIntegrationContext<
        TConfiguration,
        TRuntime,
        TContext
    >

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TContext : IAllureTestingPlatformRegistrationContext<TConfiguration>;

/// <summary>
/// Provides integration operations for an Allure Microsoft Testing Platform runtime with
/// a specific runtime type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureTestingPlatformIntegrationContext<
    TConfiguration,
    out TRuntime
> :
    IAllureTestingPlatformIntegrationContext<
        TConfiguration,
        TRuntime,
        IAllureTestingPlatformRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureTestingPlatformConfiguration
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

/// <summary>
/// Provides integration operations for the default runtime with a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureTestingPlatformIntegrationContext<TConfiguration> :
    IAllureTestingPlatformIntegrationContext<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>,
        IAllureTestingPlatformRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureTestingPlatformConfiguration;

/// <summary>
/// Provides integration and registration operations for the default Allure Microsoft Testing
/// Platform runtime.
/// </summary>
public interface IAllureTestingPlatformIntegrationContext :
    IAllureTestingPlatformIntegrationContext<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRegistrationContext
    >,
    IAllureTestingPlatformRegistrationContext;
