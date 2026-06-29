using System;
using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Registration;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Sdk.Runtime.AdapterState;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Internal.Registration;

class AllureTestingPlatformRegistration(
    AllureTestingPlatformRegistrationMode mode
) : IEmbeddedRegistrationContext, IAllureTestingPlatformSdkEvents
{
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

    public event Action<ConfiguredAllureTestingPlatform>? OnConfigured;

    public event Action<ReadyAllureTestingPlatform>? OnReady;

    public IStandaloneRegistrationContext UseLogger(
        Func<IServiceProvider, AllureConfiguration, ILogger> loggerFactory
    )
    {
        this.loggerFactory = loggerFactory;
        return this;
    }

    public IStandaloneRegistrationContext UseConfiguration(
        Func<IServiceProvider, AllureConfiguration> configurationFactory
    )
    {
        this.configurationFactory = configurationFactory;
        return this;
    }

    public IStandaloneRegistrationContext SetIsEnabled(
        Func<IServiceProvider, AllureConfiguration, bool> isEnabled
    )
    {
        this.isEnabled = isEnabled;
        return this;
    }

    public IStandaloneRegistrationContext UseWriter(
        Func<IServiceProvider, AllureConfiguration, IAllureResultsWriter> writerFactory
    )
    {
        this.writerFactory = writerFactory;
        return this;
    }

    public IStandaloneRegistrationContext UseTypeFormatters(
        Func<IServiceProvider, AllureConfiguration, IReadOnlyDictionary<Type, ITypeFormatter>> typeFormattersFactory
    )
    {
        this.typeFormattersFactory = typeFormattersFactory;
        return this;
    }

    public IStandaloneRegistrationContext UseLifecycle(
        Func<IServiceProvider, AllureLifecycleFactoryContext, AllureLifecycle> lifecycleFactory
    )
    {
        this.lifecycleFactory = lifecycleFactory;
        return this;
    }

    public IEmbeddedRegistrationContext UseCorrelation(
        Func<IServiceProvider, AllureConfiguration, ICorrelationSource> correlationServiceFactory
    )
    {
        this.correlationServiceFactory = correlationServiceFactory;
        return this;
    }

    public IEmbeddedRegistrationContext SetSdkEventHandlers(
        Action<IAllureTestingPlatformSdkEvents> sdkEventHandlersRegistration
    )
    {
        sdkEventHandlersRegistration(this);
        return this;
    }

    public AllureTestingPlatformPreparedRegistration Prepare()
    {
        return new(
            new(
                Mode: mode,
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
}
