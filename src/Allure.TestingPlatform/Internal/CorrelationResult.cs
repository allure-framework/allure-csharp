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

    public static CorrelationNotReady NotReady { get; } =
        new();

    public static CorrelationFailure Failure(string message) =>
        new(message);
};

sealed record class CorrelationSuccess(CorrelationUid CorrelationUid, IEnumerable<IData> MessagesToProcess) :
    CorrelationResult;

sealed record class CorrelationNotReady : CorrelationResult;

sealed record class CorrelationFailure(string Message) : CorrelationResult;
