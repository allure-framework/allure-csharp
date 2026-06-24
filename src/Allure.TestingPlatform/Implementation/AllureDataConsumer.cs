using System;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Internal;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Functions;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Services;
using System.Collections.Generic;
using Microsoft.Testing.Platform.Logging;
using Allure.Net.Commons.Configuration;

namespace Allure.TestingPlatform.Implementation;

public class AllureDataConsumer :
    AllureMtpToggleableExtension,
    IDataConsumer,
    ITestSessionLifetimeHandler
{
    readonly IServiceProvider serviceProvider;

    readonly Lazy<IAllureRuntime> allure;

    readonly Lazy<AllureDataConsumerState> allureState;

    readonly Lazy<SessionCorrelationState> correlationState;

    IAllureRuntime Allure => this.allure.Value;

    AllureDataConsumerState AllureState => this.allureState.Value;

    SessionCorrelationState CorrelationState => this.correlationState.Value;

    AllureLifecycle Lifecycle => this.Allure.Lifecycle;

    ILogger Logger => this.Allure.Logger;

    AllureConfiguration Config => this.Allure.Config;

    List<string> FailExceptions => this.Config.FailExceptions;

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

    public AllureDataConsumer(IServiceProvider serviceProvider) :
        base(
            "dd4f3277-5786-4010-8908-e70f07656ebc",
            "Allure.TestingPlatform data consumer",
            "Creates Allure results from Microsoft Testing Platform messages",
            serviceProvider
        )
    {
        this.serviceProvider = serviceProvider;
        this.allure = new(this.GetAllureRuntime);
        allureState = new(() => new(this.Allure.Lifecycle));
        correlationState = new(() => new(
            this.Allure.CorrelationService,
            this.Allure.Logger
        ));
    }

    public Task OnTestSessionStartingAsync(ITestSessionContext testSessionContext) =>
        Task.CompletedTask;

    public Task OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
    {
        if (this.CorrelationState.RemoveSessionData(testSessionContext.SessionUid) is CorrelationUid correlationUid)
        {
            this.AllureState.RemoveSession(correlationUid);
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

    IAllureRuntime GetAllureRuntime()
    {
        if (this.serviceProvider.GetService<IAllureRuntimeProvider>() is { Runtime: var result })
        {
            return result;
        }

        throw new InvalidOperationException("Allure is not initialized.");
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

        var state = this.AllureState.GetOrCreateSessionState(correlationUid);

        EnterTestScopeContext(state, testContextUid);

        if (stateProperty is InProgressTestNodeStateProperty)
        {
            this.StartTest();
            state.SetContext(testContextUid, this.Lifecycle.Context);
            return;
        }

        state.EnterContextIfExists(testContextUid);

        if (!this.Lifecycle.Context.HasTest)
        {
            // Missed InProgressTestNodeStateProperty. Normally, this shouldn't happen.
            // Establishing a new test context as a fallback.
            this.StartTest();
        }

        this.Lifecycle.UpdateTestCase((testResult) =>
        {
            this.ApplyProperties(testResult, node);
            ApplyFallbacks(testResult, node);
        });

        this.Lifecycle
            .StopTestCase()
            .WriteTestCase();

        state.RemoveTestContext(testContextUid);
    }

    async Task ConsumeSessionFileArtifactMessage(SessionFileArtifact message) =>
        AllureApi.AddGlobalAttachment(message.DisplayName, null!, message.FileInfo.FullName);

    async Task ConsumeCreateContextMessage(CreateContextMessage message)
    {
        var parentContextUid = message.ParentContextUid;
        this.AllureState.GetOrCreateSessionState(message.CorrelationUid)
            .InheritContext(
                message.ContextUid,
                message.ParentContextUid,
                () => message.Mutate(this.Allure)
            );
    }

    async Task ConsumeMutateModelMessage(MutateModelMessage message)
    {
        var contextUid = message.ContextUid;
        this.AllureState.GetOrCreateSessionState(message.CorrelationUid)
            .UpdateContext(
                message.ContextUid,
                () => message.Mutate(this.Allure)
            );
    }

    async Task ConsumeRemoveContextMessage(RemoveContextMessage message) =>
        this.AllureState.GetOrCreateSessionState(message.CorrelationUid)
            .ReleaseContext(
                message.ContextUid,
                () => message.Mutate(this.Allure)
            );

    async Task ConsumeScopeStopMessage(AllureScopeStopMessage message) =>
        this.AllureState.GetOrCreateSessionState(message.CorrelationUid)
            .ReleaseScopeContext(
                message.ScopeUid,
                () => message.Mutate(this.Allure)
            );

    async Task ConsumeTestsInScopeMessage(AllureTestsScopeMessage message) =>
        this.AllureState.GetOrCreateSessionState(message.CorrelationUid)
            .AssociateTestsWithScope(message.ScopeUid, message.TestUids);

    static void EnterTestScopeContext(SessionContextState state, TestContextUid testContextUid)
    {
        if (state.TryGetContext(new ScopeContextUid(testContextUid.Value), out var ctx)
            || (state.TryGetContext(testContextUid, out ctx)
                && !ctx.HasTest))
        {
            state.EnterContext(ctx);
        }
    }

    TestResult StartTest()
    {
        var testResult = ModelFunctions.CreateTestResult(this.Allure.Config);
        this.Lifecycle.StartTestCase(testResult);
        return testResult;
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
                ApplyTestNodeStateProperty(this.FailExceptions, testResult, testNodeState),

            TestMethodIdentifierProperty identifier =>
                ApplyTestMethodIdentifierProperty(testResult, identifier),

            TimingProperty timing =>
                ApplyTimingProperty(testResult, timing),

            FileArtifactProperty artifact =>
                ApplyFileArtifactProperty(testResult, artifact),

            StandardOutputProperty stdout =>
                ApplyStdoutProperty(testResult, stdout),

            StandardErrorProperty stderr =>
                ApplyStderrProperty(testResult, stderr),

            _ => testResult,
        };

    static TestResult ApplyFileArtifactProperty(TestResult testResult, FileArtifactProperty fileArtifact)
    {
        AllureApi.AddAttachment(fileArtifact.DisplayName, null!, fileArtifact.FileInfo.FullName);
        return testResult;
    }

    static TestResult ApplyStdoutProperty(TestResult testResult, StandardOutputProperty stdout)
    {
        AllureApi.AddAttachment(
            "Standard output",
            "text/plain",
            Encoding.UTF8.GetBytes(stdout.StandardOutput),
            "txt"
        );
        return testResult;
    }

    static TestResult ApplyStderrProperty(TestResult testResult, StandardErrorProperty stderr)
    {
        AllureApi.AddAttachment(
            "Standard error",
            "text/plain",
            Encoding.UTF8.GetBytes(stderr.StandardError),
            "txt"
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