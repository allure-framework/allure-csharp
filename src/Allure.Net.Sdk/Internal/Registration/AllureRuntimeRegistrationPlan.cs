using System;
using System.Threading;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Registration;

sealed class AllureRuntimeRegistrationPlan<TConfiguration, TRuntime>(
    IPreparedRuntimeRegistration<TConfiguration, TRuntime> preparedRegistration,
    LateBoundReference<TRuntime> runtimeReference
) :
    IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime>

    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    int state = 0;

    public TConfiguration Configuration => preparedRegistration.Configuration;

    public IReadOnlyLateBoundReference<TRuntime> RuntimeReference => runtimeReference;

    public IAllureRuntimeRegistration<TRuntime> Build()
    {
        if (Interlocked.CompareExchange(ref this.state, STATE_BUILDING, STATE_PREPARED) != STATE_PREPARED)
        {
            throw new InvalidOperationException(
                "This plan has already been used."
            );
        }

        var registration = preparedRegistration.Build();

        runtimeReference.Bind(registration.Runtime);

        Volatile.Write(ref this.state, STATE_BUILT);
        return registration;
    }

    const int STATE_PREPARED = 0;
    const int STATE_BUILDING = 1;
    const int STATE_BUILT = 2;
}
