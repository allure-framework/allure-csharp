using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Allure.Model;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Xunit.Runner.Common;
using Xunit.Sdk;

using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Xunit.Functions;

static class XunitMessageMapping
{
    public static bool TryGetCorrelationUid(
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> xunitTraits,
        [MaybeNullWhen(false)] out CorrelationUid correlationUid
    )
    {
        if (xunitTraits.TryGetValue(
                TestNodeMetadataCorrelationStrategy.MetadataKey,
                out var metadataValue
            )
                && metadataValue.Count == 1
        )
        {
            correlationUid = new(metadataValue.First());
            return true;
        }

        correlationUid = default;
        return false;
    }

    public static IEnumerable<AllureCorrelatedMessage> GetMessagesForTestStartingSinkEvent(
        ITestStarting testStarting
    )
    {
        if (testStarting is
            {
                Traits: { } traits,
                TestUniqueID: { } testUniqueId,
                TestCaseUniqueID: { } testCaseUniqueId,
            }
            && TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            ScopeExecutionStateUid scopeUid = new(testUniqueId);
            TestExecutionStateUid executionUid = new(testUniqueId);
            yield return new AllureTestExecutionBindingMessage(correlationUid, new(testCaseUniqueId), executionUid);
            yield return new AllureScopeStartMessage(correlationUid, scopeUid);
            yield return new AllureScopeTestsMessage(correlationUid, scopeUid, [executionUid]);
        }
    }

    public static IEnumerable<AllureCorrelatedMessage> GetMessagesForTestFinishedSinkEvent(
        ITestFinished testFinished,
        MessageMetadataCache metadataCache
    )
    {
        if (testFinished is { TestUniqueID: { } testUniqueId }
            && metadataCache.TryGetTestMetadata(testUniqueId) is { Traits: { } traits }
            && TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            yield return new AllureScopeStopMessage(correlationUid, new(testUniqueId));
            yield return new AllureTestExecutionFinishMessage(correlationUid, new(testUniqueId));
        }
    }

    public static IEnumerable<AllureCorrelatedMessage> GetMessagesForTestStartingAttributeEvent(
        MethodInfo testMethod,
        ITest test,
        object?[]? arguments
    )
    {
        if (TryGetCorrelationUid(test.Traits, out var correlationUid))
        {
            TestExecutionStateUid executionUid = new(test.UniqueID);
            yield return new AllureTestUpdateMessage(correlationUid, executionUid)
            {
                Properties = [
                    new AllureTestMethodProperty(testMethod) { Arguments = [.. arguments ?? []] },
                    new AllureDefaultSuitesProperty(testMethod.DeclaringType),
                    new AllureLabelsProperty([Label.Thread()]),
                ]
            };
        }
    }

    public static bool TryConvertToCancellation(
        ITest test,
        [NotNullWhen(true)] out AllureTestUpdateMessage? cancellation
    )
    {
        if (TryGetCorrelationUid(test.Traits, out var correlationUid))
        {
            cancellation = new AllureTestUpdateMessage(correlationUid, new(test.UniqueID))
            {
                Properties = [new AllureCancelProperty()],
            };
            return true;
        }

        cancellation = null;
        return false;
    }

    public static bool TryConvertToTestUpdateWithFailedStatus(
        ITestFailed testFailed,
        MessageMetadataCache metadataCache,
        [NotNullWhen(true)] out AllureTestUpdateMessage? allureTestUpdate
    )
    {
        if (ExceptionFunctions.IsConfiguredAssertionFailure(testFailed)
            && testFailed is { TestUniqueID: { } testUniqueUid }
            && metadataCache.TryGetTestMetadata(testUniqueUid) is { Traits: { } traits }
            && TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            allureTestUpdate = new AllureTestUpdateMessage(correlationUid, new(testUniqueUid))
            {
                Properties = [
                    new AllureStatusProperty<AllureTestResult>(Status.Failed),
                ]
            };
            return true;
        }

        allureTestUpdate = null;
        return false;
    }
}