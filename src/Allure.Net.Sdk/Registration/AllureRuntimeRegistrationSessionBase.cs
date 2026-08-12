using System;
using Allure.Sdk.Configuration;
using Allure.Sdk.Internal.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public abstract class AllureRuntimeRegistrationSessionBase<TConfiguration, TRuntime, TIntegrationContext>

    where TConfiguration : AllureConfiguration, new()
    where TRuntime : IAllureRuntime<TConfiguration>
    where TIntegrationContext : IAllureRuntimeIntegrationContextBase<TConfiguration, TRuntime>
{
    internal abstract IPreparedRuntimeRegistration<TConfiguration, TRuntime> Prepare(
        string runtimeName,
        Action<TIntegrationContext> registration
    );
}
