using System;
using System.Collections.Generic;
using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration.Hooks;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureInProcessEndpointIntegrationContext<TConfiguration, THook> :
    IAllureInProcessEndpointRegistrationContext<TConfiguration>
    where TConfiguration : AllureConfiguration
    where THook : IAllureInProcessEndpointRegistrationHook<TConfiguration>
{
    void UseRegistrationHooks(
        Func<TConfiguration, IEnumerable<THook?>> hooksFactory
    );

    void UseCurrentScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    void UseGlobalScopePredicate(Func<IAllureRuntime<TConfiguration>, bool> predicate);

    void UseOperations(Func<IAllureRuntime<TConfiguration>, AllureInProcessOperations> operationsFactory);
}
