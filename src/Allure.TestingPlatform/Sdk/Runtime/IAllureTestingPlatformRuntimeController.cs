namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Controls Allure.TestingPlatform runtime configuration and startup.
/// </summary>
public interface IAllureTestingPlatformRuntimeController
{
    /// <summary>
    /// Gets the runtime reference controlled by this controller.
    /// </summary>
    public IAllureTestingPlatformRuntimeReference RuntimeReference { get; }

    /// <summary>
    /// Configures the runtime if it has not been configured yet.
    /// </summary>
    AllureTestingPlatformRuntimeState Configure();

    /// <summary>
    /// Starts the runtime if it is configured and enabled.
    /// </summary>
    AllureTestingPlatformRuntimeState Start();
}
