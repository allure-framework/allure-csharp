using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public interface IAllureRegistrationDependencies<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    TConfiguration Configuration { get; }

    IAllureParameterSerializer ParameterSerializer { get; }

    ILateBoundReferenceView<IAllureRuntime<TConfiguration>> RuntimeReference { get; }
}
