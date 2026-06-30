using System;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Provides runtime references for Microsoft Testing Platform test applications
/// denoted by <see cref="IServiceProvider"/> .
/// </summary>
public interface IAllureTestingPlatformRuntimeReferenceRegistry
{
    /// <summary>
    /// Gets the runtime reference associated with the test application
    /// denoted by the specified service provider.
    /// </summary>
    IAllureTestingPlatformRuntimeReference GetRuntimeReference(IServiceProvider serviceProvider);
}
