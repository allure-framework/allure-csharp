using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;

namespace Allure.TestingPlatform.Registration;

public interface IAllureRegistrationContext
{
    IAllureRegistrationContext SetEnabled(bool enabled);

    IAllureRegistrationContext UseConfiguration(
        Func<AllureConfiguration> configurationFactory);

    IAllureRegistrationContext UseWriter(
        Func<AllureConfiguration, IAllureResultsWriter> writerFactory);

    IAllureRegistrationContext UseLifecycle(
        Func<AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory);

    IAllureRegistrationContext UseTypeFormatters(
        Func<AllureConfiguration, Dictionary<Type, ITypeFormatter>> typeFormattersFactory);
}