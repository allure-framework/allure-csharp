using System;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureIntegrationRegistrationContext<TConfiguration>
    where TConfiguration : AllureConfiguration, new()
{
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
