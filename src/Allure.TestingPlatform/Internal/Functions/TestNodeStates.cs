using System.Collections.Generic;
using Allure.Model;
using Allure.Sdk.Functions;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Functions;

static class TestNodeStates
{
    public static Status ToStatus(
        IEnumerable<string> failExceptions,
        TestNodeStateProperty state
    ) =>
        state switch
        {
            FailedTestNodeStateProperty => Status.Failed,
            PassedTestNodeStateProperty => Status.Passed,
            SkippedTestNodeStateProperty => Status.Skipped,
            TimeoutTestNodeStateProperty => Status.Broken,
            ErrorTestNodeStateProperty { Exception: { } exception } =>
                ErrorStatus.Resolve(failExceptions, exception),
            ErrorTestNodeStateProperty => Status.Broken,
            _ => Status.Unknown,
        };

    public static StatusDetails? ToStatusDetails(TestNodeStateProperty state) =>
        state switch
        {
            FailedTestNodeStateProperty { Exception: { } exception } =>
                StatusDetails.FromException(exception),

            ErrorTestNodeStateProperty { Exception: { } exception } =>
                StatusDetails.FromException(exception),

            TimeoutTestNodeStateProperty { Exception: { } exception } =>
                StatusDetails.FromException(exception),

            TimeoutTestNodeStateProperty { Explanation: null } =>
                new() { Message = "The test has timed out." },

            { Explanation: { Length: > 0 } explanation } =>
                new() { Message = explanation },

            _ => null,
        };
}
