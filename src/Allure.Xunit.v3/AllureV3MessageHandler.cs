using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Sdk;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Allure.Xunit;

internal sealed class AllureV3MessageHandler(
    IRunnerLogger logger,
    IMessageSink diagnosticMessageSink
) : IRunnerReporterMessageHandler
{
    private readonly ConcurrentDictionary<string, AllureContext> _contexts = new();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public bool OnMessage(IMessageSinkMessage message)
    {
        diagnosticMessageSink?.OnMessage(message);

        switch (message)
        {
            case ITestStarting testStarting:
                StartTest(testStarting);
                break;

            case ITestPassed testPassed:
                UpdateTest(testPassed, () =>
                {
                    AllureLifecycle.Instance.UpdateTestCase(result =>
                    {
                        result.status = Status.passed;
                        if (!string.IsNullOrEmpty(testPassed.Output))
                        {
                            var details = result.statusDetails ??= new();
                            details.message = testPassed.Output;
                        }
                    });
                });
                break;

            case ITestSkipped testSkipped:
                UpdateTest(testSkipped, () =>
                {
                    AllureLifecycle.Instance.UpdateTestCase(result =>
                    {
                        result.status = Status.skipped;
                        var details = result.statusDetails ??= new();
                        details.message = testSkipped.Reason;
                    });
                });
                break;

            case ITestFailed testFailed:
                UpdateTest(testFailed, () =>
                {
                    AllureLifecycle.Instance.UpdateTestCase(result =>
                    {
                        result.status = testFailed.Cause == FailureCause.Assertion
                            ? Status.failed
                            : Status.broken;

                        var details = result.statusDetails ??= new();
                        details.message = ExceptionUtility.CombineMessages(testFailed);
                        details.trace = ExceptionUtility.CombineStackTraces(testFailed);
                    });
                });
                break;

            case ITestFinished testFinished:
                FinishTest(testFinished);
                break;
        }

        return true;
    }

    private void StartTest(ITestStarting testStarting)
    {
        var testClassName = GetStringProperty(testStarting, "TestClassName");
        var testMethodName = GetStringProperty(testStarting, "TestMethodName")
            ?? GetStringProperty(testStarting, "MethodName");
        var method = ResolveTestMethod(testStarting, testClassName, testMethodName);

        var testResult = new TestResult
        {
            uuid = testStarting.TestUniqueID,
            name = testStarting.TestDisplayName,
            fullName = testStarting.TestUniqueID,
            labels =
            [
                Label.Language(),
                Label.Framework("xUnit.net v3"),
                Label.Thread(),
                Label.Host()
            ]
        };

        if (!string.IsNullOrEmpty(testClassName))
        {
            testResult.labels.Add(Label.TestClass(testClassName));
            testResult.labels.Add(Label.Package(testClassName));
        }

        if (!string.IsNullOrEmpty(testMethodName))
        {
            testResult.labels.Add(Label.TestMethod(testMethodName));
        }

        if (method is not null)
        {
            AllureApiAttribute.ApplyAllAttributes(method, testResult);

            var legacyIdAttrs = method.GetCustomAttributes<global::Allure.Xunit.Attributes.AllureIdAttribute>();
            foreach (var attr in legacyIdAttrs)
            {
                testResult.labels.Add(new Label { name = "ALLURE_ID", value = attr.AllureId });
            }
        }

        var context = AllureLifecycle.Instance.RunInContext(new AllureContext(), () =>
        {
            AllureLifecycle.Instance.StartTestCase(testResult);
        });

        _contexts[testStarting.TestUniqueID] = context;
        logger.LogRaw($"[allure] start {testResult.name}");
    }

    private void UpdateTest(ITestMessage message, Action action)
    {
        if (_contexts.TryGetValue(message.TestUniqueID, out var context))
        {
            var updatedContext = AllureLifecycle.Instance.RunInContext(context, action);
            _contexts[message.TestUniqueID] = updatedContext;
        }
    }

    private void FinishTest(ITestFinished testFinished)
    {
        if (_contexts.TryRemove(testFinished.TestUniqueID, out var context))
        {
            AllureLifecycle.Instance.RunInContext(context, () =>
            {
                AllureLifecycle.Instance.StopTestCase();
                AllureLifecycle.Instance.WriteTestCase();
            });

            logger.LogRaw($"[allure] finish {testFinished.TestUniqueID}");
        }
    }

    private static MethodInfo? ResolveTestMethod(
        ITestStarting testStarting,
        string? testClassName,
        string? testMethodName
    )
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

        var metadataToken = GetInt32Property(testStarting, "TestMethodMetadataToken");
        return metadataToken is null
            ? candidates.FirstOrDefault()
            : candidates.FirstOrDefault(method => method.MetadataToken == metadataToken.Value);
    }

    private static Type? ResolveTestClass(string testClassName)
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

    private static string? GetStringProperty(object source, string propertyName) =>
        source
            .GetType()
            .GetProperty(propertyName, BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(source) as string;

    private static int? GetInt32Property(object source, string propertyName)
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
}
