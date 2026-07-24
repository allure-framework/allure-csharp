using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

sealed class AllureRegistrationDependencies<TConfiguration>(
    TConfiguration configuration,
    IAllureParameterSerializer parameterSerializer,
    LateBoundReference<IAllureRuntime<TConfiguration>> runtimeReference
) :
    IAllureRegistrationDependencies<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    public TConfiguration Configuration => configuration;

    public ILateBoundReferenceView<IAllureRuntime<TConfiguration>> RuntimeReference => runtimeReference;

    public IAllureParameterSerializer ParameterSerializer => parameterSerializer;

    public void BindRuntime(IAllureRuntime<TConfiguration> runtime)
    {
        runtimeReference.Bind(runtime);
    }
}
