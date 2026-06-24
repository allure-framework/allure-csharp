using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Implementation;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Tests.Stubs;

namespace Allure.TestingPlatform.Tests;

public abstract class DataConsumerTestsBase : DataConsumerTestsBase<SessionUidCorrelation>;

public abstract class DataConsumerTestsBase<TCorrelationService>
    where TCorrelationService : ICorrelationService, new()
{
    protected readonly CommandLineOptionsStub commandLineOptions;
    protected readonly ServiceProviderStub serviceProvider;
    protected readonly AllureConfiguration config;
    protected readonly LoggerSpy logger;
    protected readonly TCorrelationService correlationService;
    protected readonly AllureLifecycle lifecycle;
    protected readonly InMemoryResultsWriter writer;
    protected readonly AllureRuntimeStub allure;
    protected readonly AllureDataConsumer consumer;
    protected readonly Dictionary<Type, ITypeFormatter> typeFormatters;

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
            isEnabled: true,
            config: this.config,
            logger: this.logger,
            correlationService: this.correlationService,
            writer: this.writer,
            lifecycle: this.lifecycle,
            typeFormatters: this.typeFormatters
        );
        this.serviceProvider = new(this.commandLineOptions, new(this.allure));

        this.consumer = new(this.serviceProvider);
    }
}
