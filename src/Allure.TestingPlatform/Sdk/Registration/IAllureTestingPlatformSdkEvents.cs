using System;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IAllureTestingPlatformSdkEvents
{
    event Action<ConfiguredAllureTestingPlatform> OnConfigured;

    event Action<ReadyAllureTestingPlatform> OnReady;
}
