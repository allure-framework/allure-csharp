using System.Text.Json;
using System.Text.RegularExpressions;
using Allure.Model;
using Allure.Sdk.Results;
using AllureTestResult = Allure.Model.TestResult;

namespace Allure.Net.Sdk.Tests.Results;

public class FileSystemResultsDestinationTests
{
    [Test]
    public async Task ShouldWriteTestResult()
    {
        var result = NewTestResult();

        await VerifyObjectWrite(
            "-result.json",
            result.Uuid,
            (destination) => destination.WriteTestResult(result)
        );
    }

    [Test]
    public async Task ShouldWriteTestResultAsync()
    {
        var result = NewTestResult();

        await VerifyObjectWriteAsync(
            "-result.json",
            result.Uuid,
            (destination, token) => destination.WriteTestResultAsync(result, token)
        );
    }

    [Test]
    public async Task ShouldWriteContainer()
    {
        var container = NewContainer();
        container.Afters.Add(new(){ Name = "Foo" });

        await VerifyObjectWrite(
            "-container.json",
            container.Uuid,
            (destination) => destination.WriteContainer(container)
        );
    }

    [Test]
    public async Task ShouldWriteContainerAsync()
    {
        var container = NewContainer();
        container.Afters.Add(new(){ Name = "Foo" });

        await VerifyObjectWriteAsync(
            "-container.json",
            container.Uuid,
            (destination, token) => destination.WriteContainerAsync(container, token)
        );
    }

    [Test]
    public async Task ShouldWriteGlobals()
    {
        await VerifyObjectWrite(
            "-globals.json",
            "global error",
            (destination) => destination.WriteGlobals(NewGlobals())
        );
    }

    [Test]
    public async Task ShouldWriteGlobalsAsync()
    {
        await VerifyObjectWriteAsync(
            "-globals.json",
            "global error",
            (destination, token) => destination.WriteGlobalsAsync(NewGlobals(), token)
        );
    }

