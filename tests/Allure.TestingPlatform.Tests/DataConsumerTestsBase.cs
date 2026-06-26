using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Implementation;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests;

public abstract class DataConsumerTestsBase : DataConsumerTestsBase<SessionUidCorrelation, ThrowingLoggerStub>;

public abstract class DataConsumerTestsBase<TCorrelationService, TLoggerService>
    where TCorrelationService : ICorrelationService, new()
    where TLoggerService : ILogger, new()
{
    protected readonly CommandLineOptionsStub commandLineOptions;
    protected readonly AllureConfiguration config;
    protected readonly TLoggerService logger;
    protected readonly TCorrelationService correlationService;
    protected readonly InMemoryResultsWriter writer;
    protected readonly AllureLifecycle lifecycle;
    protected readonly ExtensionSettingsStub extensionSettings;
    protected readonly ImmutableDictionary<Type, ITypeFormatter> typeFormatters;
    protected readonly AllureRuntimeStub allure;
    protected readonly AllureRuntimeProviderStub runtimeProvider;
    protected readonly ServiceProviderStub serviceProvider;
    protected readonly AllureDataConsumer consumer;

    public DataConsumerTestsBase()
    {
        this.commandLineOptions = new();
        this.writer = new();
        this.lifecycle = new(_ => this.writer);
        this.correlationService = new TCorrelationService();
        this.config = this.lifecycle.AllureConfiguration;
        this.logger = new();
        this.typeFormatters = [];

        this.allure = new(
            config: this.config,
            logger: this.logger,
            correlationService: this.correlationService,
            writer: this.writer,
            lifecycle: this.lifecycle,
            typeFormatters: this.typeFormatters
        );

        this.extensionSettings = new();
        this.runtimeProvider = new(this.allure);
        this.serviceProvider = new(this.commandLineOptions, this.runtimeProvider);

        this.consumer = new(this.serviceProvider, this.extensionSettings);
    }
}
