namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Defines the lifecycle phase of the Allure.TestingPlatform runtime.
/// </summary>
public enum AllureTestingPlatformRuntimePhase
{
    /// <summary>
    /// The runtime has not been configured yet.
    /// </summary>
    NotInitialized,

    /// <summary>
    /// The runtime is unavailable because registration was suppressed
    /// via <c>--allure off</c>.
    /// </summary>
    Suppressed,

    /// <summary>
    /// The runtime was configured but Allure is disabled.
    /// </summary>
    Disabled,

    /// <summary>
    /// The runtime is configured and ready to start.
    /// </summary>
    Configured,

    /// <summary>
    /// The runtime is started and ready to process data.
    /// </summary>
    Live,
}
