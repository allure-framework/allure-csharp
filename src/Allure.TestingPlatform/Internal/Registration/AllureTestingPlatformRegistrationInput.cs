using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Internal.Registration;

record class AllureTestingPlatformRegistrationInput(
    AllureTestingPlatformRegistrationMode Mode,
    Func<IServiceProvider, AllureConfiguration, ILogger> LoggerFactory,
    Func<IServiceProvider, AllureConfiguration> ConfigurationFactory,
    Func<IServiceProvider, AllureConfiguration, bool> IsSdkEnabled,
    Func<IServiceProvider, AllureConfiguration, ICorrelationSource> CorrelationServiceFactory,
    Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> WriterFactory,
    Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> TypeFormattersFactory,
    Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> LifecycleFactory,
    Action<ConfiguredAllureTestingPlatform>? OnConfigured,
    Action<ReadyAllureTestingPlatform>? OnReady
);
