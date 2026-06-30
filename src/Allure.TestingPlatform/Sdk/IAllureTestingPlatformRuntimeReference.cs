using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk;

/// <summary>
/// Provides access to the current Allure.TestingPlatform runtime state.
/// </summary>
public interface IAllureTestingPlatformRuntimeReference
{
    /// <summary>
    /// Gets the current runtime state.
    /// </summary>
    AllureTestingPlatformRuntimeState CurrentRuntime { get; }
}
