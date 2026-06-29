using System;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformSdkEvents
{
    event Action<ConfiguredAllureTestingPlatformRuntime> OnConfigured;

    event Action<LiveAllureTestingPlatformRuntime> OnLive;
}
