using System;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal record class AllureInProcessEndpointRegistration<TConfiguration, TRuntime>(
    string Id,
    Action<
        TRuntime,
        IAllureInProcessEndpointIntegrationContext<TRuntime>
    > Registration
)
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>;
