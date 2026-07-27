using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureIntegrationRegistrationContext<TConfiguration, TRuntimeHook, TEndpointHook> :
    IAllureRuntimeRegistrationContext<TConfiguration>

    where TConfiguration : AllureConfiguration, new()
    where TRuntimeHook : IAllureRuntimeRegistrationHook<TConfiguration>
    where TEndpointHook : IAllureInProcessEndpointRegistrationHook<TConfiguration>
{
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<TRuntimeHook?>> hooksFactory
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
        Action<IAllureRuntime<TConfiguration>, IAllureInProcessEndpointRegistrationContext<TConfiguration, TEndpointHook>> endpointRegistration
    );
}
