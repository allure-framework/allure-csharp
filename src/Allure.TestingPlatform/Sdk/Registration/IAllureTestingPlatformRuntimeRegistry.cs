using System;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformRuntimeRegistry
{
    IAllureTestingPlatformRuntimeProvider GetRuntimeProvider(IServiceProvider serviceProvider);
}
