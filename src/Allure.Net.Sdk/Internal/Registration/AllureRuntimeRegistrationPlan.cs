using System;
using System.Threading;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

sealed class AllureRuntimeRegistrationPlan<TConfiguration, TRuntime>(
    TConfiguration configuration,
    Func<TConfiguration, IAllureRuntimeRegistration<TRuntime>> runtimeFactory
) :
    IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    readonly LateBoundReference<IAllureRuntimeRegistration<TRuntime>> runtimeReference = new();

    int built = 0;

    public TConfiguration Configuration => configuration;

    public IReadOnlyLateBoundReference<IAllureRuntimeRegistration<TRuntime>> RegistrationReference =>
        this.runtimeReference;

    public IAllureRuntimeRegistration<TRuntime> Build()
    {
        if (Interlocked.Exchange(ref this.built, 1) != 0)
        {
            throw new InvalidOperationException(
                "The runtime has already been built."
            );
        }

        var runtimeRegistration = runtimeFactory(this.Configuration);
        this.runtimeReference.Bind(runtimeRegistration);
        return runtimeRegistration;
    }
}
