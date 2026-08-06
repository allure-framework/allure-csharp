using Allure.Abstractions;
using Allure.Sdk.Results;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides the resolved components used to construct an Allure runtime.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="Configuration">The resolved runtime configuration.</param>
/// <param name="ParameterSerializer">The configured parameter serializer.</param>
/// <param name="Destination">The destination that receives generated Allure results.</param>
/// <param name="Context">The execution-context service.</param>
/// <param name="LifecycleApi">The lifecycle API service.</param>
/// <param name="ModelApi">The model API service.</param>
public record class RuntimeCreationArguments<TConfiguration>(
    TConfiguration Configuration,
    IAllureParameterSerializer ParameterSerializer,
    IAllureResultsDestination Destination,
    IAllureExecutionContext Context,
    IAllureLifecycleApi LifecycleApi,
    IAllureModelApi ModelApi
);
