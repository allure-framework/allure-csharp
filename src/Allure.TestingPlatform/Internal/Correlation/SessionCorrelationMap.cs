using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Messages;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.Logging;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Internal.Correlation;

class SessionCorrelationMap(ICorrelationSource correlationService, ILogger logger)
{
    readonly Dictionary<SessionUid, CorrelationUid> correlatedSessions = [];

    readonly Dictionary<CorrelationUid, SessionUid> activeCorrelations = [];

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

            _ => CorrelationResult.NotReady,
        };

    public CorrelationUid? RemoveSessionData(SessionUid sessionUid)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.correlatedSessions, sessionUid, out CorrelationUid correlationUid))
        {
            this.activeCorrelations.Remove(correlationUid);
        }
        else
        {
            correlationUid = default;
        }

        var buffer = Dequeue(sessionUid, correlationUid).ToList();

        if (buffer.Any())
        {
            logger.LogError(
                $"[Allure.TestingPlatform]: {buffer.Count} uncorrelated messages were discarded "
                    + "because the corresponding MTP session had been finished."
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

        if (this.correlatedSessions.TryGetValue(sessionUid, out var storedCorrelationUid))
        {
            // Already has correlation. Pass the message through
            return CorrelationResult.Success(storedCorrelationUid, [message]);
        }

        cancellationToken.ThrowIfCancellationRequested();

        if (await correlationService.GetCorrelationAsync(dataProducer, message, cancellationToken) is CorrelationUid correlationUid)
        {
            if (this.activeCorrelations.TryGetValue(correlationUid, out var existingSession))
            {
                return CorrelationResult.Failure(
                    $"Two active sessions '{existingSession.Value}' and '{sessionUid.Value}' "
                        + $"share the same correlation UID '{correlationUid.Value}'. All active sessions "
                        + "must have unique correlation UIDs. This is most likely an issue with the currently "
                        + "running Allure integration.");
            }

            // New correlation to remember.
            this.correlatedSessions[sessionUid] = correlationUid;
            this.activeCorrelations[correlationUid] = sessionUid;

            // dequeue both buffers and restore the order of buffered messages
            return CorrelationResult.Success(
                correlationUid,
                [..this.Dequeue(sessionUid, correlationUid), message]
            );
        }

        // Can't correlate yet. Buffer the message and yield nothing to process
        Enqueue(message);

        return CorrelationResult.NotReady;
    }

    CorrelationResult Correlate(DataWithCorrelationUid message)
    {
        var correlationUid = message.CorrelationUid;

        if (this.activeCorrelations.ContainsKey(correlationUid) || !this.sessionUidBuffers.Any())
        {
            // Correlation either exists or not needed so far.
            return CorrelationResult.Success(correlationUid, [message]);
        }

        // No correlation exists and there are uncorrelated sessions.
        // Buffer the message and wait until the correlation is established
        // to prevent out of order delivery.
        Enqueue(message);
        return CorrelationResult.NotReady;
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
