using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Exposes services specific to an Allure Microsoft Testing Platform runtime.
/// </summary>
public interface IAllureTestingPlatformRuntimeBase :
    IAllureRuntimeBase
{
    /// <summary>
    /// Gets the runtime logger.
    /// </summary>
    ILogger Logger { get; }

    /// <summary>
    /// Gets the strategy used to correlate Microsoft Testing Platform messages with Allure messages.
    /// </summary>
    ICorrelationStrategy CorrelationStrategy { get; }

    /// <summary>
    /// Gets the context that provides the current correlation identifier.
    /// </summary>
    ICorrelationContext CorrelationContext { get; }

    /// <summary>
    /// Gets the context that tracks the current Allure execution state.
    /// </summary>
    ExecutionStateContext ExecutionStateContext { get; }
}

/// <summary>
/// Represents an Allure Microsoft Testing Platform runtime with a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureTestingPlatformRuntime<out TConfiguration> :
    IAllureTestingPlatformRuntimeBase,
    IAllureRuntime<TConfiguration>

    where TConfiguration : AllureTestingPlatformConfiguration;

/// <summary>
/// Represents the default Allure Microsoft Testing Platform runtime.
/// </summary>
public interface IAllureTestingPlatformRuntime :
    IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>;
