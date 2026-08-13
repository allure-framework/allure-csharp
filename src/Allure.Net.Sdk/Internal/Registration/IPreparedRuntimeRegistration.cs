using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

internal interface IPreparedRuntimeRegistration<out TConfiguration, out TRuntime>
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    TConfiguration Configuration { get; }

    IAllureRuntimeRegistration<TRuntime> Build();
}
