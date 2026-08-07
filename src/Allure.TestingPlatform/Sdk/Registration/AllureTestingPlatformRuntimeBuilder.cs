using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public class AllureTestingPlatformRuntimeBuilder<
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
) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook,
        TRuntimeIntegrationContext,
        TIntegrationSnapshot,
        TRuntime
    >(runtimeName, sessionFactory)

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
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
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

public class AllureTestingPlatformRuntimeBuilder<
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
            TRuntimeIntegrationContext,
            IAllureTestingPlatformRuntime<TConfiguration>
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
        IAllureTestingPlatformRuntime<TConfiguration>
    >(runtimeName, sessionFactory)

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntimeRegistrationContext : IAllureTestingPlatformRuntimeRegistrationContext<TConfiguration>
    where TRuntimeHook : IAllureTestingPlatformRuntimeRegistrationHook<TConfiguration, TRuntimeRegistrationContext>
    where TEndpointRegistrationContext : IAllureTestingPlatformEndpointRegistrationContext<TConfiguration>
    where TEndpointHook : IAllureTestingPlatformEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext>
    where TRuntimeIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContext<
        TConfiguration,
        TRuntimeRegistrationContext,
        TRuntimeHook,
        TEndpointRegistrationContext,
        TEndpointHook
    >
    where TIntegrationSnapshot : IAllureRuntimeIntegrationSnapshot<
        TConfiguration,
        TEndpointRegistrationContext,
        TEndpointHook,
        IAllureTestingPlatformRuntime<TConfiguration>
    >;

public class AllureTestingPlatformRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntimeRegistrationContext,
        IAllureTestingPlatformRuntimeRegistrationHook,
        IAllureTestingPlatformEndpointRegistrationContext,
        IAllureTestingPlatformEndpointRegistrationHook,
        IAllureTestingPlatformRuntimeIntegrationContext,
        IAllureRuntimeIntegrationSnapshot<
            AllureTestingPlatformConfiguration,
            IAllureTestingPlatformEndpointRegistrationContext,
            IAllureTestingPlatformEndpointRegistrationHook,
            IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
        >,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >(runtimeName, () => new AllureTestingPlatformRuntimeRegistrationSession());
