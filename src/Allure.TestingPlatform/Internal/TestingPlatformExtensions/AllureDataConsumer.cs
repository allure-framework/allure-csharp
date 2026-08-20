using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Allure.TestingPlatform.Internal.Functions;
using Microsoft.Testing.Platform.Extensions.TestHost;
using Microsoft.Testing.Platform.Services;
using System.Collections.Generic;
using Microsoft.Testing.Platform.Logging;
using Allure.TestingPlatform.Internal.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Configuration;
using Allure.Model;
using System.Collections.Immutable;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.TestingPlatformExtensions;
using Allure.TestingPlatform.Internal.Registration;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Internal.Lifecycle;

namespace Allure.TestingPlatform.Internal.TestingPlatformExtensions;

using IAllureTestingPlatformRegistrationControl =
    IAllureTestingPlatformRegistrationControl<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >;

sealed class AllureDataConsumer :
    AllureTestingPlatformExtension<
        AllureTestingPlatformConfiguration,
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration>
    >,
    IDataConsumer,
    ITestSessionLifetimeHandler,
    IAsyncDisposable
{
    readonly IAllureTestingPlatformRegistrationControl runtimeControl;

    readonly ITestingPlatformRequestBinding requestBinding;

    readonly Lazy<SessionLifecycleRegistry> allureLifecycleState;

    readonly Lazy<SessionCorrelationMap> correlationState;

    SessionLifecycleRegistry AllureLifecycleState => this.allureLifecycleState.Value;

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

        typeof(AllureStepStartMessage),
        typeof(AllureStepUpdateMessage),
        typeof(AllureStepStopMessage),

        typeof(AllureScopeTestsMessage),

        typeof(AllureTestExecutionBindingMessage),
        typeof(AllureTestUpdateMessage),
        typeof(AllureTestExecutionFinishMessage),

        typeof(AllureExecutableItemUpdateMessage),
    ];

    public AllureDataConsumer(
        IAllureTestingPlatformRegistrationControl runtimeControl,
        ITestingPlatformRequestBinding requestBinding
    ) :
        base(
            "dd4f3277-5786-4010-8908-e70f07656ebc",
            "Allure.TestingPlatform data consumer",
            "Creates Allure results from Microsoft Testing Platform messages.",
            runtimeControl
        )
    {
        this.runtimeControl = runtimeControl;
        this.requestBinding = requestBinding;
        this.allureLifecycleState = new(() => new(
            this.ContextApi,
            runtimeControl.CreateTestExecutionCoordinator
        ));
        correlationState = new(() => new(
            this.CorrelationStrategy,
            this.Logger
        ));
    }

    public Task OnTestSessionStartingAsync(ITestSessionContext testSessionContext)
    {

        try
        {
            this.requestBinding.Activate();
            this.runtimeControl.EnsureRuntimeStarted();
        }
        catch
        {
            this.requestBinding.Release();
            throw;
        }

        return Task.CompletedTask;
    }

    public Task OnTestSessionFinishingAsync(ITestSessionContext testSessionContext)
    {
        try
        {
            if (
                this.CorrelationState.RemoveSessionData(testSessionContext.SessionUid)
                    is CorrelationUid correlationUid
            )
            {
                this.AllureLifecycleState.Remove(correlationUid);
            }
        }
        finally
        {
            this.requestBinding.Release();
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

            await this.Logger.LogErrorAsync($"Error while processing {value}", e);
        }
    }

    public ValueTask DisposeAsync()
    {
        this.requestBinding.Dispose();
        return default;
    }

    async Task ConsumeAsyncUnsafe(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
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

                AllureModelCreateMessage allureModelCreateMessage =>
                    this.ConsumeAllureModelCreateMessage(allureModelCreateMessage),

                AllureModelUpdateMessage allureModelUpdateMessage =>
                    this.ConsumeAllureModelUpdateMessage(allureModelUpdateMessage),

                AllureScopeStopMessage allureScopeStopMessage =>
                    this.ConsumeScopeStopMessage(allureScopeStopMessage),

                AllureModelRemoveMessage allureModelRemoveMessage =>
                    this.ConsumeAllureModelRemoveMessage(allureModelRemoveMessage),

                AllureScopeTestsMessage allureScopeTestsMessage =>
                    this.ConsumeTestsInScopeMessage(allureScopeTestsMessage),

                AllureTestExecutionBindingMessage allureTestExecutionBindingMessage =>
                    this.ConsumeTestExecutionBindingMessage(allureTestExecutionBindingMessage),

                AllureTestExecutionFinishMessage allureTestExecutionFinishMessage =>
                    this.ConsumeTestExecutionFinishMessage(allureTestExecutionFinishMessage),

                _ => Task.CompletedTask,
            });
        }
    }

    async Task ConsumeTestNodeUpdateMessage(CorrelationUid correlationUid, TestNodeUpdateMessage message)
    {
        var node = message.TestNode;
        var uid = node.Uid;
        TestExecutionStateUid testContextUid = new(node.Uid);

        var stateProperty = node.Properties
            .OfType<TestNodeStateProperty>()
            .SingleOrDefault();
        if (stateProperty is null or DiscoveredTestNodeStateProperty)
        {
            return;
        }

        var coordinator = this.AllureLifecycleState.GetOrCreate(correlationUid);

        if (stateProperty is InProgressTestNodeStateProperty)
        {
            coordinator.StartTestState(testContextUid, () => this.StartTest(node.DisplayName));
            return;
        }

        coordinator.FinishTestState(
            testContextUid,
            startTestIfMissing: () => this.StartTest(node.DisplayName),
            finishTest: (runtime) => this.FinalizeTestResult(runtime, node)
        );
    }

    async Task ConsumeSessionFileArtifactMessage(SessionFileArtifact message) =>
        GlobalAttachments.SaveFile(
            this.ResultsDestination,
            message.DisplayName,
            message.FileInfo
        );

    async Task ConsumeAllureModelCreateMessage(AllureModelCreateMessage message)
    {
        var parentContextUid = message.ParentContextUid;
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .InheritState(
                message.ContextUid,
                message.ParentContextUid,
                () => message.ApplyTo(this.Runtime)
            );
    }

    async Task ConsumeAllureModelUpdateMessage(AllureModelUpdateMessage message)
    {
        var contextUid = message.ContextUid;
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .UpdateState(
                message.ContextUid,
                () => message.ApplyTo(this.Runtime)
            );
    }

    async Task ConsumeAllureModelRemoveMessage(AllureModelRemoveMessage message)
    {
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .ReleaseState(
                message.ContextUid,
                (_) => message.ApplyTo(this.Runtime)
            );
    }

    async Task ConsumeScopeStopMessage(AllureScopeStopMessage message)
    {
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .ReleaseScopeState(
                message.ScopeUid,
                (_) => message.ApplyTo(this.Runtime)
            );
    }

    async Task ConsumeTestsInScopeMessage(AllureScopeTestsMessage message)
    {
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .AssociateTestsWithScope(message.ScopeUid, message.TestUids);
    }

    async Task ConsumeTestExecutionBindingMessage(AllureTestExecutionBindingMessage message)
    {
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .BindTestExecution(message.TestNodeUid, message.ExecutionUid);
    }

    async Task ConsumeTestExecutionFinishMessage(AllureTestExecutionFinishMessage message)
    {
        this.AllureLifecycleState.GetOrCreate(message.CorrelationUid)
            .FinishTestExecution(message.ExecutionUid);
    }

    void StartTest(string name)
    {
        var testResult = TestResults.Create(name, this.Configuration, Environment.GetEnvironmentVariables());
        this.LifecycleApi.StartTest(testResult);
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
        TestAttachments.SaveFile(
            this.ResultsDestination,
            testResult,
            fileArtifact.DisplayName,
            fileArtifact.FileInfo
        );
        return testResult;
    }

    TestResult ApplyStdoutProperty(TestResult testResult, StandardOutputProperty stdout)
    {
        TestAttachments.SaveText(
            this.ResultsDestination,
            testResult,
            "Standard output",
            stdout.StandardOutput
        );
        return testResult;
    }

    TestResult ApplyStderrProperty(TestResult testResult, StandardErrorProperty stderr)
    {
        TestAttachments.SaveText(
            this.ResultsDestination,
            testResult,
            "Standard error",
            stderr.StandardError
        );
        return testResult;
    }

    static TestResult ApplyTimingProperty(TestResult testResult, TimingProperty timing)
    {
        TestResults.ApplyTimings(testResult, timing);
        return testResult;
    }

    static void ApplyFallbacks(TestResult testResult, TestNode node)
    {
        testResult.FullName ??= node.Uid;
        testResult.ApplyDefaultSuites();
    }

    static TestResult ApplyTestNodeStateProperty(
        ImmutableList<string> failExceptions,
        TestResult testResult,
        TestNodeStateProperty testNodeState
    )
    {
        testResult.ApplyStateAsFallback(failExceptions, testNodeState);
        return testResult;
    }

    static TestResult ApplyTestMethodIdentifierProperty(TestResult testResult, TestMethodIdentifierProperty identifierProperty)
    {
        testResult.ApplyIdentityAsFallback(identifierProperty);
        return testResult;
    }

    void FinalizeTestResult(IAllureRuntimeBase runtime, TestNode node)
    {
        bool isCancelled = false;
        runtime.ModelApi.UpdateTestResult((testResult) =>
        {
            this.ApplyProperties(testResult, node);
            if (testResult.IsCancelled)
            {
                isCancelled = true;
                return;
            }
            ApplyFallbacks(testResult, node);
        });

        var testResult = runtime.LifecycleApi.StopTest();

        if (!isCancelled)
        {
            runtime.ResultsDestination.WriteTestResult(testResult);
        }
    }
}
