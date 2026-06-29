using System;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformPreparedRegistration(
    AllureTestingPlatformRegistrationInput input
)
{
    public bool HostProcessWatchdogEnabled => input.HostProcessWatchdogEnabled;

    public AllureTestingPlatformRuntimeReferenceRegistry RuntimeReferences { get; } = new();

    public AllureTestingPlatformRuntimeController CreateController(IServiceProvider serviceProvider) =>
        new(
            input: input,
            serviceProvider: serviceProvider,
            runtimeReference:
                this.RuntimeReferences.GetRuntimeReference(serviceProvider)
        );
}
