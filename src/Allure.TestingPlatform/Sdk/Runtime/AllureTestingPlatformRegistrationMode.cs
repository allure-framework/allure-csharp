namespace Allure.TestingPlatform.Sdk.Runtime;

/// <summary>
/// Defines how Allure.TestingPlatform is registered.
/// </summary>
public enum AllureTestingPlatformRegistrationMode
{
    /// <summary>
    /// Registers Allure as the primary result producer.
    /// </summary>
    Standalone,

    /// <summary>
    /// Registers Allure for use by another Allure adapter.
    /// </summary>
    Embedded,
}
