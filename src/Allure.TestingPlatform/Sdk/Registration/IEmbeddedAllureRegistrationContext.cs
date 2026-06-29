using System;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IEmbeddedAllureRegistrationContext : IStandaloneAllureRegistrationContext
{
    IEmbeddedAllureRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationStrategy> correlationStrategyFactory
    );

    IEmbeddedAllureRegistrationContext SetSdkEventHandlers(
        Action<IAllureTestingPlatformSdkEvents> sdkEventHandlersRegistration
    );
}
