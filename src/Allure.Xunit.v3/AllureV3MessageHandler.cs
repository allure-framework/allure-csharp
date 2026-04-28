using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.TestPlan;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Allure.Xunit;

internal sealed class AllureMessageSink(
    IRunnerLogger logger,
    IMessageSink diagnosticMessageSink
) : IRunnerReporterMessageHandler
{
    static bool s_loggedPatcherDisabled;
    static readonly ConcurrentDictionary<string, AllureContext> s_contextsByTestUniqueId = new();
    static readonly ConcurrentDictionary<string, byte> s_activeTestUniqueIds = new();

    static AllureTestPlan TestPlan => AllureLifecycle.Instance.TestPlan;

    static AllureContext AllureContext => AllureLifecycle.Instance.Context;

    readonly ConcurrentDictionary<string, AllureV3TestData> allureTestData = new();

    internal static bool TryGetStoredContext(string testUniqueId, out AllureContext context) =>
        s_contextsByTestUniqueId.TryGetValue(testUniqueId, out context!);

    internal static bool TryGetSingleStoredContext(out string testUniqueId, out AllureContext context)
    {
        if (s_contextsByTestUniqueId.Count == 1)
        {
            var entry = s_contextsByTestUniqueId.First();
            testUniqueId = entry.Key;
            context = entry.Value;
            return true;
        }

        testUniqueId = string.Empty;
        context = default!;
        return false;
    }

    internal static bool TryGetSingleActiveTestUniqueId(out string testUniqueId)
    {
        if (s_activeTestUniqueIds.Count == 1)
        {
            testUniqueId = s_activeTestUniqueIds.Keys.First();
            return true;
        }

        testUniqueId = string.Empty;
        return false;
    }

    internal static void SaveStoredContext(string testUniqueId, AllureContext context) =>
        s_contextsByTestUniqueId[testUniqueId] = context;

    public bool OnMessage(IMessageSinkMessage message)
    {
        diagnosticMessageSink?.OnMessage(message);

        if (!s_loggedPatcherDisabled)
        {
            s_loggedPatcherDisabled = true;
            logger.LogRaw("[allure] Patcher disabled for v3 — using message-based context only");
        }

        switch (message)
        {
            case ITestStarting testStarting:
                OnTestStarting(testStarting);
                break;

            case ITestPassed testPassed:
                OnTestPassed(testPassed);
                break;

            case ITestSkipped testSkipped:
                OnTestSkipped(testSkipped);
                break;

            case ITestFailed testFailed:
                OnTestFailed(testFailed);
                break;

            case ITestFinished testFinished:
                OnTestFinished(testFinished);
                break;
        }

        return true;
    }

    void OnTestStarting(ITestStarting message)
    {
        s_activeTestUniqueIds[message.TestUniqueID] = 0;

        var testData = GetOrCreateTestData(message.TestUniqueID);
        var method = ResolveTestMethod(message);

        var testResult = method is not null
            ? AllureXunitHelper.CreateTestResult(method, message.TestDisplayName)
            : CreateFallbackTestResult(message);

        testData.TestMethod = method;
        testData.TestResult = testResult;
        testData.IsSelected = true;
        testData.Context = AllureLifecycle.Instance.Context;

        testData.Context = AllureLifecycle.Instance.RunInContext(testData.Context, () =>
        {
            if (method is not null && !IsStaticTestMethod(method))
            {
                AllureXunitHelper.StartNewAllureContainer(method.DeclaringType?.FullName ?? method.DeclaringType?.Name ?? "unknown");
            }

            AllureXunitHelper.StartAllureTestCase(testResult);
        });

        SaveStoredContext(message.TestUniqueID, testData.Context);
        AllureLifecycle.Instance.RestoreContext(testData.Context);

        logger.LogRaw($"[allure] start {testResult.name}");
    }

    void OnTestPassed(ITestPassed message)
    {
        if (!allureTestData.TryGetValue(message.TestUniqueID, out var testData) || !testData.IsSelected)
        {
            return;
        }

        RefreshStoredContext(message.TestUniqueID, testData);

        UpdateTestContext(message.TestUniqueID, () =>
        {
            EnsureTestStarted(testData);
            AllureXunitHelper.ApplyTestSuccess(message.Output);
        });
    }

    void OnTestSkipped(ITestSkipped message)
    {
        if (!allureTestData.TryGetValue(message.TestUniqueID, out var testData) || !testData.IsSelected)
        {
            return;
        }

        RefreshStoredContext(message.TestUniqueID, testData);

        UpdateTestContext(message.TestUniqueID, () =>
        {
            EnsureTestStarted(testData);
            AllureXunitHelper.ApplyTestSkip(message.Reason);
        });
    }

    void OnTestFailed(ITestFailed message)
    {
        if (!allureTestData.TryGetValue(message.TestUniqueID, out var testData) || !testData.IsSelected)
        {
            return;
        }

        RefreshStoredContext(message.TestUniqueID, testData);

        UpdateTestContext(message.TestUniqueID, () =>
        {
            EnsureTestStarted(testData);
            AllureXunitHelper.ApplyTestFailure(message);
        });
    }

    void OnTestFinished(ITestFinished message)
    {
        if (!allureTestData.TryRemove(message.TestUniqueID, out var testData) || !testData.IsSelected)
        {
            s_activeTestUniqueIds.TryRemove(message.TestUniqueID, out _);
            s_contextsByTestUniqueId.TryRemove(message.TestUniqueID, out _);
            return;
        }

        RefreshStoredContext(message.TestUniqueID, testData);

        AllureLifecycle.Instance.RunInContext(testData.Context, () =>
        {
            if (testData.TestMethod is null)
            {
                testData.TestMethod = ResolveTestMethod(message);
            }

            var arguments = testData.Arguments ?? GetArguments(message);
            AddAllureParameters(testData.TestMethod, arguments);
            if (testData.TestMethod is not null)
            {
                AllureXunitHelper.ApplyDefaultSuites(testData.TestMethod);
            }
            AllureXunitHelper.ReportCurrentTestCase();

            if (testData.TestMethod is not null && !IsStaticTestMethod(testData.TestMethod) && AllureContext.HasContainer)
            {
                AllureXunitHelper.ReportCurrentTestContainer();
            }
        });

        s_activeTestUniqueIds.TryRemove(message.TestUniqueID, out _);
        s_contextsByTestUniqueId.TryRemove(message.TestUniqueID, out _);
        logger.LogRaw($"[allure] finish {message.TestUniqueID}");
    }

    public ValueTask DisposeAsync()
    {
        foreach (var kv in allureTestData.ToArray())
        {
            if (!allureTestData.TryRemove(kv.Key, out var data))
            {
                continue;
            }

            if (!data.IsSelected)
            {
                continue;
            }

            AllureLifecycle.Instance.RunInContext(data.Context, () =>
            {
                if (AllureContext.HasTest)
                {
                    AllureXunitHelper.ReportCurrentTestCase();
                }

                if (AllureContext.HasContainer)
                {
                    AllureXunitHelper.ReportCurrentTestContainer();
                }
            });

            s_activeTestUniqueIds.TryRemove(kv.Key, out _);
            s_contextsByTestUniqueId.TryRemove(kv.Key, out _);
        }

        return ValueTask.CompletedTask;
    }

    static object[] GetArguments(object message)
    {
        var argsProperty = message.GetType().GetProperty("TestMethodArguments", BindingFlags.Instance | BindingFlags.Public)
            ?? message.GetType().GetProperty("Arguments", BindingFlags.Instance | BindingFlags.Public);

        return argsProperty?.GetValue(message) as object[] ?? [];
    }

    AllureV3TestData GetOrCreateTestData(string testUniqueId)
    {
        if (!allureTestData.TryGetValue(testUniqueId, out var data))
        {
            data = new AllureV3TestData();
            allureTestData[testUniqueId] = data;
        }

        return data;
    }

    void AddAllureParameters(MethodInfo? method, object[]? arguments)
    {
        if (method is null)
        {
            return;
        }

        arguments ??= [];

        var parameters = method.GetParameters();
        if (parameters.Length == 0)
        {
            return;
        }

        if (!arguments.Any())
        {
            return;
        }

        AllureXunitHelper.ApplyTestParameters(method, arguments);
    }

    void CaptureTestContext(string testUniqueId)
    {
        var data = GetOrCreateTestData(testUniqueId);
        data.Context = AllureContext;
        SaveStoredContext(testUniqueId, data.Context);
        AllureLifecycle.Instance.RestoreContext(data.Context);
    }

    AllureContext RunInTestContext(string testUniqueId, Action action)
    {
        var data = GetOrCreateTestData(testUniqueId);
        var updatedContext = AllureLifecycle.Instance.RunInContext(data.Context, action);
        data.Context = updatedContext;
        SaveStoredContext(testUniqueId, updatedContext);
        return updatedContext;
    }

    void UpdateTestContext(string testUniqueId, Action action)
    {
        _ = RunInTestContext(testUniqueId, action);
    }

    static void EnsureTestStarted(AllureV3TestData testData)
    {
        if (AllureContext.HasTest)
        {
            return;
        }

        if (testData.TestResult is not null)
        {
            AllureXunitHelper.StartAllureTestCase(testData.TestResult);
        }
    }

    static bool IsStaticTestMethod(MethodInfo method) => method.IsStatic;

    static MethodInfo? ResolveTestMethod(ITestStarting testStarting)
    {
        var testClassName = GetStringProperty(testStarting, "TestClassName");
        var testMethodName = GetStringProperty(testStarting, "TestMethodName")
            ?? GetStringProperty(testStarting, "MethodName");

        var resolved = ResolveTestMethodByClassAndName(testStarting, testClassName, testMethodName);
        if (resolved is not null)
        {
            return resolved;
        }

        return ResolveTestMethodFromDisplayName(testStarting.TestDisplayName);
    }

    static MethodInfo? ResolveTestMethod(ITestFinished testFinished)
    {
        var testClassName = GetStringProperty(testFinished, "TestClassName");
        var testMethodName = GetStringProperty(testFinished, "TestMethodName")
            ?? GetStringProperty(testFinished, "MethodName");

        var resolved = ResolveTestMethodByClassAndName(testFinished, testClassName, testMethodName);
        if (resolved is not null)
        {
            return resolved;
        }

        return ResolveTestMethodFromDisplayName(GetStringProperty(testFinished, "TestDisplayName"));
    }

    static MethodInfo? ResolveTestMethodFromDisplayName(string? displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
        {
            return null;
        }

        var normalized = displayName;
        var argsIndex = normalized.IndexOf('(');
        if (argsIndex > 0)
        {
            normalized = normalized.Substring(0, argsIndex);
        }

        var lastDot = normalized.LastIndexOf('.');
        if (lastDot <= 0 || lastDot >= normalized.Length - 1)
        {
            return null;
        }

        var testClassName = normalized.Substring(0, lastDot);
        var testMethodName = normalized.Substring(lastDot + 1);

        return ResolveTestMethodByClassAndName(new object(), testClassName, testMethodName);
    }

    static MethodInfo? ResolveTestMethodByClassAndName(object message, string? testClassName, string? testMethodName)
    {
        if (string.IsNullOrEmpty(testClassName) || string.IsNullOrEmpty(testMethodName))
        {
            return null;
        }

        var testClass = ResolveTestClass(testClassName);
        if (testClass is null)
        {
            return null;
        }

        var candidates = testClass
            .GetMethods(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance | BindingFlags.Static)
            .Where(method => string.Equals(method.Name, testMethodName, StringComparison.Ordinal))
            .ToArray();

        if (candidates.Length <= 1)
        {
            return candidates.FirstOrDefault();
        }

        var metadataToken = GetInt32Property(message, "TestMethodMetadataToken");
        return metadataToken is null
            ? candidates.FirstOrDefault()
            : candidates.FirstOrDefault(method => method.MetadataToken == metadataToken.Value);
    }

    static TestResult CreateFallbackTestResult(ITestStarting message)
    {
        var testClassName = GetStringProperty(message, "TestClassName");
        var testMethodName = GetStringProperty(message, "TestMethodName")
            ?? GetStringProperty(message, "MethodName");

        var labels = new System.Collections.Generic.List<Label>
        {
            Label.Thread(),
            Label.Host(),
            Label.Language(),
            Label.Framework("xUnit.net v3")
        };

        labels.AddRange(Allure.Net.Commons.Functions.ModelFunctions.EnumerateEnvironmentLabels());
        labels.AddRange(Allure.Net.Commons.Functions.ModelFunctions.EnumerateGlobalLabels());

        if (!string.IsNullOrEmpty(testClassName))
        {
            labels.Add(Label.TestClass(testClassName));
            labels.Add(Label.Package(testClassName));
        }

        if (!string.IsNullOrEmpty(testMethodName))
        {
            labels.Add(Label.TestMethod(testMethodName));
        }

        return new TestResult
        {
            uuid = Allure.Net.Commons.Functions.IdFunctions.CreateUUID(),
            name = message.TestDisplayName,
            fullName = message.TestUniqueID,
            labels = labels
        };
    }

    static Type? ResolveTestClass(string testClassName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(testClassName, throwOnError: false, ignoreCase: false);
            if (type is not null)
            {
                return type;
            }
        }

        return Type.GetType(testClassName, throwOnError: false, ignoreCase: false);
    }

    static string? GetStringProperty(object source, string propertyName) =>
        source
            .GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(source) as string;

    static int? GetInt32Property(object source, string propertyName)
    {
        var value = source
            .GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(source);

        if (value is int intValue)
        {
            return intValue;
        }

        return value as int?;
    }

    static void RefreshStoredContext(string testUniqueId, AllureV3TestData testData)
    {
        if (TryGetStoredContext(testUniqueId, out var storedContext))
        {
            testData.Context = storedContext;
            return;
        }

        if (AllureContext.HasTest || AllureContext.HasContainer || AllureContext.HasFixture)
        {
            testData.Context = AllureContext;
            SaveStoredContext(testUniqueId, testData.Context);
        }
    }

    sealed class AllureV3TestData
    {
        public bool IsSelected { get; set; }

        public TestResult? TestResult { get; set; }

        public MethodInfo? TestMethod { get; set; }

        public object[]? Arguments { get; set; }

        public AllureContext Context { get; set; } = new();
    }
}
