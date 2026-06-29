using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Registration;
using Microsoft.Testing.Platform.Logging;

public interface IStandaloneRegistrationContext
{
    IStandaloneRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    );

    IStandaloneRegistrationContext UseLogger(
        Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory
    );

    IStandaloneRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    );

    IStandaloneRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    );

    IStandaloneRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    );

    IStandaloneRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory
    );
}
