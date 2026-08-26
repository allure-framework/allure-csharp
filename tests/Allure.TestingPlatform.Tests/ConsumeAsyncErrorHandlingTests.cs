using Allure.Model;
using Allure.Sdk.Configuration;
using Allure.Sdk.Registration;
using Allure.Sdk.Results;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Internal.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Registration;
using Allure.TestingPlatform.Internal.TestingPlatformExtensions;
using Allure.TestingPlatform.Tests.Stubs;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Logging;
using AllureTestResult = Allure.Model.TestResult;
using Allure.TestingPlatform.Internal.Registration;

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
        InMemoryResultsDestination sink = new();
        var fixture = CreateFixture(
            new SessionUidCorrelationStrategy(),
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
        var attachment = await Assert.That(globals.Attachments).HasSingleItem();
        await Assert.That(attachment.Name).IsEqualTo("second");
    }

    [Test]
    public async Task ShouldDiscardTestContextWhenWriterExceptionIsSwallowed()
    {
        var exception = new InvalidOperationException("test result write failed");
        InMemoryResultsDestination sink = new();
        var fixture = CreateFixture(
            new SessionUidCorrelationStrategy(),
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
        await Assert.That(testResult.Name).IsEqualTo("second attempt");
    }

    static Fixture CreateFixture(
        ICorrelationStrategy correlationStrategy,
        IAllureResultsDestination writer = null,
        InMemoryResultsDestination sink = null
    )
    {
        LoggerSpy logger = new();
        sink ??= new();
        writer ??= sink;
        var config = new AllureTestingPlatformConfiguration();
        var builder = new AllureTestingPlatformBuilder("error-handling-test");
        var registrationPlan = builder.Prepare(context =>
        {
            context.UseConfigurationSource(
                () => DelegateConfigurationSource.Create("test", () => config)
            );
            context.UseLogger(_ => logger);
            context.UseDestination(_ => writer);
            context.UseCorrelationStrategy(_ => correlationStrategy);
        });
        registrationPlan.Build();
        return new(
            new(
                new RuntimeCoordinatorSpy(
                    builder.ConfigurationReference,
                    registrationPlan.RuntimeReference
                ),
                ITestingPlatformRequestBinding.Mock()
            ),
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
        InMemoryResultsDestination Writer
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
        IAllureResultsDestination inner,
        Exception exception
    ) : IAllureResultsDestination
    {
        bool shouldThrow = true;

        public string CopyAttachment(string sourceFilePath, string fileExtension) =>
            inner.CopyAttachment(sourceFilePath, fileExtension);

        public Task<string> CopyAttachmentAsync(string sourceFilePath, string fileExtension, CancellationToken cancellationToken) =>
            inner.CopyAttachmentAsync(sourceFilePath, fileExtension, cancellationToken);

        public string WriteAttachment(Stream content, string fileExtension) =>
            inner.WriteAttachment(content, fileExtension);

        public Task<string> WriteAttachmentAsync(Stream content, string fileExtension, CancellationToken cancellationToken) =>
            inner.WriteAttachmentAsync(content, fileExtension, cancellationToken);

        public void WriteContainer(TestResultScope scope) =>
            inner.WriteContainer(scope);

        public Task WriteContainerAsync(TestResultScope scope, CancellationToken cancellationToken) =>
            inner.WriteContainerAsync(scope, cancellationToken);

        public void WriteGlobals(Globals globals)
        {
            if (this.shouldThrow)
            {
                this.shouldThrow = false;
                throw exception;
            }

            inner.WriteGlobals(globals);
        }

        public Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken) =>
            inner.WriteGlobalsAsync(globals, cancellationToken);

        public void WriteTestResult(AllureTestResult testResult) =>
            inner.WriteTestResult(testResult);

        public Task WriteTestResultAsync(AllureTestResult testResult, CancellationToken cancellationToken) =>
            inner.WriteTestResultAsync(testResult, cancellationToken);
    }

    sealed class ThrowingOnceTestResultWriter(
        IAllureResultsDestination inner,
        Exception exception
    ) : IAllureResultsDestination
    {
        bool shouldThrow = true;

        public string CopyAttachment(string sourceFilePath, string fileExtension) =>
            inner.CopyAttachment(sourceFilePath, fileExtension);

        public Task<string> CopyAttachmentAsync(string sourceFilePath, string fileExtension, CancellationToken cancellationToken) =>
            inner.CopyAttachmentAsync(sourceFilePath, fileExtension, cancellationToken);

        public string WriteAttachment(Stream content, string fileExtension) =>
            inner.WriteAttachment(content, fileExtension);

        public Task<string> WriteAttachmentAsync(Stream content, string fileExtension, CancellationToken cancellationToken) =>
            inner.WriteAttachmentAsync(content, fileExtension, cancellationToken);

        public void WriteContainer(TestResultScope scope) =>
            inner.WriteContainer(scope);

        public Task WriteContainerAsync(TestResultScope scope, CancellationToken cancellationToken) =>
            inner.WriteContainerAsync(scope, cancellationToken);

        public void WriteGlobals(Globals globals) =>
            inner.WriteGlobals(globals);

        public Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken) =>
            inner.WriteGlobalsAsync(globals, cancellationToken);

        public void WriteTestResult(AllureTestResult testResult)
        {
            if (this.shouldThrow)
            {
                this.shouldThrow = false;
                throw exception;
            }
            inner.WriteTestResult(testResult);
        }

        public Task WriteTestResultAsync(AllureTestResult testResult, CancellationToken cancellationToken) =>
            inner.WriteTestResultAsync(testResult, cancellationToken);
    }
}
