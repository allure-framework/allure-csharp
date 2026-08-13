using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public class AllureTestingPlatformRuntimeBuilder<TConfiguration>(string runtimeName) :
    AllureRuntimeBuilder<
        TConfiguration,
        IAllureTestingPlatformRuntime<TConfiguration>,
        IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration>
    >(
        runtimeName,
        () => new AllureTestingPlatformRuntimeRegistrationSession<TConfiguration>()
    )

    where TConfiguration : AllureTestingPlatformConfiguration, new();

public class AllureTestingPlatformRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRuntimeIntegrationContext
    >(
        runtimeName,
        () => new AllureTestingPlatformRuntimeRegistrationSession()
    );
