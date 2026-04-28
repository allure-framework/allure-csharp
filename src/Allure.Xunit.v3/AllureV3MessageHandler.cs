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
    static bool s_loggedStartMessageShape;

    static AllureTestPlan TestPlan => AllureLifecycle.Instance.TestPlan;

    static AllureContext AllureContext => AllureLifecycle.Instance.Context;

    readonly ConcurrentDictionary<string, AllureV3TestData> allureTestData = new();

    public ValueTask DisposeAsync()
    {
        foreach (var kv in allureTestData.ToArray())
        {
            if (!allureTestData.TryRemove(kv.Key, out var data))
            {
                continue;
            }

            if (data.IsSelected)
            {
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
            }
        }

        return ValueTask.CompletedTask;
    }

    public bool OnMessage(IMessageSinkMessage message)
    {
        diagnosticMessageSink?.OnMessage(message);

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
        if (!s_loggedStartMessageShape)
        {
            s_loggedStartMessageShape = true;
            try
            {
                var props = message
                    .GetType()
                    .GetProperties(BindingFlags.Instance | BindingFlags.Public)
                    .Select(static p => p.Name)
                    .ToArray();
                logger.LogRaw("[allure] ITestStarting properties: " + string.Join(",", props));
            }
            catch
            {
            }
        }

        var testData = GetOrCreateTestData(message.TestUniqueID);
        var method = ResolveTestMethod(message);

        var testResult = method is not null
            ? AllureXunitHelper.CreateTestResult(method, message.TestDisplayName)
            : CreateFallbackTestResult(message);
        var isSelected = true;

        testData.TestMethod = method;
        testData.TestResult = testResult;
        testData.IsSelected = isSelected;

        if (!isSelected)
        {
            return;
        }

        if (method is not null && !IsStaticTestMethod(method))
        {
            AllureXunitHelper.StartNewAllureContainer(method.DeclaringType?.FullName ?? method.DeclaringType?.Name ?? "unknown");
        }

        AllureXunitHelper.StartAllureTestCase(testResult);
        CaptureTestContext(message.TestUniqueID);

        logger.LogRaw($"[allure] start {testResult.name}");
    }

    void OnTestPassed(ITestPassed message)
    {
        if (!allureTestData.TryGetValue(message.TestUniqueID, out var testData) || !testData.IsSelected)
        {
            return;
        }

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
            return;
        }

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

        logger.LogRaw($"[allure] finish {message.TestUniqueID}");
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

        var parameters = method.GetParameters();
        arguments ??= [];

        if (parameters.Any() && !arguments.Any())
        {
            return;
        }

        AllureXunitHelper.ApplyTestParameters(method, arguments);
    }

    void CaptureTestContext(string testUniqueId)
    {
        var data = GetOrCreateTestData(testUniqueId);
        data.Context = AllureContext;
        AllureLifecycle.Instance.RestoreContext(data.Context);
    }

    AllureContext RunInTestContext(string testUniqueId, Action action)
    {
        var data = GetOrCreateTestData(testUniqueId);
        var updatedContext = AllureLifecycle.Instance.RunInContext(data.Context, action);
        data.Context = updatedContext;
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

        return ResolveTestMethod(testStarting, testClassName, testMethodName);
    }

    static MethodInfo? ResolveTestMethod(ITestFinished testFinished)
    {
        var testClassName = GetStringProperty(testFinished, "TestClassName");
        var testMethodName = GetStringProperty(testFinished, "TestMethodName")
            ?? GetStringProperty(testFinished, "MethodName");

        return ResolveTestMethod(testFinished, testClassName, testMethodName);
    }

    static MethodInfo? ResolveTestMethod(object message, string? testClassName, string? testMethodName)
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

    sealed class AllureV3TestData
    {
        public bool IsSelected { get; set; }

        public TestResult? TestResult { get; set; }

        public MethodInfo? TestMethod { get; set; }

        public object[]? Arguments { get; set; }

        public AllureContext Context { get; set; } = new();
    }
}
