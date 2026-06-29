using System;
using Allure.Net.Commons.Configuration;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Sdk.Registration;

public interface IEmbeddedRegistrationContext : IStandaloneRegistrationContext
{
    IEmbeddedRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationSource> correlationServiceFactory
    );

    IEmbeddedRegistrationContext SetSdkEventHandlers(
        Action<IAllureTestingPlatformSdkEvents> sdkEventHandlersRegistration
    );
}
