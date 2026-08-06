using System;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal record class AllureInProcessEndpointRegistration<
    TConfiguration,
    TEndpointRegistrationContext,
    TEndpointHook,
    TRuntime
>(
    string Id,
    Action<
        TRuntime,
        IAllureInProcessEndpointIntegrationContext<
            TConfiguration,
            TEndpointRegistrationContext,
            TEndpointHook,
            TRuntime
        >
    > Registration
)
    where TConfiguration : AllureConfiguration
    where TEndpointRegistrationContext : IAllureInProcessEndpointRegistrationContext<TConfiguration, TRuntime>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration, TEndpointRegistrationContext, TRuntime>
    where TRuntime : IAllureRuntime<TConfiguration>;
