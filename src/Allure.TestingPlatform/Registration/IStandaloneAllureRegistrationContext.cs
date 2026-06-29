using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Registration;

public interface IStandaloneAllureRegistrationContext
{
    IStandaloneAllureRegistrationContext DisableHostProcessWatchdog();

    IStandaloneAllureRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    );

    IStandaloneAllureRegistrationContext UseLogger(
        Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory
    );

    IStandaloneAllureRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    );

    IStandaloneAllureRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    );

    IStandaloneAllureRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    );

    IStandaloneAllureRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory
    );
}
