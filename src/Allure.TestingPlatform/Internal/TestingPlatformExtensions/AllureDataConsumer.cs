using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Functions;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Services;
using System.Collections.Generic;
using Microsoft.Testing.Platform.Logging;
using Allure.TestingPlatform.Internal.Correlation;
using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

public class AllureDataConsumer : AllureTestingPlatformExtension, IDataConsumer, ITestSessionLifetimeHandler
{
    readonly Lazy<TestHostAllureLifecycleState> allureLifecycleState;

    readonly Lazy<SessionCorrelationMap> correlationState;

    TestHostAllureLifecycleState AllureLifecycleState => this.allureLifecycleState.Value;

    SessionCorrelationMap CorrelationState => this.correlationState.Value;

    public Type[] DataTypesConsumed { get; } =
    [
        typeof(TestNodeUpdateMessage),
        typeof(SessionFileArtifact),

        typeof(AllureScopeStartMessage),
        typeof(AllureScopeStopMessage),

        typeof(AllureBeforeFixtureStartMessage),
        typeof(AllureAfterFixtureStartMessage),
        typeof(AllureFixtureUpdateMessage),
        typeof(AllureFixtureStopMessage),

        typeof(AllureTestsScopeMessage),

        typeof(AllureTestUpdateMessage),
    ];

    public AllureDataConsumer(IAllureTestingPlatformRuntimeProvider allureTestingPlatformStateProvider) :
        base(
            "dd4f3277-5786-4010-8908-e70f07656ebc",
            "Allure.TestingPlatform data consumer",
            "Creates Allure results from Microsoft Testing Platform messages",
            allureTestingPlatformStateProvider
        )
    {
        this.allureLifecycleState = new(() => new(this.Lifecycle));
        correlationState = new(() => new(
            this.CorrelationStrategy,
            this.Logger
        ));
    }

    public Task OnTestSessionStartingAsync(ITestSessionContext testSessionContext) =>
        Task.CompletedTask;

    public Task OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
    {
        if (this.CorrelationState.RemoveSessionData(testSessionContext.SessionUid) is CorrelationUid correlationUid)
        {
            this.AllureLifecycleState.RemoveSession(correlationUid);
        }
        return Task.CompletedTask;
    }

