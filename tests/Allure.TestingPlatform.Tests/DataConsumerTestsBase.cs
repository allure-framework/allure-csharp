using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Internal.TestingPlatformExtensions;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests;

public abstract class DataConsumerTestsBase : DataConsumerTestsBase<SessionUidCorrelation, ThrowingLoggerStub>;

public abstract class DataConsumerTestsBase<TCorrelationService, TLoggerService>
    where TCorrelationService : ICorrelationSource, new()
    where TLoggerService : ILogger, new()
{
    protected readonly CommandLineOptionsStub commandLineOptions;
    protected readonly AllureConfiguration config;
    protected readonly TLoggerService logger;
    protected readonly TCorrelationService correlationService;
    protected readonly InMemoryResultsWriter writer;
    protected readonly AllureLifecycle lifecycle;
    protected readonly ImmutableDictionary<Type, ITypeFormatter> typeFormatters;
    protected readonly ServiceProviderStub serviceProvider;
    protected readonly ReadyAllureTestingPlatformRuntime allureState;
    protected readonly AllureRuntimeProviderStub stateProvider;
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

        this.allureState = new(
            Mode: AllureTestingPlatformRegistrationMode.Standalone,
            Configuration: this.config,
            Logger: this.logger,
            CorrelationSource: this.correlationService,
            Writer: this.writer,
            TypeFormatters: this.typeFormatters,
            Lifecycle: this.lifecycle
        );

        this.stateProvider = new(this.allureState);

        // this.extensionSettings = new();
        // this.runtimeProvider = new(this.allure);
        // this.serviceProvider = new(this.commandLineOptions, this.runtimeProvider);

        this.consumer = new(this.stateProvider);
    }
}
