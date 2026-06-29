using System;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeReferenceRegistry
{
    IAllureTestingPlatformRuntimeReference GetRuntimeReference(IServiceProvider serviceProvider);
}