    [Test]
    public async Task ShouldUseAllureJsonSerializationConventions()
    {
        var directory = NewDirectoryPath();
        try
        {
            var result = NewTestResult();
            var destination = new FileSystemResultsDestination(directory, false);

            destination.WriteTestResult(result);

            var json = await File.ReadAllTextAsync(Directory.GetFiles(directory).Single());
            using var document = JsonDocument.Parse(json);
            var root = document.RootElement;

            await Assert.That(root.GetProperty("uuid").GetString()).IsEqualTo(result.Uuid);
            await Assert.That(root.GetProperty("status").GetString()).IsEqualTo("passed");
            await Assert.That(root.GetProperty("stage").GetString()).IsEqualTo("finished");
            await Assert.That(
                root.GetProperty("parameters")[0].GetProperty("mode").GetString()
            ).IsEqualTo("masked");
            await Assert.That(root.TryGetProperty("description", out _)).IsFalse();
            await Assert.That(root.TryGetProperty("Description", out _)).IsFalse();
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldControlJsonIndentation()
    {
        var compactDirectory = NewDirectoryPath();
        var indentedDirectory = NewDirectoryPath();
        try
        {
            var result = NewTestResult();
            new FileSystemResultsDestination(compactDirectory, false).WriteTestResult(result);
            new FileSystemResultsDestination(indentedDirectory, true).WriteTestResult(result);

            var compact = await File.ReadAllTextAsync(
                Directory.GetFiles(compactDirectory).Single()
            );
            var indented = await File.ReadAllTextAsync(
                Directory.GetFiles(indentedDirectory).Single()
            );

            await Assert.That(compact.Contains('\n')).IsFalse();
            await Assert.That(indented.Contains('\n')).IsTrue();
            await Assert.That(indented).Contains("  \"uuid\"");
            await Assert.That(JsonElement.DeepEquals(
                JsonDocument.Parse(compact).RootElement,
                JsonDocument.Parse(indented).RootElement
            )).IsTrue();
        }
        finally
        {
            DeleteDirectory(compactDirectory);
            DeleteDirectory(indentedDirectory);
        }
    }

    [Test]
    public async Task ShouldCreateUniqueFilesForSeparateWrites()
    {
        var directory = NewDirectoryPath();
        try
        {
            var destination = new FileSystemResultsDestination(directory, false);

            destination.WriteTestResult(NewTestResult());
            destination.WriteTestResult(NewTestResult());

            var files = Directory.GetFiles(directory, "*-result.json");
            await Assert.That(files.Count()).IsEqualTo(2);
            await Assert.That(files.Distinct().Count()).IsEqualTo(2);
            foreach (var file in files)
            {
                using var document = JsonDocument.Parse(await File.ReadAllTextAsync(file));
                await Assert.That(document.RootElement.ValueKind)
                    .IsEqualTo(JsonValueKind.Object);
            }
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    [Test]
    public async Task ShouldCancelAsyncObjectWriteWithoutCompletedJson()
    {
        var directory = NewDirectoryPath();
        try
        {
            var destination = new FileSystemResultsDestination(directory, false);
            using var cancellation = new CancellationTokenSource();
            cancellation.Cancel();

            await Assert.That(async () =>
                await destination.WriteTestResultAsync(
                    NewTestResult(),
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
    public async Task ShouldThrowWhenOutputDirectoryIsAnExistingFile()
    {
        var path = NewDirectoryPath();
        try
        {
            await File.WriteAllTextAsync(path, "not a directory");
            var destination = new FileSystemResultsDestination(path, false);

            await Assert.That(() => destination.WriteTestResult(NewTestResult()))
                .Throws<IOException>();

            await Assert.That(await File.ReadAllTextAsync(path))
                .IsEqualTo("not a directory");
        }
        finally
        {
            File.Delete(path);
        }
    }

    [Test]
    public async Task ShouldWriteObjectsConcurrentlyFromSynchronousCalls()
    {
        await VerifyConcurrentObjectWrites((destination, index) =>
            Task.Run(() =>
            {
                var container = NewContainer($"container-{index:D2}");
                container.Afters.Add(new() { Name = "Foo" });
                destination.WriteTestResult(NewTestResult($"result-{index:D2}"));
                destination.WriteContainer(container);
                destination.WriteGlobals(NewGlobals($"global-{index:D2}"));
            })
        );
    }

    [Test]
    public async Task ShouldWriteObjectsConcurrentlyFromAsynchronousCalls()
    {
        await VerifyConcurrentObjectWrites(async (destination, index) =>
        {
            var container = NewContainer($"container-{index:D2}");
            container.Afters.Add(new() { Name = "Foo" });
            await destination.WriteTestResultAsync(
                NewTestResult($"result-{index:D2}"),
                CancellationToken.None
            );
            await destination.WriteContainerAsync(container, CancellationToken.None);
            await destination.WriteGlobalsAsync(
                NewGlobals($"global-{index:D2}"),
                CancellationToken.None
            );
        });
    }

    [Test]
    public async Task ShouldWriteDistinctAttachmentsConcurrentlyFromSynchronousCalls()
    {
        await VerifyConcurrentAttachmentWrites((destination, fileName, content) =>
            Task.Run(() =>
            {
                using var stream = new MemoryStream(content);
                destination.WriteAttachment(fileName, stream);
            })
        );
    }

    [Test]
    public async Task ShouldWriteDistinctAttachmentsConcurrentlyFromAsynchronousCalls()
    {
        await VerifyConcurrentAttachmentWrites(async (destination, fileName, content) =>
        {
            using var stream = new MemoryStream(content);
            await destination.WriteAttachmentAsync(
                fileName,
                stream,
                CancellationToken.None
            );
        });
    }

    static async Task VerifyConcurrentObjectWrites(
        Func<FileSystemResultsDestination, int, Task> write
    )
    {
        const int writeCount = 20;
        var directory = NewDirectoryPath();
        try
        {
            var destination = new FileSystemResultsDestination(directory, false);

            await Task.WhenAll(
                Enumerable.Range(0, writeCount).Select(index => write(destination, index))
            );

            var files = Directory.GetFiles(directory);
            await Assert.That(files.Length).IsEqualTo(writeCount * 3);
            await Assert.That(files.Select(Path.GetFileName).Distinct().Count())
                .IsEqualTo(writeCount * 3);
            await Assert.That(files.Count(path => path.EndsWith("-result.json")))
                .IsEqualTo(writeCount);
            await Assert.That(files.Count(path => path.EndsWith("-container.json")))
                .IsEqualTo(writeCount);
            await Assert.That(files.Count(path => path.EndsWith("-globals.json")))
                .IsEqualTo(writeCount);

            var contents = await Task.WhenAll(
                files.Select(path => File.ReadAllTextAsync(path))
            );
            await Assert.That(contents.All(IsValidJson)).IsTrue();
            foreach (var index in Enumerable.Range(0, writeCount))
            {
                await Assert.That(contents.Count(content => content.Contains($"result-{index:D2}")))
                    .IsEqualTo(1);
                await Assert.That(contents.Count(content => content.Contains($"container-{index:D2}")))
                    .IsEqualTo(1);
                await Assert.That(contents.Count(content => content.Contains($"global-{index:D2}")))
                    .IsEqualTo(1);
            }
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    static async Task VerifyConcurrentAttachmentWrites(
        Func<FileSystemResultsDestination, string, byte[], Task> write
    )
    {
        const int writeCount = 20;
        var directory = NewDirectoryPath();
        try
        {
            var destination = new FileSystemResultsDestination(directory, false);
            var attachments = Enumerable.Range(0, writeCount)
                .Select(index => (
                    FileName: $"attachment-{index}.bin",
                    Content: Enumerable.Range(0, index + 1)
                        .Select(value => (byte)(value + index))
                        .ToArray()
                ))
                .ToArray();

            await Task.WhenAll(
                attachments.Select(attachment =>
                    write(destination, attachment.FileName, attachment.Content)
                )
            );

            await Assert.That(Directory.GetFiles(directory).Length)
                .IsEqualTo(writeCount);
            foreach (var attachment in attachments)
            {
                await Assert.That(
                    await File.ReadAllBytesAsync(
                        Path.Combine(directory, attachment.FileName)
                    )
                ).IsEquivalentTo(attachment.Content);
            }
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    static async Task VerifyObjectWrite(
        string suffix,
        string expectedContent,
        Action<FileSystemResultsDestination> write
    )
    {
        var directory = NewDirectoryPath();
        try
        {
            await Assert.That(Directory.Exists(directory)).IsFalse();

            write(new FileSystemResultsDestination(directory, false));

            await VerifyWrittenFile(directory, suffix, expectedContent);
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    static async Task VerifyObjectWriteAsync(
        string suffix,
        string expectedContent,
        Func<FileSystemResultsDestination, CancellationToken, Task> write
    )
    {
        var directory = NewDirectoryPath();
        try
        {
            await Assert.That(Directory.Exists(directory)).IsFalse();

            await write(
                new FileSystemResultsDestination(directory, false),
                CancellationToken.None
            );

            await VerifyWrittenFile(directory, suffix, expectedContent);
        }
        finally
        {
            DeleteDirectory(directory);
        }
    }

    static async Task VerifyWrittenFile(
        string directory,
        string suffix,
        string expectedContent
    )
    {
        await Assert.That(Directory.Exists(directory)).IsTrue();
        var file = await Assert.That(Directory.GetFiles(directory)).HasSingleItem();
        await Assert.That(Regex.IsMatch(
            Path.GetFileName(file),
            $"^[0-9a-f]{{32}}{Regex.Escape(suffix)}$"
        )).IsTrue();

        var json = await File.ReadAllTextAsync(file);
        using var document = JsonDocument.Parse(json);
        await Assert.That(json).Contains(expectedContent);
        await Assert.That(document.RootElement.ValueKind).IsEqualTo(JsonValueKind.Object);
    }

    static AllureTestResult NewTestResult(string name = "result name") =>
        new()
        {
            Uuid = Guid.NewGuid().ToString("N"),
            Name = name,
            Status = Status.Passed,
            Stage = Stage.Finished,
            Parameters =
            {
                new()
                {
                    Name = "secret",
                    Value = "value",
                    Mode = ParameterMode.Masked,
                },
            },
        };

    static TestResultScope NewContainer(string name = "container name") =>
        new()
        {
            Uuid = Guid.NewGuid().ToString("N"),
            Name = name,
            Children = { "child-id" },
        };

    static Globals NewGlobals(string message = "global error") =>
        new()
        {
            Errors =
            {
                new()
                {
                    Message = message,
                    Trace = "global trace",
                },
            },
        };

    static string NewDirectoryPath() =>
        Path.Combine(Path.GetTempPath(), $"allure-sdk-destination-{Guid.NewGuid():N}");

    static void DeleteDirectory(string path)
    {
        if (Directory.Exists(path))
        {
            Directory.Delete(path, true);
        }
    }

    static bool IsValidJson(string content)
    {
        try
        {
            using var _ = JsonDocument.Parse(content);
            return true;
        }
        catch (JsonException)
        {
            return false;
        }
    }
}
