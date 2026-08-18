using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Registration;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Provides an Allure Microsoft Testing Platform runtime with a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="commonArgs">The common Allure runtime arguments.</param>
/// <param name="testingPlatformArgs">
/// The Microsoft Testing Platform specific arguments.
/// </param>
public class AllureTestingPlatformRuntime<TConfiguration>(
    RuntimeCreationArguments<TConfiguration> commonArgs,
    AllureTestingPlatformRuntimeArguments testingPlatformArgs
) :
    AllureRuntime<TConfiguration>(
        commonArgs.Configuration,
        commonArgs.ParameterSerializer,
        commonArgs.Destination,
        commonArgs.Context,
        commonArgs.LifecycleApi,
        commonArgs.ModelApi
    ),
    IAllureTestingPlatformRuntime<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration
{
    /// <inheritdoc />
    public ILogger Logger => testingPlatformArgs.Logger;

    /// <inheritdoc />
    public ICorrelationStrategy CorrelationStrategy =>
        testingPlatformArgs.CorrelationStrategy;

    /// <inheritdoc />
    public ICorrelationContext CorrelationContext =>
        testingPlatformArgs.CorrelationContext;

    /// <inheritdoc />
    public ExecutionStateContext ExecutionStateContext =>
        testingPlatformArgs.ExecutionStateContext;
}

/// <summary>
/// Provides the default Allure Microsoft Testing Platform runtime.
/// </summary>
/// <param name="commonArgs">The common Allure runtime arguments.</param>
/// <param name="testingPlatformArgs">
/// The Microsoft Testing Platform specific arguments.
/// </param>
public class AllureTestingPlatformRuntime(
    RuntimeCreationArguments<AllureTestingPlatformConfiguration> commonArgs,
    AllureTestingPlatformRuntimeArguments testingPlatformArgs
) :
    AllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>(
        commonArgs,
        testingPlatformArgs
    ),
    IAllureTestingPlatformRuntime;
