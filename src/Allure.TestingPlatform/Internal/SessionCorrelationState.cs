using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Internal;

class SessionCorrelationState(ICorrelationService correlationService)
{
    readonly Dictionary<SessionUid, CorrelationUid> map = [];

    readonly HashSet<CorrelationUid> correlations = [];

    readonly Dictionary<SessionUid, Queue<(int, DataWithSessionUid)>> sessionUidBuffers = [];

    readonly Dictionary<CorrelationUid, Queue<(int, DataWithCorrelationUid)>> correlationUidBuffers = [];

    int sequenceNumber = 0;

    public async Task<CorrelationResult> Correlate(
        IDataProducer dataProducer,
        IData message,
        CancellationToken cancellationToken
    ) =>
        message switch
        {
            DataWithSessionUid dataWithSessionUid =>
                await this.Correlate(dataProducer, dataWithSessionUid, cancellationToken),

            DataWithCorrelationUid dataWithCorrelationUid =>
                this.Correlate(dataWithCorrelationUid),

            _ => CorrelationResult.Failure,
        };

    public CorrelationUid? RemoveSessionData(SessionUid sessionUid)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.map, sessionUid, out CorrelationUid correlationUid))
        {
            this.correlations.Remove(correlationUid);
        }
        else
        {
            correlationUid = default;
        }

        var buffer = Dequeue(sessionUid, correlationUid).ToList();

        if (buffer.Any())
        {
            // TODO: use logger
            Console.Error.WriteLine(
                $"[Allure.TestingPlatform]: {buffer.Count} uncorrelated messages have been thrown away."
            );
        }

        return correlationUid;
    }

    async Task<CorrelationResult> Correlate(
        IDataProducer dataProducer,
        DataWithSessionUid message,
        CancellationToken cancellationToken
    )
    {
        var sessionUid = message.SessionUid;

        if (this.map.TryGetValue(sessionUid, out var storedCorrelationUid))
        {
            // Already has correlation. Pass the message through
            return CorrelationResult.Success(storedCorrelationUid, [message]);
        }

        if (await correlationService.GetCorrelationAsync(dataProducer, message, cancellationToken) is CorrelationUid correlationUid)
        {
            // New correlation to remember.
            this.map[sessionUid] = correlationUid;
            this.correlations.Add(correlationUid);

            // dequeue both buffers and restore the order of buffered messages
            return CorrelationResult.Success(
                correlationUid,
                [..this.Dequeue(sessionUid, correlationUid), message]
            );
        }

        // Can't correlate yet. Buffer the message and yield nothing to process
        Enqueue(message);

        return CorrelationResult.Failure;
    }

    CorrelationResult Correlate(DataWithCorrelationUid message)
    {
        var correlationUid = message.CorrelationUid;

        if (this.correlations.Contains(correlationUid) || !this.sessionUidBuffers.Any())
        {
            // Correlation either exists or not needed so far.
            return CorrelationResult.Success(correlationUid, [message]);
        }

        // No correlation exists and there are uncorrelated sessions.
        // Buffer the message and wait until the correlation is established
        // to prevent out of order delivery.
        Enqueue(message);
        return CorrelationResult.Failure;
    }

    void Enqueue(DataWithSessionUid dataWithSessionUid) =>
        this.Enqueue(
            this.sessionUidBuffers,
            dataWithSessionUid.SessionUid,
            dataWithSessionUid
        );

    void Enqueue(DataWithCorrelationUid dataWithCorrelationUid) =>
        this.Enqueue(
            this.correlationUidBuffers,
            dataWithCorrelationUid.CorrelationUid,
            dataWithCorrelationUid
        );

    void Enqueue<TKey, TValue>(
        Dictionary<TKey, Queue<(int, TValue)>> bufferMap,
        TKey key,
        TValue value
    )
    {
        var item = (this.GetSequenceNumber(), value);
        if (bufferMap.TryGetValue(key, out var buffer))
        {
            buffer.Enqueue(item);
        }
        else
        {
            bufferMap[key] = new([item]);
        }
    }

    IEnumerable<IData> Dequeue(
        SessionUid sessionUid,
        CorrelationUid correlationUid
    ) =>
        CollectionAlgorithms.MergeSortedByItem1<DataWithSessionUid, DataWithCorrelationUid, IData>(
            CollectionAlgorithms.RemoveAndGet(this.sessionUidBuffers, sessionUid, []),
            CollectionAlgorithms.RemoveAndGet(this.correlationUidBuffers, correlationUid, [])
        );

    int GetSequenceNumber() => this.sequenceNumber++;
}