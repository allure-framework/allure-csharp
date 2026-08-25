using System.Collections.Generic;
using System.Reflection;
using Allure.Model;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Xunit.Runner.Common;
using Xunit.Sdk;

using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Xunit.Internal.Functions;

static class AllureMessages
{
    public static IEnumerable<AllureCorrelatedMessage> ForTestStarting(
        ITestStarting testStarting
    )
    {
        if (testStarting is
            {
                Traits: { } traits,
                TestUniqueID: { } testUniqueId,
                TestCaseUniqueID: { } testCaseUniqueId,
            }
            && XunitTraits.TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            ScopeExecutionStateUid scopeUid = new(testUniqueId);
            TestExecutionStateUid executionUid = new(testUniqueId);
            yield return new AllureTestExecutionBindingMessage(
                correlationUid,
                new(testCaseUniqueId),
                executionUid
            );
            yield return new AllureScopeStartMessage(correlationUid, scopeUid);
            yield return new AllureScopeTestsMessage(
                correlationUid,
                scopeUid,
                [executionUid]
            );
        }
    }

    public static IEnumerable<AllureCorrelatedMessage> ForCancellation(ITest test)
    {
        if (XunitTraits.TryGetCorrelationUid(test.Traits, out var correlationUid))
        {
            yield return new AllureTestUpdateMessage(
                correlationUid,
                new(test.UniqueID)
            )
            {
                Properties = [new AllureCancelProperty()],
            };
        }
    }

    public static IEnumerable<AllureCorrelatedMessage> ForBeforeTest(
        MethodInfo testMethod,
        ITest test,
        object?[]? arguments
    )
    {
        if (XunitTraits.TryGetCorrelationUid(test.Traits, out var correlationUid))
        {
            TestExecutionStateUid executionUid = new(test.UniqueID);
            yield return new AllureTestUpdateMessage(correlationUid, executionUid)
            {
                Properties =
                [
                    new AllureTestMethodProperty(testMethod)
                    {
                        Arguments = [.. arguments ?? []],
                    },
                    new AllureDefaultSuitesProperty(testMethod.DeclaringType),
                    new AllureLabelsProperty([Label.Thread()]),
                ],
            };
        }
    }

    public static IEnumerable<AllureCorrelatedMessage> ForTestFailed(
        IEnumerable<string> failExceptions,
        ITestFailed testFailed,
        MessageMetadataCache metadataCache
    )
    {
        if (XunitTestFailures.IsAssertionFailure(failExceptions, testFailed)
            && testFailed is { TestUniqueID: { } testUniqueId }
            && metadataCache.TryGetTestMetadata(testUniqueId) is { Traits: { } traits }
            && XunitTraits.TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            yield return new AllureTestUpdateMessage(
                correlationUid,
                new(testUniqueId)
            )
            {
                Properties =
                [
                    new AllureStatusProperty<AllureTestResult>(Status.Failed),
                ],
            };
        }
    }

    public static IEnumerable<AllureCorrelatedMessage> ForTestFinished(
        ITestFinished testFinished,
        MessageMetadataCache metadataCache
    )
    {
        if (testFinished is { TestUniqueID: { } testUniqueId }
            && metadataCache.TryGetTestMetadata(testUniqueId) is { Traits: { } traits }
            && XunitTraits.TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            yield return new AllureScopeStopMessage(
                correlationUid,
                new(testUniqueId)
            );
            yield return new AllureTestExecutionFinishMessage(
                correlationUid,
                new(testUniqueId)
            );
        }
    }
}
