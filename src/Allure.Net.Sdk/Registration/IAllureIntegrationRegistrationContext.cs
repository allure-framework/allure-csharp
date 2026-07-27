using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureIntegrationRegistrationContext<TConfiguration, THook>
    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRegistrationHook<TConfiguration>
{
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<IAllureRegistrationHookProvider<TConfiguration, THook>>> hookProvidersFactory
    );

    void UseContext(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureExecutionContext> contextFactory
    );

    void UseLifecycleApi(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureLifecycleApi> lifecycleApiFactory
    );

    void UseModelApi(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureModelApi> modelApiFactory
    );

    public void RegisterInProcessEndpoint(
        string endpointId,
        Action<IAllureRuntime<TConfiguration>, IAllureInProcessEndpointRegistrationContext<TConfiguration>> endpointRegistration
    );
}
