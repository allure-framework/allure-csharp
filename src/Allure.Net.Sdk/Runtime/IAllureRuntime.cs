using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;

namespace Allure.Sdk.Runtime;

public interface IAllureRuntime
{
    AllureConfiguration Configuration { get; }

    IAllureExecutionContext ContextApi { get; }

    IAllureLifecycleApi LifecycleApi { get; }

    IAllureModelApi ModelApi { get; }

    IAllureResultsDestination ResultsDestination { get; }

    IAllureParameterSerializer ParameterSerializer { get; }
}

public interface IAllureRuntime<out TConfiguration> : IAllureRuntime
    where TConfiguration : AllureConfiguration
{
    new TConfiguration Configuration { get; }
}
