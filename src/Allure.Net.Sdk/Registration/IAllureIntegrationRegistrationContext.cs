using System;
using System.Collections.Generic;
using Allure.Sdk.Configuration;
using Allure.Sdk.Extensions;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureIntegrationRegistrationContext<TConfiguration, THook>
    where TConfiguration : AllureConfiguration, new()
    where THook : IAllureRuntimeRegistrationHook<TConfiguration>
{
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<IAllureRuntimeRegistrationHookProvider<TConfiguration, THook>>> hookProvidersFactory
    );

    void UseContext(
        Func<IAllureRegistrationDependencies<TConfiguration>, IAllureRuntimeContext> contextFactory
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
