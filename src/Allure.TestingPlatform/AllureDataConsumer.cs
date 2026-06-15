using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.TestingPlatform.Internal;
using Allure.TestingPlatform.Messages;
using Microsoft.Testing.Platform.Extensions;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform;

public class AllureDataConsumer : IDataConsumer
{
    readonly IAllureInfrastructure allure;

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

    public string Uid { get; } = "dd4f3277-5786-4010-8908-e70f07656ebc";

    public string Version { get; } =
        Assembly
            .GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            .InformationalVersion
            .Split('+')
            .First();

    public string DisplayName { get; } = nameof(AllureDataConsumer);

    public string Description { get; } =
        "A data consumer extension that creates Allure data from Microsoft Testing Platform messages";

    public AllureLifecycle Lifecycle => this.allure.Lifecycle;

    readonly List<Func<IDataProducer, IData, CancellationToken, Task<bool>>> consumeFunctions;

    public AllureDataConsumer(IAllureInfrastructure allure)
    {
        this.allure = allure;
        this.state = new(allure.Lifecycle);

        this.consumeFunctions = [
            async (_, data, _) => this.TryConsumeMessage<TestNodeUpdateMessage>(
                data,
                this.ConsumeTestNodeUpdateMessage),
            async (_, data, _) => this.TryConsumeMessage<SessionFileArtifact>(
                data,
                (sfa) =>
                    AllureApi.AddGlobalAttachment(sfa.DisplayName, null!, sfa.FileInfo.FullName)),
            async (_, data, _) => this.TryConsumeMessage<CreateContextMessage>(
                data,
                this.ConsumeCreateContextMessage),
            async (_, data, _) => this.TryConsumeMessage<MutateModelMessage>(
                data,
                this.ConsumeMutateModelMessage),
            async (_, data, _) => this.TryConsumeMessage<RemoveContextMessage>(
                data,
                this.ConsumeRemoveContextMessage),
            async (_, data, _) => this.TryConsumeMessage<AllureTestsScopeMessage>(
                data,
                this.ConsumeTestsInScopeMessage),
        ];
    }

    public async Task ConsumeAsync(IDataProducer dataProducer, IData value, CancellationToken cancellationToken)
    {
        foreach (var consume in this.consumeFunctions)
        {
            if (await consume(dataProducer, value, cancellationToken))
            {
                return;
            }
        }
    }

    public Task<bool> IsEnabledAsync() => Task.FromResult(true);

    bool TryConsumeMessage<TMessage>(IData message, Action<TMessage> consume) where TMessage : IData
    {
        if (message is TMessage typedMessage)
        {
            consume(typedMessage);
            return true;
        }
        return false;
    }

    void ConsumeTestNodeUpdateMessage(TestNodeUpdateMessage message)
    {
        var session = message.SessionUid;
        var node = message.TestNode;
        var uid = node.Uid;
        var stateProperty = node.Properties
            .OfType<TestNodeStateProperty>()
            .SingleOrDefault();
        if (stateProperty is null or DiscoveredTestNodeStateProperty)
        {
            return;
        }

        if (this.state.TryGetContext(session, uid, out var ctx))
        {
            if (!ctx.HasTest && ctx.HasContainer)
            {
                // Has scope with the same UID, most likely - a scope for "before/after each" fixtures
                this.state.MakeUidShared(session, uid);
            }
            this.Lifecycle.RestoreContext(ctx);
        }

        if (stateProperty is InProgressTestNodeStateProperty)
        {
            this.StartTest(session, node);
            this.state.SetContext(session, uid, this.Lifecycle.Context);
            return;
        }

        if (!this.Lifecycle.Context.HasTest)
        {
            this.StartTest(session, node);
        }

        this.Lifecycle.UpdateTestCase((testResult) =>
        {
            ApplyProperties(testResult, node);
            ApplyFallbacks(testResult, node);
        });

        this.Lifecycle
            .StopTestCase()
            .WriteTestCase();

        this.state.RemoveTestContext(session, uid);
    }

    void ConsumeCreateContextMessage(CreateContextMessage message)
    {
        var parentContextUid = message.ParentContextUid;
        this.state.InheritContext(
            message.Session,
            message.ContextUid,
            message.ParentContextUid,
            () => message.Mutate(this.allure)
        );
    }

    void ConsumeMutateModelMessage(MutateModelMessage message)
    {
        var contextUid = message.ContextUid;
        this.state.UpdateContext(
            message.Session,
            message.ContextUid,
            () => message.Mutate(this.allure)
        );
    }

    void ConsumeRemoveContextMessage(RemoveContextMessage message)
    {
        Action<SessionUid, string, Action> release =
            message is AllureScopeStopMessage
                ? this.state.ReleaseScopeContext
                : this.state.ReleaseContext;

        release(message.Session, message.ContextUid, () => message.Mutate(this.allure));
    }

    void ConsumeTestsInScopeMessage(AllureTestsScopeMessage message)
    {
        this.state.AssociateTestsWithScope(
            message.SessionUid,
            message.ScopeUid,
            message.TestUids
        );
    }

    TestResult StartTest(SessionUid session, TestNode node)
    {
        var testResult = CreateTestResult(node);

        this.state.TryEnterTestScope(session, node.Uid);

        this.Lifecycle.StartTestCase(testResult);
        return testResult;
    }

    static TestResult CreateTestResult(TestNode node) =>
        new()
        {
            uuid = IdFunctions.CreateUUID(),
            labels = [
                Label.Language(),
                Label.Host(),
                // No Label.Thread as we can't tell here in which one the test has been run
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
}