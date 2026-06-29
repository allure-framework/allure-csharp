using System;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IEmbeddedAllureRegistrationContext : IStandaloneAllureRegistrationContext
{
    IEmbeddedAllureRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationSource> correlationServiceFactory
    );

    IEmbeddedAllureRegistrationContext SetSdkEventHandlers(
        Action<IAllureTestingPlatformSdkEvents> sdkEventHandlersRegistration
    );
}
