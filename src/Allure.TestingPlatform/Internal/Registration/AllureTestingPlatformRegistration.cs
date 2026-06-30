using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRegistration(
    AllureTestingPlatformRegistrationMode mode
) :
    IEmbeddedAllureRegistrationContext,
    IAllureTestingPlatformSdkEvents
{
    bool hostWatchdogEnabled = true;

    Func<IServiceProvider, AllureConfiguration> configurationFactory =
        AllureRegistrationDefaults.ReadAllureConfiguration;

    Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory =
        AllureRegistrationDefaults.GetTestingPlatformLogger;

    Func<IServiceProvider, AllureConfiguration, bool> isEnabled =
        AllureRegistrationDefaults.AlwaysEnabled;

    Func<IServiceProvider, AllureConfiguration, ICorrelationStrategy> correlationStrategyFactory =
        AllureRegistrationDefaults.CorrelateBySessionUidOnly;

    Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory =
        AllureRegistrationDefaults.GetFileSystemResultsWriter;

    Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory =
        AllureRegistrationDefaults.NoTypeFormatters;

    Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory =
        AllureRegistrationDefaults.CreateLifecycle;

    public event Action<ConfiguredAllureTestingPlatformRuntime>? OnConfigured;

    public event Action<LiveAllureTestingPlatformRuntime>? OnLive;

    public IStandaloneAllureRegistrationContext DisableHostProcessWatchdog()
    {
        this.hostWatchdogEnabled = false;
        return this;
    }

    public IEmbeddedAllureRegistrationContext UseLogger(
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

    public IEmbeddedAllureRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    )
    {
        this.lifecycleFactory = lifecycleFactory;
        return this;
    }

    public IEmbeddedAllureRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationStrategy> correlationStrategyFactory
    )
    {
        this.correlationStrategyFactory = correlationStrategyFactory;
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
                HostProcessWatchdogEnabled: this.hostWatchdogEnabled,
                LoggerFactory: this.loggerFactory,
                ConfigurationFactory: this.configurationFactory,
                IsSdkEnabled: this.isEnabled,
                CorrelationStrategyFactory: this.correlationStrategyFactory,
                WriterFactory: this.writerFactory,
                TypeFormattersFactory: this.typeFormattersFactory,
                LifecycleFactory: this.lifecycleFactory,
                OnConfigured: this.OnConfigured,
                OnLive: this.OnLive
            )
        );
}
