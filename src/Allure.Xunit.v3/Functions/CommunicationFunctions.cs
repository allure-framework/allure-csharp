using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Xunit.Runner.Common;
using Xunit.Sdk;

using AllureTestResult = Allure.Net.Commons.TestResult;

namespace Allure.Xunit.Functions;

static class CommunicationFunctions
{
    public static bool TryGetCorrelationUid(
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> xunitTraits,
        [NotNullWhen(true)] out CorrelationUid? correlationUid
    )
    {
        if (xunitTraits.TryGetValue(TestNodeMetadataCorrelationStrategy.MetadataKey, out var metadataValue)
            && metadataValue.Count == 1
        )
        {
            correlationUid = new(metadataValue.First());
            return true;
        }
        correlationUid = null;
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
            allureScopeStart = new(correlationUid.Value, new(scopeUid));
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
            allureScopeStop = new(correlationUid.Value, new(scopeUid));
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
            allureTestUpdate = new AllureTestUpdateMessage(correlationUid.Value, new(test.TestCase.UniqueID))
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
            allureTestUpdate = new AllureTestUpdateMessage(correlationUid.Value, new(testCaseUniqueId))
            {
                Properties = [
                    new AllureStatusProperty<AllureTestResult>(Status.failed),
                ]
            };
            return true;
        }

        allureTestUpdate = null;
        return false;
    }
}