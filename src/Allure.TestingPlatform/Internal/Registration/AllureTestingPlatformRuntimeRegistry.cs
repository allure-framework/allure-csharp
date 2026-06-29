using System;
using System.Runtime.CompilerServices;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRuntimeRegistry : IAllureTestingPlatformRuntimeRegistry
{
    readonly ConditionalWeakTable<IServiceProvider, AllureTestingPlatformRuntimeProvider> registry = new();

    IAllureTestingPlatformRuntimeProvider IAllureTestingPlatformRuntimeRegistry.GetRuntimeProvider(
        IServiceProvider serviceProvider
    ) =>
        this.GetRuntimeProvider(serviceProvider);

    public AllureTestingPlatformRuntimeProvider GetRuntimeProvider(IServiceProvider serviceProvider) =>
        this.registry.GetOrCreateValue(serviceProvider);
}
