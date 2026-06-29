using System;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRegistrationResult
{
    IAllureTestingPlatformServiceProvider GetProvider(IServiceProvider serviceProvider);
}
