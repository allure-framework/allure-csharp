using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Tests.Stubs;

namespace Allure.TestingPlatform.Tests;

public abstract class DataConsumerTestsBase
{
    protected readonly AllureConfiguration config;
    protected readonly ICorrelationService correlationService;
    protected readonly AllureLifecycle lifecycle;
    protected readonly InMemoryResultsWriter writer;
    protected readonly AllureRuntimeStub allure;
    protected readonly AllureDataConsumer consumer;
    protected readonly Dictionary<Type, ITypeFormatter> typeFormatters;

    public DataConsumerTestsBase()
    {
        this.writer = new();
        this.lifecycle = new(_ => this.writer);
        this.correlationService = new SessionUidCorrelation();
        this.config = this.lifecycle.AllureConfiguration;
        this.typeFormatters = [];
        this.allure = new(
            isEnabled: true,
            config: this.config,
            correlationService: this.correlationService,
            writer: this.writer,
            lifecycle: this.lifecycle,
            typeFormatters: this.typeFormatters
        );
        this.consumer = new(this.allure);
    }
}
