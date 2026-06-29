using System;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeRegistry
{
    IAllureTestingPlatformRuntimeProvider GetRuntimeProvider(IServiceProvider serviceProvider);
}