    public async Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        try
        {
            await this.ConsumeAsyncUnsafe(dataProducer, value, cancellationToken);
        }
        catch (Exception e)
        {
            if (e is OperationCanceledException)
            {
                throw;
            }

            await this.Logger.LogErrorAsync($"Error when processing {value}", e);
        }
    }

    public async Task ConsumeAsyncUnsafe(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        var correlationResult =
            await this.CorrelationState.Correlate(dataProducer, value, cancellationToken);

        if (correlationResult is CorrelationSuccess { CorrelationUid: var correlationUid, MessagesToProcess: var messages })
        {
            await this.ConsumeBufferedMessages(correlationUid, messages);
        }
        else if (correlationResult is CorrelationFailure { Message: var message })
        {
            await this.Logger.LogErrorAsync($"Session correlation error: {message}");
        }
    }

    async Task ConsumeBufferedMessages(CorrelationUid correlationUid, IEnumerable<IData> messages)
    {
        foreach (var message in messages)
        {
            await (message switch
            {
                TestNodeUpdateMessage testNodeUpdateMessage =>
                    this.ConsumeTestNodeUpdateMessage(correlationUid, testNodeUpdateMessage),

                SessionFileArtifact sessionFileArtifact =>
                    this.ConsumeSessionFileArtifactMessage(sessionFileArtifact),

                CreateContextMessage createContextMessage =>
                    this.ConsumeCreateContextMessage(createContextMessage),

                MutateModelMessage mutateModelMessage =>
                    this.ConsumeMutateModelMessage(mutateModelMessage),

                AllureScopeStopMessage allureScopeStopMessage =>
                    this.ConsumeScopeStopMessage(allureScopeStopMessage),

                RemoveContextMessage removeContextMessage =>
                    this.ConsumeRemoveContextMessage(removeContextMessage),

                AllureTestsScopeMessage allureTestsScopeMessage =>
                    this.ConsumeTestsInScopeMessage(allureTestsScopeMessage),

                _ => Task.CompletedTask,
            });
        }
    }

    async Task ConsumeTestNodeUpdateMessage(CorrelationUid correlationUid, TestNodeUpdateMessage message)
    {
        var node = message.TestNode;
        var uid = node.Uid;
        TestContextUid testContextUid = new(node.Uid);

        var stateProperty = node.Properties
            .OfType<TestNodeStateProperty>()
            .SingleOrDefault();
        if (stateProperty is null or DiscoveredTestNodeStateProperty)
        {
            return;
        }

        var state = this.AllureLifecycleState.GetOrCreateSessionState(correlationUid);

        if (stateProperty is InProgressTestNodeStateProperty)
        {
            state.ForkNewTestContext(testContextUid, this.StartTest);
            return;
        }

        var runningTestContext = state.GetRunningTestContext(testContextUid);

        if (!runningTestContext.HasTest)
        {
            // Missed InProgressTestNodeStateProperty. Normally, this shouldn't happen.
            // If it does though, we create a new test context and pass the context through the state
            // to apply pending updates.
            runningTestContext = state.ForkContext(testContextUid, runningTestContext, this.StartTest);
        }

        state.ReleaseContext(
            testContextUid,
            () => this.Lifecycle
                .UpdateTestCase((testResult) =>
                {
                    this.ApplyProperties(testResult, node);
                    ApplyFallbacks(testResult, node);
                })
                .StopTestCase()
                .WriteTestCase()
        );
    }

    async Task ConsumeSessionFileArtifactMessage(SessionFileArtifact message) =>
        ModelFunctions.AddGlobalAttachmentFile(
            this.Writer,
            message.DisplayName,
            message.FileInfo
        );

    async Task ConsumeCreateContextMessage(CreateContextMessage message)
    {
        var parentContextUid = message.ParentContextUid;
        this.AllureLifecycleState.GetOrCreateSessionState(message.CorrelationUid)
            .InheritContext(
                message.ContextUid,
                message.ParentContextUid,
                () => message.Mutate(this.ReadyRuntime)
            );
    }

    async Task ConsumeMutateModelMessage(MutateModelMessage message)
    {
        var contextUid = message.ContextUid;
        this.AllureLifecycleState.GetOrCreateSessionState(message.CorrelationUid)
            .UpdateContext(
                message.ContextUid,
                () => message.Mutate(this.ReadyRuntime)
            );
    }

    async Task ConsumeRemoveContextMessage(RemoveContextMessage message) =>
        this.AllureLifecycleState.GetOrCreateSessionState(message.CorrelationUid)
            .ReleaseContext(
                message.ContextUid,
                () => message.Mutate(this.ReadyRuntime)
            );

    async Task ConsumeScopeStopMessage(AllureScopeStopMessage message) =>
        this.AllureLifecycleState.GetOrCreateSessionState(message.CorrelationUid)
            .ReleaseScopeContext(
                message.ScopeUid,
                () => message.Mutate(this.ReadyRuntime)
            );

    async Task ConsumeTestsInScopeMessage(AllureTestsScopeMessage message) =>
        this.AllureLifecycleState.GetOrCreateSessionState(message.CorrelationUid)
            .AssociateTestsWithScope(message.ScopeUid, message.TestUids);

    void StartTest()
    {
        var testResult = ModelFunctions.CreateTestResult(this.Configuration);
        this.Lifecycle.StartTestCase(testResult);
    }

    void ApplyProperties(TestResult testResult, TestNode node)
    {
        foreach (var property in node.Properties)
        {
            this.ApplyProperty(testResult, property);
        }
    }

    TestResult ApplyProperty(TestResult testResult, IProperty property) =>
        property switch
        {
            TestNodeStateProperty testNodeState =>
                ApplyTestNodeStateProperty(this.Configuration.FailExceptions, testResult, testNodeState),

            TestMethodIdentifierProperty identifier =>
                ApplyTestMethodIdentifierProperty(testResult, identifier),

            TimingProperty timing =>
                ApplyTimingProperty(testResult, timing),

            FileArtifactProperty artifact =>
                this.ApplyFileArtifactProperty(testResult, artifact),

            StandardOutputProperty stdout =>
                this.ApplyStdoutProperty(testResult, stdout),

            StandardErrorProperty stderr =>
                this.ApplyStderrProperty(testResult, stderr),

            _ => testResult,
        };

    TestResult ApplyFileArtifactProperty(TestResult testResult, FileArtifactProperty fileArtifact)
    {
        ModelFunctions.AddFileAttachment(
            this.Writer,
            testResult,
            fileArtifact.DisplayName,
            fileArtifact.FileInfo
        );
        return testResult;
    }

    TestResult ApplyStdoutProperty(TestResult testResult, StandardOutputProperty stdout)
    {
        ModelFunctions.AddTxtAttachment(
            this.Writer,
            testResult,
            "Standard output",
            stdout.StandardOutput
        );
        return testResult;
    }

    TestResult ApplyStderrProperty(TestResult testResult, StandardErrorProperty stderr)
    {
        ModelFunctions.AddTxtAttachment(
            this.Writer,
            testResult,
            "Standard error",
            stderr.StandardError
        );
        return testResult;
    }

    static TestResult ApplyTimingProperty(TestResult testResult, TimingProperty timing)
    {
        ModelFunctions.ApplyTimings(testResult, timing);
        return testResult;
    }

    static void ApplyFallbacks(TestResult testResult, TestNode node)
    {
        testResult.name ??= node.DisplayName;
        testResult.fullName ??= node.Uid;
    }

    static TestResult ApplyTestNodeStateProperty(
        List<string> failExceptions,
        TestResult testResult,
        TestNodeStateProperty testNodeState
    )
    {
        ModelFunctions.ApplyStateAsFallback(failExceptions, testResult, testNodeState);
        return testResult;
    }

    static TestResult ApplyTestMethodIdentifierProperty(TestResult testResult, TestMethodIdentifierProperty identifierProperty)
    {
        ModelFunctions.ApplyIdentityAsFallback(testResult, identifierProperty);
        return testResult;
    }
}