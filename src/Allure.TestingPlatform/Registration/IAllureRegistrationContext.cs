using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Registration;

public interface IAllureRegistrationContext
{
    IAllureRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    );

    IAllureRegistrationContext UseLogger(
        Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory
    );

    IAllureRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    );

    IAllureRegistrationContext UseCorrelationService(
        Func<IServiceProvider, AllureConfiguration, ICorrelationService> correlationServiceFactory
    );

    IAllureRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    );

    IAllureRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    );

    IAllureRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory
    );
}