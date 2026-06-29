using System;
using System.Runtime.CompilerServices;
using Allure.TestingPlatform.Sdk.Registration;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformServiceMapper : IAllureTestingPlatformRegistrationResult
{
    readonly ConditionalWeakTable<IServiceProvider, AllureTestingPlatformServiceProvider> states = new();

    IAllureTestingPlatformServiceProvider IAllureTestingPlatformRegistrationResult.GetProvider(IServiceProvider serviceProvider) =>
        this.GetProvider(serviceProvider);

    public AllureTestingPlatformServiceProvider GetProvider(IServiceProvider serviceProvider) =>
        this.states.GetOrCreateValue(serviceProvider);
}
