using System.Text;
using Allure.Sdk.Results;

namespace Allure.Net.Sdk.Tests.Results;

public class FileSystemResultsDestinationAttachmentTests
{
    static readonly byte[] AttachmentContent = [0, 1, 2, 127, 128, 254, 255];

    [Test]
    public async Task ShouldWriteAttachment()
    {
        await VerifyStreamWrite(
            (destination, stream) =>
            {
                destination.WriteAttachment("attachment.bin", stream);
                return Task.CompletedTask;
            }
        );
    }

    [Test]
    public async Task ShouldWriteAttachmentAsync()
    {
        await VerifyStreamWrite(
            (destination, stream) =>
                destination.WriteAttachmentAsync(
                    "attachment.bin",
                    stream,
                    CancellationToken.None
                )
        );
    }

    [Test]
    public async Task ShouldWriteEmptyAttachment()
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new MemoryStream();

            new FileSystemResultsDestination(directory, false)
                .WriteAttachment("empty.bin", content);

            await Assert.That(new FileInfo(Path.Combine(directory, "empty.bin")).Length)
                .IsEqualTo(0);
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldWriteAttachmentFromCurrentStreamPosition()
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new MemoryStream(AttachmentContent);
            content.Position = 3;

            new FileSystemResultsDestination(directory, false)
                .WriteAttachment("attachment.bin", content);

