using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

/// <summary>
/// Builds a default Allure Microsoft Testing Platform runtime with a specific configuration type.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <param name="runtimeName">The name used to identify the runtime registration.</param>
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

/// <summary>
/// Builds the default Allure Microsoft Testing Platform runtime.
/// </summary>
/// <param name="runtimeName">The name used to identify the runtime registration.</param>
public class AllureTestingPlatformRuntimeBuilder(string runtimeName) :
    AllureRuntimeBuilder<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime,
        IAllureTestingPlatformRuntimeIntegrationContext
    >(
        runtimeName,
        () => new AllureTestingPlatformRuntimeRegistrationSession()
    );
