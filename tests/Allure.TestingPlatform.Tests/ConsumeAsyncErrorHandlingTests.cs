using System.Collections.Immutable;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Sdk;
using Allure.Net.Commons.Sdk.Writers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Logging;

namespace Allure.TestingPlatform.Tests;

public class ConsumeAsyncErrorHandlingTests
{
    [Test]
    public async Task ShouldLogAndSwallowNonCancellationExceptionFromCorrelationStrategy()
    {
        var exception = new InvalidOperationException("correlation failed");
        var strategy = new ThrowingCorrelationStrategy(exception);
        var fixture = CreateFixture(strategy);

        await fixture.Consumer.ConsumeAsync(
            DataProducerStub.Instance,
            PassedTestNode(),
            CancellationToken.None
        );

        var log = await Assert.That(fixture.Logger.Calls).HasSingleItem();
        await Assert.That(log.Level).IsEqualTo(LogLevel.Error);
        await Assert.That(log.Exception).IsSameReferenceAs(exception);
        await Assert.That(log.State).IsTypeOf<string>()
            .And.Contains("Error while processing");
        await Assert.That(fixture.Writer.TestResults).IsEmpty();
    }

    [Test]
    public async Task ShouldPropagateOperationCanceledExceptionFromCorrelationStrategy()
    {
        var exception = new OperationCanceledException("correlation cancelled");
        var strategy = new ThrowingCorrelationStrategy(exception);
        var fixture = CreateFixture(strategy);

        await Assert.That(async () => await fixture.Consumer.ConsumeAsync(
            DataProducerStub.Instance,
            PassedTestNode(),
            CancellationToken.None
        )).Throws<OperationCanceledException>();

        await Assert.That(fixture.Logger.Calls).IsEmpty();
    }

    [Test]
    public async Task ShouldLogMessageProcessingExceptionAndContinueWithNextMessage()
    {
        var exception = new InvalidOperationException("write failed");
        InMemoryResultsWriter sink = new();
        var fixture = CreateFixture(
            new TestingPlatformSessionUidCorrelationStrategy(),
            new ThrowingOnceResultsWriter(sink, exception),
            sink
        );

        await fixture.Consumer.ConsumeAsync(
            DataProducerStub.Instance,
            SessionArtifact("first"),
            CancellationToken.None
        );
        await fixture.Consumer.ConsumeAsync(
            DataProducerStub.Instance,
            SessionArtifact("second"),
            CancellationToken.None
        );

        var log = await Assert.That(fixture.Logger.Calls).HasSingleItem();
        await Assert.That(log.Level).IsEqualTo(LogLevel.Error);
        await Assert.That(log.Exception).IsSameReferenceAs(exception);
        await Assert.That(log.State).IsTypeOf<string>()
            .And.Contains("Error while processing");

        var globals = await Assert.That(fixture.Writer.Globals).HasSingleItem();
        var attachment = await Assert.That(globals.attachments).HasSingleItem();
        await Assert.That(attachment.name).IsEqualTo("second");
    }

    [Test]
    public async Task ShouldDiscardTestContextWhenWriterExceptionIsSwallowed()
    {
        var exception = new InvalidOperationException("test result write failed");
        InMemoryResultsWriter sink = new();
        var fixture = CreateFixture(
            new TestingPlatformSessionUidCorrelationStrategy(),
            new ThrowingOnceTestResultWriter(sink, exception),
            sink
        );
        var firstAttempt = PassedTestNode("first attempt");
        var secondAttempt = PassedTestNode("second attempt");

        await fixture.Consumer.ConsumeAsync(
            DataProducerStub.Instance,
            firstAttempt,
            CancellationToken.None
        );
        await fixture.Consumer.ConsumeAsync(
            DataProducerStub.Instance,
            secondAttempt,
            CancellationToken.None
        );

        await Assert.That(fixture.Logger.Calls).HasSingleItem();
        var testResult = await Assert.That(fixture.Writer.TestResults).HasSingleItem();
        await Assert.That(testResult.name).IsEqualTo("second attempt");
    }

    static Fixture CreateFixture(
        ICorrelationStrategy correlationStrategy,
        IAllureResultsWriter writer = null,
        InMemoryResultsWriter sink = null
    )
    {
        LoggerSpy logger = new();
        sink ??= new();
        writer ??= sink;
        var config = new AllureConfiguration();
        AllureLifecycle lifecycle = new(config, writer);
        LiveAllureTestingPlatformRuntime runtime = new(
            AllureTestingPlatformRegistrationMode.Standalone,
            logger,
            config,
            correlationStrategy,
            writer,
            [],
            lifecycle
        );
        return new(
            new(new AllureRuntimeReferenceStub(runtime)),
            logger,
            sink
        );
    }

    static TestNodeUpdateMessage PassedTestNode(string displayName = "test") => new(
        new("session-1"),
        new()
        {
            DisplayName = displayName,
            Uid = "test-1",
            Properties = new(new PassedTestNodeStateProperty()),
        }
    );

    static SessionFileArtifact SessionArtifact(string displayName) => new(
        new("session-1"),
        new($"{displayName}.txt"),
        displayName
    );

    sealed record Fixture(
        AllureDataConsumer Consumer,
        LoggerSpy Logger,
        InMemoryResultsWriter Writer
    );

    sealed class ThrowingCorrelationStrategy(Exception exception) : ICorrelationStrategy
    {
        public Task<CorrelationUid?> GetCorrelationAsync(
            IDataProducer dataProducer,
            DataWithSessionUid message,
            CancellationToken cancellationToken
        ) =>
            throw exception;
    }

    sealed class ThrowingOnceResultsWriter(
        IAllureResultsWriter inner,
        Exception exception
    ) : IAllureResultsWriter
    {
        bool shouldThrow = true;

        public void CleanUp() => inner.CleanUp();

        public void Write(Allure.Net.Commons.TestResult testResult) => inner.Write(testResult);

        public void Write(TestResultContainer container) => inner.Write(container);

        public void Write(Globals globals)
        {
            if (this.shouldThrow)
            {
                this.shouldThrow = false;
                throw exception;
            }

            inner.Write(globals);
        }

        public void Write(string outputFileName, byte[] content) =>
            inner.Write(outputFileName, content);

        public void Write(string destinationFileName, string sourceFilePath) =>
            inner.Write(destinationFileName, sourceFilePath);
    }

    sealed class ThrowingOnceTestResultWriter(
        IAllureResultsWriter inner,
        Exception exception
    ) : IAllureResultsWriter
    {
        bool shouldThrow = true;

        public void CleanUp() => inner.CleanUp();

        public void Write(Allure.Net.Commons.TestResult testResult)
        {
            if (this.shouldThrow)
            {
                this.shouldThrow = false;
                throw exception;
            }

            inner.Write(testResult);
        }

        public void Write(TestResultContainer container) => inner.Write(container);

        public void Write(Globals globals) => inner.Write(globals);

        public void Write(string outputFileName, byte[] content) =>
            inner.Write(outputFileName, content);

        public void Write(string destinationFileName, string sourceFilePath) =>
            inner.Write(destinationFileName, sourceFilePath);
    }
}
