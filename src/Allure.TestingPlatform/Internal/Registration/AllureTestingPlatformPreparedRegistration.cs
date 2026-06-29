using System;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformPreparedRegistration(
    AllureTestingPlatformRegistrationInput input
)
{
    public bool HostProcessWatchdogEnabled => input.HostProcessWatchdogEnabled;

    public AllureTestingPlatformRuntimeRegistry RegistrationResults { get; } = new();

    public AllureTestingPlatformRuntimeBuilder CreateBuilder(IServiceProvider serviceProvider) =>
        new(
            input: input,
            serviceProvider: serviceProvider,
            runtimeProvider:
                this.RegistrationResults.GetRuntimeProvider(serviceProvider)
        );
}
