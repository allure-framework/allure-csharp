using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Allure.Xunit;

internal sealed class AllureV3MessageHandler(
    IRunnerLogger logger,
    IMessageSink diagnosticMessageSink
) : IRunnerReporterMessageHandler
{
    private readonly MessageMetadataCache _metadataCache = new();
    private readonly ConcurrentDictionary<string, AllureContext> _contexts = new();

    public ValueTask DisposeAsync() => ValueTask.CompletedTask;

    public bool OnMessage(IMessageSinkMessage message)
    {
        diagnosticMessageSink?.OnMessage(message);

        switch (message)
        {
            case ITestCaseStarting testCaseStarting:
                _metadataCache.Set(testCaseStarting);
                break;

            case ITestStarting testStarting:
                _metadataCache.Set(testStarting);
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
                _metadataCache.TryRemove(testFinished);
                break;
        }

        return true;
    }

    private void StartTest(ITestStarting testStarting)
    {
        var testCaseMetadata = _metadataCache.TryGetTestCaseMetadata(testStarting);
        var metadata = _metadataCache.TryGetTestMetadata(testStarting);

        var testResult = new TestResult
        {
            uuid = testStarting.TestUniqueID,
            name = metadata?.TestDisplayName ?? testCaseMetadata?.TestCaseDisplayName ?? testStarting.TestUniqueID,
            fullName = testStarting.TestUniqueID,
            labels =
            [
                Label.Language(),
                Label.Framework("xUnit.net v3"),
                Label.Thread(),
                Label.Host()
            ]
        };

        if (!string.IsNullOrEmpty(testCaseMetadata?.TestClassName))
        {
            testResult.labels.Add(Label.TestClass(testCaseMetadata.TestClassName));
            testResult.labels.Add(Label.Package(testCaseMetadata.TestClassName));
        }

        if (!string.IsNullOrEmpty(testCaseMetadata?.TestMethodName))
        {
            testResult.labels.Add(Label.TestMethod(testCaseMetadata.TestMethodName));
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
}
