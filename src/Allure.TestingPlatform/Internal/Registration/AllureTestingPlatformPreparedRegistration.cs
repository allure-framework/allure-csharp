using System;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformPreparedRegistration(
    AllureTestingPlatformRegistrationInput input
)
{
    public AllureTestingPlatformServiceMapper RegistrationResults { get; } = new();

    public AllureTestingPlatformBuilder CreateBuilder(IServiceProvider serviceProvider) =>
        new(
            input: input,
            serviceProvider: serviceProvider,
            allureStateProvider:
                this.RegistrationResults.GetProvider(serviceProvider)
        );
}
