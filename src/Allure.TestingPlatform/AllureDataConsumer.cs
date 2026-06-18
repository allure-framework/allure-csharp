using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Internal;
using Allure.TestingPlatform.Messages;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform;

public class AllureDataConsumer : AllureMtpToggleableExtension, IDataConsumer
{
    readonly AllureDataConsumerState state;

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

    public AllureLifecycle Lifecycle => this.Allure.Lifecycle;

    readonly ImmutableArray<Func<IDataProducer, IData, CancellationToken, Task<bool>>> highLevelConsumeFunctions;

    readonly ImmutableArray<Func<IDataProducer, DataWithSessionUid, CancellationToken, Task<bool>>> nativeMessageConsumeFunctions;

    readonly ImmutableArray<Func<IDataProducer, DataWithCorrelationUid, CancellationToken, Task<bool>>> allureMessageConsumeFunctions;

    public AllureDataConsumer(IAllureInfrastructure allure) : base(
        "dd4f3277-5786-4010-8908-e70f07656ebc",
        "Allure.TestingPlatform data consumer",
        "Creates Allure results from Microsoft Testing Platform messages",
        allure
    )
    {
        this.state = new(allure.Lifecycle);
        this.nativeMessageConsumeFunctions = InitializeNativeMtpMessageConsumeFunctions().ToImmutableArray();
        this.allureMessageConsumeFunctions = InitializeAllureMessageConsumeFunctions().ToImmutableArray();
        this.highLevelConsumeFunctions = InitializeHighLevelConsumeFunctions().ToImmutableArray();
    }

