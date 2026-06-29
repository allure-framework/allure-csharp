using System.Collections.Generic;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal.Correlation;

record class CorrelationResult
{
    public static CorrelationSuccess Success(
        CorrelationUid correlationUid,
        IEnumerable<IData> messagesToProcess
    ) => new(correlationUid, messagesToProcess);

    public static CorrelationNotReady NotReady { get; } =
        new();

    public static CorrelationFailure Failure(string message) =>
        new(message);
};
