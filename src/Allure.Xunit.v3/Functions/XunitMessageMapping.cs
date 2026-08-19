using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Allure.Model;
using Allure.TestingPlatform.Sdk.Correlation;
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
        if (xunitTraits.TryGetValue(TestNodeMetadataCorrelationStrategy.MetadataKey, out var metadataValue)
            && metadataValue.Count == 1
        )
        {
            correlationUid = new(metadataValue.First());
            return true;
        }

        correlationUid = default;
        return false;
    }

    public static bool TryConvertToAllureScopeStartMessage(
        ITestCaseStarting testCaseStarting,
        [NotNullWhen(true)] out AllureScopeStartMessage? allureScopeStart
    )
    {
        if (testCaseStarting is { Traits: { } traits, TestCaseUniqueID: { } scopeUid }
            && TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            allureScopeStart = new(correlationUid, new(scopeUid));
            return true;
        }
        allureScopeStart = null;
        return false;
    }

    public static bool TryConvertToAllureScopeStopMessage(
        ITestCaseFinished testCaseFinished,
        MessageMetadataCache metadataCache,
        [NotNullWhen(true)] out AllureScopeStopMessage? allureScopeStop
    )
    {
        if (testCaseFinished is { TestCaseUniqueID: { } scopeUid }
            && metadataCache.TryGetTestCaseMetadata(scopeUid) is { Traits: { } traits }
            && TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            allureScopeStop = new(correlationUid, new(scopeUid));
            return true;
        }
        allureScopeStop = null;
        return false;
    }

    public static bool TryConvertToTestUpdateWithMethod(
        MethodInfo testMethod,
        ITest test,
        object?[]? arguments,
        [NotNullWhen(true)] out AllureTestUpdateMessage? allureTestUpdate
    )
    {
        if (TryGetCorrelationUid(test.Traits, out var correlationUid))
        {
            allureTestUpdate = new AllureTestUpdateMessage(correlationUid, new(test.TestCase.UniqueID))
            {
                Properties = [
                    new AllureTestMethodProperty(testMethod) { Arguments = [.. arguments ?? []] },
                    new AllureDefaultSuitesProperty(testMethod.DeclaringType),
                    new AllureLabelsProperty([Label.Thread()]),
                ]
            };
            return true;
        }

        allureTestUpdate = null;
        return false;
    }

    public static bool TryConvertToCancellation(
        ITest test,
        [NotNullWhen(true)] out AllureTestUpdateMessage? cancellation
    )
    {
        if (TryGetCorrelationUid(test.Traits, out var correlationUid))
        {
            cancellation = new AllureTestUpdateMessage(correlationUid, new(test.TestCase.UniqueID))
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
            && testFailed is { TestCaseUniqueID: { } testCaseUniqueId }
            && metadataCache.TryGetTestCaseMetadata(testFailed.TestCaseUniqueID) is { Traits: { } traits }
            && TryGetCorrelationUid(traits, out var correlationUid)
        )
        {
            allureTestUpdate = new AllureTestUpdateMessage(correlationUid, new(testCaseUniqueId))
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