            await Assert.That(
                await File.ReadAllBytesAsync(Path.Combine(directory, "attachment.bin"))
            ).IsEquivalentTo(AttachmentContent[3..]);
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldRejectExistingAttachmentDestination()
    {
        var directory = NewDirectoryPath();
        try
        {
            Directory.CreateDirectory(directory);
            var outputPath = Path.Combine(directory, "attachment.txt");
            await File.WriteAllTextAsync(outputPath, "long existing content");
            using var syncContent = new MemoryStream(Encoding.UTF8.GetBytes("sync"));
            using var asyncContent = new MemoryStream(Encoding.UTF8.GetBytes("async"));
            var destination = new FileSystemResultsDestination(directory, false);

            await Assert.That(() =>
                destination.WriteAttachment("attachment.txt", syncContent)
            ).Throws<IOException>();
            await Assert.That(async () =>
                await destination.WriteAttachmentAsync(
                    "attachment.txt",
                    asyncContent,
                    CancellationToken.None
                )
            ).Throws<IOException>();

            await Assert.That(await File.ReadAllTextAsync(outputPath))
                .IsEqualTo("long existing content");
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldCancelAttachmentWriteBeforeCreatingOutput()
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new MemoryStream(AttachmentContent);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            var destination = new FileSystemResultsDestination(directory, false);

            await Assert.That(async () =>
                await destination.WriteAttachmentAsync(
                    "attachment.bin",
                    content,
                    cancellation.Token
                )
            ).Throws<OperationCanceledException>();

            await Assert.That(Directory.Exists(directory)).IsFalse();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldPublishAttachmentOnlyAfterAtomicWriteCompletes()
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new PausingReadStream(AttachmentContent);
            var outputPath = Path.Combine(directory, "attachment.bin");
            var write = new FileSystemResultsDestination(directory, false)
                .WriteAttachmentAsync(
                    "attachment.bin",
                    content,
                    CancellationToken.None
                );

            await content.WaitUntilPaused();

            await Assert.That(File.Exists(outputPath)).IsFalse();
            await Assert.That(GetTemporaryFiles(directory).Count()).IsEqualTo(1);

            content.Resume();
            await write;

            await Assert.That(await File.ReadAllBytesAsync(outputPath))
                .IsEquivalentTo(
                    AttachmentContent,
                    TUnit.Assertions.Enums.CollectionOrdering.Matching
                );
            await Assert.That(GetTemporaryFiles(directory)).IsEmpty();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldRemoveTemporaryFileWhenAtomicWriteFails()
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new PausingReadStream(
                AttachmentContent,
                throwAfterPause: true
            );
            var outputPath = Path.Combine(directory, "attachment.bin");
            var write = new FileSystemResultsDestination(directory, false)
                .WriteAttachmentAsync(
                    "attachment.bin",
                    content,
                    CancellationToken.None
                );

            await content.WaitUntilPaused();

            await Assert.That(File.Exists(outputPath)).IsFalse();
            await Assert.That(GetTemporaryFiles(directory).Count()).IsEqualTo(1);

            content.Resume();
            await Assert.That(async () => await write).Throws<IOException>();

            await Assert.That(File.Exists(outputPath)).IsFalse();
            await Assert.That(GetTemporaryFiles(directory)).IsEmpty();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldRemoveTemporaryFileWhenAtomicWriteIsCancelled()
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new PausingReadStream(AttachmentContent);
            using var cancellation = new CancellationTokenSource();
            var outputPath = Path.Combine(directory, "attachment.bin");
            var write = new FileSystemResultsDestination(directory, false)
                .WriteAttachmentAsync(
                    "attachment.bin",
                    content,
                    cancellation.Token
                );

            await content.WaitUntilPaused();

            await Assert.That(File.Exists(outputPath)).IsFalse();
            await Assert.That(GetTemporaryFiles(directory).Count()).IsEqualTo(1);

            cancellation.Cancel();
            await Assert.That(async () => await write)
                .Throws<OperationCanceledException>();

            await Assert.That(File.Exists(outputPath)).IsFalse();
            await Assert.That(GetTemporaryFiles(directory)).IsEmpty();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldCopyAttachment()
    {
        await VerifyFileCopy(
            (destination, source) =>
            {
                destination.CopyAttachment("copied.bin", source);
                return Task.CompletedTask;
            }
        );
    }

    [Test]
    public async Task ShouldCopyAttachmentAsync()
    {
        await VerifyFileCopy(
            (destination, source) =>
                destination.CopyAttachmentAsync(
                    "copied.bin",
                    source,
                    CancellationToken.None
                )
        );
    }

    [Test]
    public async Task ShouldRejectExistingCopyDestination()
    {
        var directory = NewDirectoryPath();
        var source = NewSourceFile();
        try
        {
            Directory.CreateDirectory(directory);
            await File.WriteAllTextAsync(Path.Combine(directory, "copied.bin"), "existing");
            var destination = new FileSystemResultsDestination(directory, false);

            await Assert.That(() =>
                destination.CopyAttachment("copied.bin", source)
            ).Throws<IOException>();
            await Assert.That(async () =>
                await destination.CopyAttachmentAsync(
                    "copied.bin",
                    source,
                    CancellationToken.None
                )
            ).Throws<IOException>();
            await Assert.That(
                await File.ReadAllTextAsync(Path.Combine(directory, "copied.bin"))
            ).IsEqualTo("existing");
        }
        finally
        {
            DeleteDirectory(directory);
            File.Delete(source);
        }
    }

    [Test]
    public async Task ShouldThrowWhenCopySourceDoesNotExist()
    {
        var directory = NewDirectoryPath();
        var missingSource = Path.Combine(
            Path.GetTempPath(),
            $"allure-sdk-missing-{Guid.NewGuid():N}"
        );
        var destination = new FileSystemResultsDestination(directory, false);

        await Assert.That(() =>
            destination.CopyAttachment("copied.bin", missingSource)
        ).Throws<FileNotFoundException>();
        await Assert.That(async () =>
            await destination.CopyAttachmentAsync(
                "copied.bin",
                missingSource,
                CancellationToken.None
            )
        ).Throws<FileNotFoundException>();
    }

    [Test]
    public async Task ShouldCancelCopyBeforeOpeningFiles()
    {
        var directory = NewDirectoryPath();
        var missingSource = Path.Combine(
            Path.GetTempPath(),
            $"allure-sdk-missing-{Guid.NewGuid():N}"
        );
        try
        {
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();
            var destination = new FileSystemResultsDestination(directory, false);

            await Assert.That(async () =>
                await destination.CopyAttachmentAsync(
                    "copied.bin",
                    missingSource,
                    cancellation.Token
                )
            ).Throws<OperationCanceledException>();

            await Assert.That(Directory.Exists(directory)).IsFalse();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    static async Task VerifyStreamWrite(
        Func<FileSystemResultsDestination, Stream, Task> write
    )
    {
        var directory = NewDirectoryPath();
        try
        {
            using var content = new MemoryStream(AttachmentContent);

            await write(new FileSystemResultsDestination(directory, false), content);

            await Assert.That(Directory.Exists(directory)).IsTrue();
            await Assert.That(
                await File.ReadAllBytesAsync(Path.Combine(directory, "attachment.bin"))
            ).IsEquivalentTo(AttachmentContent, TUnit.Assertions.Enums.CollectionOrdering.Matching);
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    static async Task VerifyFileCopy(
        Func<FileSystemResultsDestination, string, Task> copy
    )
    {
        var directory = NewDirectoryPath();
        var source = NewSourceFile();
        try
        {
            var originalSource = await File.ReadAllBytesAsync(source);

            await copy(new FileSystemResultsDestination(directory, false), source);

            await Assert.That(Directory.Exists(directory)).IsTrue();
            await Assert.That(
                await File.ReadAllBytesAsync(Path.Combine(directory, "copied.bin"))
            ).IsEquivalentTo(originalSource, TUnit.Assertions.Enums.CollectionOrdering.Matching);
        }
        finally
        {
            DeleteDirectory(directory);
            File.Delete(source);
        }
    }

    static string NewSourceFile()
    {
        var path = Path.Combine(
            Path.GetTempPath(),
            $"allure-sdk-source-{Guid.NewGuid():N}.bin"
        );
        File.WriteAllBytes(path, AttachmentContent);
        return path;
    }

    static string NewDirectoryPath() =>
        Path.Combine(Path.GetTempPath(), $"allure-sdk-destination-{Guid.NewGuid():N}");

    static IEnumerable<string> GetTemporaryFiles(string directory) =>
        Directory.Exists(directory)
            ? Directory.EnumerateFiles(directory, ".allure-write-*.tmp")
            : [];

    static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    sealed class PausingReadStream(
        byte[] content,
        bool throwAfterPause = false
    ) : Stream
    {
        readonly TaskCompletionSource paused = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        readonly TaskCompletionSource resume = new(
            TaskCreationOptions.RunContinuationsAsynchronously
        );
        int readCount;

        public Task WaitUntilPaused() => this.paused.Task;

        public void Resume() => this.resume.TrySetResult();

        public override async ValueTask<int> ReadAsync(
            Memory<byte> buffer,
            CancellationToken cancellationToken = default
        )
        {
            if (this.readCount++ == 0)
            {
                var count = Math.Min(3, content.Length);
                content.AsMemory(0, count).CopyTo(buffer);
                return count;
            }

            if (this.readCount == 2)
            {
                this.paused.TrySetResult();
                await this.resume.Task.WaitAsync(cancellationToken);

                if (throwAfterPause)
                {
                    throw new IOException("The source stream failed.");
                }

                var remainder = content.AsMemory(Math.Min(3, content.Length));
                remainder.CopyTo(buffer);
                return remainder.Length;
            }

            return 0;
        }

        public override async Task<int> ReadAsync(
            byte[] buffer,
            int offset,
            int count,
            CancellationToken cancellationToken
        ) =>
            await this.ReadAsync(
                buffer.AsMemory(offset, count),
                cancellationToken
            );

        public override bool CanRead => true;
        public override bool CanSeek => false;
        public override bool CanWrite => false;
        public override long Length => throw new NotSupportedException();
        public override long Position
        {
            get => throw new NotSupportedException();
            set => throw new NotSupportedException();
        }

        public override void Flush()
        {
        }

        public override int Read(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();

        public override long Seek(long offset, SeekOrigin origin) =>
            throw new NotSupportedException();

        public override void SetLength(long value) =>
            throw new NotSupportedException();

        public override void Write(byte[] buffer, int offset, int count) =>
            throw new NotSupportedException();
    }
}
