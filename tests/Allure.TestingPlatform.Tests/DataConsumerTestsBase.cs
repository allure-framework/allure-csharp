using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests;

public abstract class DataConsumerTestsBase : DataConsumerTestsBase<TestingPlatformSessionUidCorrelationStrategy, ThrowingLoggerStub>;

public abstract class DataConsumerTestsBase<TCorrelationStrategy, TLoggerService>
    where TCorrelationStrategy : ICorrelationStrategy, new()
    where TLoggerService : ILogger, new()
{
    protected readonly CommandLineOptionsStub commandLineOptions;
    protected readonly AllureConfiguration config;
    protected readonly TLoggerService logger;
    protected readonly TCorrelationStrategy correlationStrategy;
    protected readonly InMemoryResultsWriter writer;
    protected readonly AllureLifecycle lifecycle;
    protected readonly ImmutableDictionary<Type, ITypeFormatter> typeFormatters;
    protected readonly ServiceProviderStub serviceProvider;
    protected readonly LiveAllureTestingPlatformRuntime allureRuntime;
    protected readonly AllureRuntimeReferenceStub runtimeReference;
    protected readonly AllureDataConsumer consumer;

    public DataConsumerTestsBase()
    {
        this.commandLineOptions = new();
        this.config = new();
        this.logger = new();
        this.writer = new();
        this.typeFormatters = [];
        this.correlationStrategy = new TCorrelationStrategy();
        this.lifecycle = new(this.config, this.writer);

        this.allureRuntime = new(
            Mode: AllureTestingPlatformRegistrationMode.Standalone,
            Configuration: this.config,
            Logger: this.logger,
            CorrelationStrategy: this.correlationStrategy,
            Writer: this.writer,
            TypeFormatters: this.typeFormatters,
            Lifecycle: this.lifecycle
        );

        this.runtimeReference = new(this.allureRuntime);

        this.consumer = new(this.runtimeReference);
    }
}
