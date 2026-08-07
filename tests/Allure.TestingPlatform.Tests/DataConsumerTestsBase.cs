using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Registration;
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
    protected virtual AllureTestingPlatformConfiguration Config { get; } = new();
    protected readonly TLoggerService logger;
    protected readonly TCorrelationStrategy correlationStrategy;
    protected readonly InMemoryResultsDestination writer;
    protected readonly ServiceProviderStub serviceProvider;
    protected readonly IAllureRuntimeRegistrationPlan<AllureTestingPlatformConfiguration, IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>> registrationPlan;
    protected readonly LateBoundReference<IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>> runtimeReference;
    protected readonly AllureDataConsumer<AllureTestingPlatformConfiguration, IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>> consumer;

    public DataConsumerTestsBase()
    {

        this.commandLineOptions = new();
        this.logger = new();
        this.writer = new();
        this.correlationStrategy = new TCorrelationStrategy();

        var builder = new AllureTestingPlatformRuntimeBuilder("test");
        this.registrationPlan = builder.Prepare((context) =>
        {
            context.UseConfigurationSource(() => DelegateConfigurationSource.Create("test", () => this.Config));
            context.UseLogger((_) => this.logger);
            context.UseDestination((_) => this.writer);
            context.UseCorrelationStrategy((_) => this.correlationStrategy);
        });

        this.runtimeReference = new();
        this.consumer = new(this.runtimeReference);
    }

    [Before(Test)]
    public void SetUp()
    {
        this.runtimeReference.Bind(this.registrationPlan.Build().Runtime);
    }
}
