using System;
using System.Runtime.CompilerServices;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Registration;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRuntimeReferenceRegistry : IAllureTestingPlatformRuntimeReferenceRegistry
{
    readonly ConditionalWeakTable<IServiceProvider, AllureTestingPlatformRuntimeReference> registry = new();

    IAllureTestingPlatformRuntimeReference IAllureTestingPlatformRuntimeReferenceRegistry.GetRuntimeReference(
        IServiceProvider serviceProvider
    ) =>
        this.GetRuntimeReference(serviceProvider);

    public AllureTestingPlatformRuntimeReference GetRuntimeReference(IServiceProvider serviceProvider) =>
        this.registry.GetOrCreateValue(serviceProvider);
}
