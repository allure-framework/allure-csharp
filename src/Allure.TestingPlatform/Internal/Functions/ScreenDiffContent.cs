using System;
using System.Buffers;
using System.Buffers.Text;
using System.IO;
using System.IO.Pipelines;
using System.Threading;
using System.Threading.Tasks;

namespace Allure.TestingPlatform.Internal.Functions;

sealed class ScreenDiffContent
{
    const int CHUNK_SIZE = 3 * 16 * 1024;

    public static async Task<TResult> ConsumeAsync<TResult>(
        Stream expected,
        Stream actual,
        Stream diff,
        Func<Stream, CancellationToken, Task<TResult>> consumeAsync,
        CancellationToken cancellationToken
    )
    {
        var pipe = new Pipe(
            new PipeOptions(
                pauseWriterThreshold: 128 * 1024,
                resumeWriterThreshold: 64 * 1024,
                useSynchronizationContext: false
            )
        );

        using var stopProducer =
            CancellationTokenSource.CreateLinkedTokenSource(
                cancellationToken
            );

        using var output = pipe.Reader.AsStream(leaveOpen: true);

        var producer = ProduceAsync(
            expected,
            actual,
            diff,
            pipe.Writer,
            stopProducer.Token
        );

        Exception? consumerError = null;

        try
        {
            return await consumeAsync(output, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            consumerError = exception;
            throw;
        }
        finally
        {
            stopProducer.Cancel();

            await pipe.Reader.CompleteAsync(consumerError)
                .ConfigureAwait(false);

            try
            {
                await producer.ConfigureAwait(false);
            }
            catch when (consumerError is not null)
            {
                // Preserve the consumer's original exception.
            }
        }
    }

    static async Task ProduceAsync(
        Stream expected,
        Stream actual,
        Stream diff,
        PipeWriter writer,
        CancellationToken cancellationToken
    )
    {
        byte[] buffer = ArrayPool<byte>.Shared.Rent(CHUNK_SIZE + 2);

        Exception? error = null;

        try
        {
            writer.Write("{\"expected\":\"data:image/png;base64,"u8);
            await WriteStream(writer, expected, buffer, cancellationToken)
                .ConfigureAwait(false);
            writer.Write("\",\"actual\":\"data:image/png;base64,"u8);
            await WriteStream(writer, actual, buffer, cancellationToken)
                .ConfigureAwait(false);
            writer.Write("\",\"diff\":\"data:image/png;base64,"u8);
            await WriteStream(writer, diff, buffer, cancellationToken)
                .ConfigureAwait(false);
            writer.Write("\"}"u8);
            await FlushAsync(writer, cancellationToken)
                .ConfigureAwait(false);
        }
        catch (Exception exception)
        {
            error = exception;
            throw;
        }
        finally
        {
            ArrayPool<byte>.Shared.Return(buffer);

            await writer.CompleteAsync(error).ConfigureAwait(false);
        }
    }

    static async Task WriteStream(
        PipeWriter writer,
        Stream input,
        byte[] buffer,
        CancellationToken cancellationToken
    )
    {
        int buffered = 0;

        while (true)
        {
            int read = await input.ReadAsync(
                buffer,
                buffered,
                CHUNK_SIZE,
                cancellationToken
            ).ConfigureAwait(false);

            if (read == 0)
            {
                if (buffered != 0)
                {
                    Encode(
                        writer,
                        buffer.AsSpan(0, buffered),
                        isFinalBlock: true
                    );

                    if (!await FlushAsync(writer, cancellationToken).ConfigureAwait(false))
                    {
                        return;
                    }
                }

                break;
            }

            buffered += read;

            int encodable = buffered - buffered % 3;
            if (encodable == 0)
            {
                continue;
            }

            Encode(
                writer,
                buffer.AsSpan(0, encodable),
                isFinalBlock: false
            );

            buffered -= encodable;

            if (buffered != 0)
            {
                buffer.AsSpan(encodable, buffered)
                    .CopyTo(buffer);
            }

            if (!await FlushAsync(writer, cancellationToken).ConfigureAwait(false))
            {
                return;
            }
        }
    }

    static void Encode(
        PipeWriter writer,
        ReadOnlySpan<byte> source,
        bool isFinalBlock
    )
    {
        int required = Base64.GetMaxEncodedToUtf8Length(source.Length);

        Span<byte> destination = writer.GetSpan(required);

        OperationStatus status = Base64.EncodeToUtf8(
            source,
            destination,
            out int consumed,
            out int written,
            isFinalBlock
        );

        if (status != OperationStatus.Done || consumed != source.Length)
        {
            throw new InvalidOperationException(
                $"Unexpected Base64 status: {status}"
            );
        }

        writer.Advance(written);
    }

    static async ValueTask<bool> FlushAsync(
        PipeWriter writer,
        CancellationToken cancellationToken
    )
    {
        FlushResult result = await writer
            .FlushAsync(cancellationToken)
            .ConfigureAwait(false);

        return !result.IsCompleted && !result.IsCanceled;
    }
}
