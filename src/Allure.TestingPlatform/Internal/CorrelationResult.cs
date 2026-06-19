using System.Collections.Generic;
using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Internal;

record class CorrelationResult
{
    public static CorrelationSuccess Success(
        CorrelationUid correlationUid,
        IEnumerable<IData> messagesToProcess
    ) => new(correlationUid, messagesToProcess);

    public static CorrelationFailure Failure { get; } =
        new CorrelationFailure();
};

sealed record class CorrelationSuccess(
    CorrelationUid CorrelationUid,
    IEnumerable<IData> MessagesToProcess
) : CorrelationResult;

sealed record class CorrelationFailure : CorrelationResult;
