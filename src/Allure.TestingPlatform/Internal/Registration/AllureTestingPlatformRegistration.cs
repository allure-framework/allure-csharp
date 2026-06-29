using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRegistration(
    AllureTestingPlatformRegistrationMode mode
) : IEmbeddedAllureRegistrationContext, IAllureTestingPlatformSdkEvents
{
    bool hostWatchdogEnabled = true;

    Func<IServiceProvider, AllureConfiguration> configurationFactory =
        AllureRegistrationFunctions.ReadAllureConfiguration;

    Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory =
        AllureRegistrationFunctions.GetTestingPlatformLogger;

    Func<IServiceProvider, AllureConfiguration, bool> isEnabled =
        AllureRegistrationFunctions.DoNotDisable;

    Func<IServiceProvider, AllureConfiguration, ICorrelationSource> correlationServiceFactory =
        AllureRegistrationFunctions.CorrelateBySessionUidOnly;

    Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory =
        AllureRegistrationFunctions.GetFileSystemResultsWriter;

    Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory =
        AllureRegistrationFunctions.NoTypeFormatters;

    Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory =
        AllureRegistrationFunctions.CreateLifecycle;

    public event Action<ConfiguredAllureTestingPlatformRuntime>? OnConfigured;

    public event Action<ReadyAllureTestingPlatformRuntime>? OnReady;

    public IStandaloneAllureRegistrationContext DisableHostProcessWatchdog()
    {
        this.hostWatchdogEnabled = false;
        return this;
    }

    public IStandaloneAllureRegistrationContext UseLogger(
        Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory
    )
    {
        this.loggerFactory = loggerFactory;
        return this;
    }

    public IStandaloneAllureRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    )
    {
        this.configurationFactory = configurationFactory;
        return this;
    }

    public IStandaloneAllureRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    )
    {
        this.isEnabled = isEnabled;
        return this;
    }

    public IStandaloneAllureRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    )
    {
        this.writerFactory = writerFactory;
        return this;
    }

    public IStandaloneAllureRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory
    )
    {
        this.typeFormattersFactory = typeFormattersFactory;
        return this;
    }

    public IStandaloneAllureRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    )
    {
        this.lifecycleFactory = lifecycleFactory;
        return this;
    }

    public IEmbeddedAllureRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationSource> correlationServiceFactory
    )
    {
        this.correlationServiceFactory = correlationServiceFactory;
        return this;
    }

    public IEmbeddedAllureRegistrationContext SetSdkEventHandlers(
        Action<IAllureTestingPlatformSdkEvents> sdkEventHandlersRegistration
    )
    {
        sdkEventHandlersRegistration(this);
        return this;
    }

    public AllureTestingPlatformPreparedRegistration Prepare() =>
        new(
            new(
                Mode: mode,
                HostProcessWathdogEnabled: this.hostWatchdogEnabled,
                LoggerFactory: this.loggerFactory,
                ConfigurationFactory: this.configurationFactory,
                IsSdkEnabled: this.isEnabled,
                CorrelationServiceFactory: this.correlationServiceFactory,
                WriterFactory: this.writerFactory,
                TypeFormattersFactory: this.typeFormattersFactory,
                LifecycleFactory: this.lifecycleFactory,
                OnConfigured: this.OnConfigured,
                OnReady: this.OnReady
            )
        );
}
