using Allure.Abstractions;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

public record class RuntimeCreationArguments<TConfiguration>(
    TConfiguration Configuration,
    IAllureParameterSerializer ParameterSerializer,
    IAllureResultsDestination Destination,
    IAllureExecutionContext Context,
    IAllureLifecycleApi LifecycleApi,
    IAllureModelApi ModelApi
);
