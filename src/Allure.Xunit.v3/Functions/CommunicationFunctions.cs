using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Messages;
using Allure.TestingPlatform.Sdk.Properties;
using Xunit.Runner.Common;
using Xunit.Sdk;

namespace Allure.Xunit.Functions;

static class CommunicationFunctions
{
    public static bool TryGetCorrelationUid(
        IReadOnlyDictionary<string, IReadOnlyCollection<string>> xunitTraits,
        [NotNullWhen(true)] out CorrelationUid? correlationUid
    )
    {
        if (xunitTraits.TryGetValue(TestNodeMetadataCorrelationStrategy.MetadataKey, out var metadataValue)
            && metadataValue.Count == 1)
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
        if (testCaseStarting is { Traits: { } traits, TestCaseUniqueID: var scopeUid }
            && TryGetCorrelationUid(traits, out var correlationUid))
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
        if (testCaseFinished is { TestCaseUniqueID: var scopeUid }
            && metadataCache.TryGetTestCaseMetadata(scopeUid) is { Traits: { } traits }
            && TryGetCorrelationUid(traits, out var correlationUid))
        {
            allureScopeStop = new(correlationUid.Value, new(scopeUid));
            return true;
        }
        allureScopeStop = null;
        return false;
    }

    public static bool TryConvertToTestUpdateWithMethod(
        MethodInfo testMethod,
        string testCaseUniqueID,
        object?[]? arguments,
        MessageMetadataCache metadataCache,
        [NotNullWhen(true)] out AllureTestUpdateMessage? allureTestUpdate
    )
    {
        if (metadataCache.TryGetTestCaseMetadata(testCaseUniqueID) is { Traits: { } traits }
            && TryGetCorrelationUid(traits, out var correlationUid))
        {
            allureTestUpdate = new AllureTestUpdateMessage(correlationUid.Value, new(testCaseUniqueID))
            {
                Properties = [
                    new AllureTestMethodProperty(testMethod) { Arguments = [.. arguments ?? []] },
                    new AllureDefaultSuitesProperty(testMethod.DeclaringType),
                ]
            };
            return true;
        }

        allureTestUpdate = null;
        return false;
    }
}