    public async Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        await ApplyConsumeFunctionsOneByOne(
            this.highLevelConsumeFunctions,
            dataProducer,
            value,
            cancellationToken
        );
    }

    async Task<bool> TryConsumeMessage<TMessage>(IData message, Func<TMessage, Task> consume) where TMessage : IData
    {
        if (message is TMessage typedMessage)
        {
            await consume(typedMessage);
            return true;
        }
        return false;
    }

    async Task<bool> TryConsumeDataWithSessionUid<TMessage>(
        DataWithSessionUid message,
        Func<AllureMtpSessionState, TMessage, Task> consume
    )
        where TMessage : DataWithSessionUid
    {
        if (message is TMessage { SessionUid.Value: var sessionUid } typedMessage)
        {
            CorrelationUid correlationUid = new(sessionUid);
            await consume(
                this.state.GetOrCreateSessionState(correlationUid),
                typedMessage
            );
            return true;
        }
        return false;
    }

    async Task<bool> TryConsumeDataWithCorrelationUid<TMessage>(
        DataWithCorrelationUid message,
        Func<AllureMtpSessionState, TMessage, Task> consume
    )
        where TMessage : DataWithCorrelationUid
    {
        if (message is TMessage { CorrelationUid: var correlationUid } typedMessage)
        {
            await consume(
                this.state.GetOrCreateSessionState(correlationUid),
                typedMessage
            );
            return true;
        }
        return false;
    }

    static async Task ApplyConsumeFunctionsOneByOne<TMessage>(
        IEnumerable<Func<IDataProducer, TMessage, CancellationToken, Task<bool>>> functions,
        IDataProducer dataProducer,
        TMessage message,
        CancellationToken cancellationToken
    )
        where TMessage : IData
    {
        foreach (var consume in functions)
        {
            if (await consume(dataProducer, message, cancellationToken))
            {
                return;
            }
        }
    }

    async Task ConsumeTestNodeUpdateMessage(AllureMtpSessionState state, TestNodeUpdateMessage message)
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
            ApplyProperties(testResult, node);
            ApplyFallbacks(testResult, node);
        });

        this.Lifecycle
            .StopTestCase()
            .WriteTestCase();

        state.RemoveTestContext(testContextUid);
    }

    async Task ConsumeSessionFileArtifactMessage(AllureMtpSessionState _, SessionFileArtifact message) =>
        AllureApi.AddGlobalAttachment(message.DisplayName, null!, message.FileInfo.FullName);

    async Task ConsumeCreateContextMessage(AllureMtpSessionState state, CreateContextMessage message)
    {
        var parentContextUid = message.ParentContextUid;
        state.InheritContext(
            message.ContextUid,
            message.ParentContextUid,
            () => message.Mutate(this.Allure)
        );
    }

    async Task ConsumeMutateModelMessage(AllureMtpSessionState state, MutateModelMessage message)
    {
        var contextUid = message.ContextUid;
        state.UpdateContext(
            message.ContextUid,
            () => message.Mutate(this.Allure)
        );
    }

    async Task ConsumeRemoveContextMessage(AllureMtpSessionState state, RemoveContextMessage message) =>
        state.ReleaseContext(
            message.ContextUid,
            () => message.Mutate(this.Allure)
        );

    async Task ConsumeScopeStopMessage(AllureMtpSessionState state, AllureScopeStopMessage message) =>
        state.ReleaseScopeContext(
            message.ScopeUid,
            () => message.Mutate(this.Allure)
        );

    async Task ConsumeTestsInScopeMessage(AllureMtpSessionState state, AllureTestsScopeMessage message) =>
        state.AssociateTestsWithScope(message.ScopeUid, message.TestUids);

    static void EnterTestScopeContext(AllureMtpSessionState state, TestContextUid testContextUid)
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
        var testResult = CreateTestResult(this.Allure.Config);
        this.Lifecycle.StartTestCase(testResult);
        return testResult;
    }

    static TestResult CreateTestResult(AllureConfiguration config) =>
        new()
        {
            uuid = IdFunctions.CreateUUID(),
            labels = [
                Label.Language(),
                Label.Host(),

                // TODO: Cover with tests
                ..ModelFunctions.EnumerateEnvironmentLabels(),
                ..ModelFunctions.EnumerateGlobalLabels(config),
            ],
        };

    static void ApplyProperties(TestResult testResult, TestNode node)
    {
        foreach (var property in node.Properties)
        {
            ApplyProperty(testResult, property);
        }
    }

    static TestResult ApplyProperty(TestResult testResult, IProperty property) =>
        property switch
        {
            TestNodeStateProperty testNodeState =>
                ApplyTestNodeStateProperty(testResult, testNodeState),

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
        // If present, TimingProperty is the ultimate source of truth about the timing.
        testResult.start = timing.GlobalTiming.StartTime.ToUnixTimeMilliseconds();
        testResult.stop = timing.GlobalTiming.EndTime.ToUnixTimeMilliseconds();
        return testResult;
    }

    static void ApplyFallbacks(TestResult testResult, TestNode node)
    {
        testResult.name ??= node.DisplayName;
        testResult.fullName ??= node.Uid;
    }

    static TestResult ApplyTestNodeStateProperty(TestResult testResult, TestNodeStateProperty testNodeState)
    {
        if (testResult.status == Status.none)
        {
            testResult.status = GetStatus(testNodeState);
        }

        testResult.statusDetails ??= GetStatusDetails(testNodeState);

        return testResult;
    }

    static TestResult ApplyTestMethodIdentifierProperty(TestResult testResult, TestMethodIdentifierProperty identifierProperty)
    {
        var sb = new StringBuilder();
        List<string> titlePath = [];
        var assembly = identifierProperty.AssemblyFullName;
        if (assembly is not null)
        {
            if (assembly.Contains(','))
            {
                assembly = new AssemblyName(assembly).Name;
            }
            sb.Append(assembly);
            sb.Append(":");
            titlePath.Add(assembly);
        }

        var @namespace = identifierProperty.Namespace;
        if (@namespace is not null)
        {
            sb.Append(@namespace);
            sb.Append(".");
            titlePath.AddRange(@namespace.Split('.'));
        }

        var typeName = identifierProperty.TypeName;
        if (typeName is not null)
        {
            sb.Append(typeName);
            sb.Append(".");
            titlePath.Add(typeName);
        }

        var methodName = identifierProperty.MethodName;
        if (methodName is not null)
        {
            sb.Append(methodName);
        }

        var parameterTypes = string.Join(",", identifierProperty.ParameterTypeFullNames);
        sb.Append("(");
        sb.Append(parameterTypes);
        sb.Append(")");

        if (parameterTypes.Length > 0)
        {
            titlePath.Add($"{methodName}({parameterTypes})");
        }

        testResult.fullName ??= sb.ToString();

        if (testResult.titlePath.Count == 0)
        {
            testResult.titlePath = titlePath;
        }

        ModelFunctions.EnsureSuites(testResult, assembly, @namespace, typeName);

        return testResult;
    }

    static Status GetStatus(TestNodeStateProperty state) =>
        state switch
        {
            FailedTestNodeStateProperty => Status.failed,
            PassedTestNodeStateProperty => Status.passed,
            SkippedTestNodeStateProperty => Status.skipped,
            TimeoutTestNodeStateProperty or ErrorTestNodeStateProperty => Status.broken,
            _ => Status.none,
        };

    static StatusDetails? GetStatusDetails(TestNodeStateProperty state) =>
        state switch
        {
            FailedTestNodeStateProperty { Exception: { } exception } =>
                ModelFunctions.ToStatusDetails(exception),

            ErrorTestNodeStateProperty { Exception: { } exception } =>
                ModelFunctions.ToStatusDetails(exception),

            TimeoutTestNodeStateProperty { Exception: { } exception } =>
                ModelFunctions.ToStatusDetails(exception),

            TimeoutTestNodeStateProperty { Explanation: null } =>
                new(){ message = "The test has timed out." },

            _ => new () { message = state.Explanation },
        };

    IEnumerable<Func<IDataProducer, DataWithSessionUid, CancellationToken, Task<bool>>> InitializeNativeMtpMessageConsumeFunctions() => [
        async (_, data, _) => await this.TryConsumeDataWithSessionUid<TestNodeUpdateMessage>(
            data,
            this.ConsumeTestNodeUpdateMessage),
        async (_, data, _) => await this.TryConsumeDataWithSessionUid<SessionFileArtifact>(
            data,
            this.ConsumeSessionFileArtifactMessage),
    ];

    IEnumerable<Func<IDataProducer, DataWithCorrelationUid, CancellationToken, Task<bool>>> InitializeAllureMessageConsumeFunctions() => [
        async (_, data, _) => await this.TryConsumeDataWithCorrelationUid<CreateContextMessage>(
            data,
            this.ConsumeCreateContextMessage),
        async (_, data, _) => await this.TryConsumeDataWithCorrelationUid<MutateModelMessage>(
            data,
            this.ConsumeMutateModelMessage),
        async (_, data, _) => await this.TryConsumeDataWithCorrelationUid<AllureScopeStopMessage>(
            data,
            this.ConsumeScopeStopMessage),
        async (_, data, _) => await this.TryConsumeDataWithCorrelationUid<RemoveContextMessage>(
            data,
            this.ConsumeRemoveContextMessage),
        async (_, data, _) => await this.TryConsumeDataWithCorrelationUid<AllureTestsScopeMessage>(
            data,
            this.ConsumeTestsInScopeMessage),
    ];

    IEnumerable<Func<IDataProducer, IData, CancellationToken, Task<bool>>> InitializeHighLevelConsumeFunctions() => [
        async (producer, data, ct) => await TryConsumeMessage<DataWithSessionUid>(
            data,
            (typedData) => ApplyConsumeFunctionsOneByOne(
                this.nativeMessageConsumeFunctions,
                producer,
                typedData,
                ct
            )
        ),
        async (producer, data, ct) => await TryConsumeMessage<DataWithCorrelationUid>(
            data,
            (typedData) => ApplyConsumeFunctionsOneByOne(
                this.allureMessageConsumeFunctions,
                producer,
                typedData,
                ct
            )
        ),
    ];
}