using System;
using Allure.Sdk.Registration;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Registration;

public class AllureTestingPlatformRuntimeBuilder<
    TConfiguration,
    TRuntime,
    TIntegrationContext
>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSessionBase<
            TConfiguration,
            TRuntime,
            TIntegrationContext
        >
    > sessionFactory
) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        TIntegrationContext
    >(runtimeName, sessionFactory)

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<
        TConfiguration,
        TRuntime
    >;

public class AllureTestingPlatformRuntimeBuilder<TConfiguration, TRuntime>(
    string runtimeName,
    Func<
        AllureRuntimeRegistrationSessionBase<
            TConfiguration,
            TRuntime,
            IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
        >
    > sessionFactory
) :
    AllureRuntimeBuilder<
        TConfiguration,
        TRuntime,
        IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
    >(runtimeName, sessionFactory)

    where TConfiguration : AllureTestingPlatformConfiguration, new()
    where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>;

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
    )
{
    public static AllureTestingPlatformRuntimeBuilder<
        TConfiguration,
        TRuntime,
        TIntegrationContext
    > Create<TConfiguration, TRuntime, TIntegrationContext>(
        string runtimeName,
        Func<
            AllureRuntimeRegistrationSessionBase<
                TConfiguration,
                TRuntime,
                TIntegrationContext
            >
        > sessionFactory
    )
        where TConfiguration : AllureTestingPlatformConfiguration, new()
        where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
        where TIntegrationContext : IAllureTestingPlatformRuntimeIntegrationContextBase<TConfiguration, TRuntime>
    =>
        new(runtimeName, sessionFactory);

    public static AllureTestingPlatformRuntimeBuilder<TConfiguration, TRuntime> Create<TConfiguration, TRuntime>(
        string runtimeName,
        Func<
            AllureRuntimeRegistrationSessionBase<
                TConfiguration,
                TRuntime,
                IAllureTestingPlatformRuntimeIntegrationContext<TConfiguration, TRuntime>
            >
        > sessionFactory
    )
        where TConfiguration : AllureTestingPlatformConfiguration, new()
        where TRuntime : IAllureTestingPlatformRuntime<TConfiguration>
    =>
        new(runtimeName, sessionFactory);
}
