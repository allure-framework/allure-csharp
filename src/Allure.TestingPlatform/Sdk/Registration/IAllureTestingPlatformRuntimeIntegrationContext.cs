using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

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

public interface IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration> :
    IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>,
        IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    >

    where TConfiguration : AllureTestingPlatformConfiguration;

public interface IAllureTestingPlatformRuntimeIntegrationContext :
    IAllureTestingPlatformRuntimeIntegrationContext<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRuntimeRegistrationContext
    >,
    IAllureTestingPlatformRuntimeRegistrationContext;
