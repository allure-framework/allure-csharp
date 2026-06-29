using System;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRegistrationResult
{
    IAllureTestingPlatformServiceProvider GetProvider(IServiceProvider serviceProvider);
}
