using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

class AllureRuntime<TConfiguration>(
    TConfiguration configuration,
    IAllureParameterSerializer parameterSerializer,
    IAllureResultsDestination resultsDestination,
    IAllureExecutionContext context,
    IAllureLifecycleApi lifecycleApi,
    IAllureModelApi modelApi
) :
    IAllureRuntime<TConfiguration>

    where TConfiguration : AllureConfiguration
{
    AllureConfiguration IAllureRuntime.Configuration => this.Configuration;

    public TConfiguration Configuration { get; } = configuration;

    public IAllureParameterSerializer ParameterSerializer { get; } = parameterSerializer;

    public IAllureResultsDestination ResultsDestination { get; } = resultsDestination;

    public IAllureExecutionContext ContextApi { get; } = context;

    public IAllureLifecycleApi LifecycleApi { get; } = lifecycleApi;

    public IAllureModelApi ModelApi { get; } = modelApi;
}
