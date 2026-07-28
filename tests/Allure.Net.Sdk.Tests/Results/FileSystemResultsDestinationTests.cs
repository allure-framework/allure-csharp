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

    static AllureTestResult NewTestResult() =>
        new()
        {
            Uuid = Guid.NewGuid().ToString("N"),
            Name = "result name",
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

    static TestResultScope NewContainer() =>
        new()
        {
            Uuid = Guid.NewGuid().ToString("N"),
            Name = "container name",
            Children = { "child-id" },
        };

    static Globals NewGlobals() =>
        new()
        {
            Errors =
            {
                new()
                {
                    Message = "global error",
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
