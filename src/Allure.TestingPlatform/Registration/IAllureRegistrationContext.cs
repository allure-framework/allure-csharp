using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform.Registration;

public interface IAllureRegistrationContext
{
    IAllureRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    );

    IAllureRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
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