using System.Collections.Immutable;
using System.Diagnostics;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using Allure.Testing;

#nullable enable

namespace Allure.NUnit.Tests;

internal record class SampleRunnerInput(
    List<string> Arguments,
    object? Configuration,
    Dictionary<string, string> Environment
)
{
    public static SampleRunnerInput Default { get; } = new([], null, []);
}

internal class SampleRunner
{
    static readonly Encoding encoding = new UTF8Encoding(false, false);
    static readonly JsonSerializerOptions jsonSerializerOptions = new() { WriteIndented = true };

    public static async Task<RunResult> RunAsync(AllureSampleRegistry sample) =>
        await RunAsync(sample, SampleRunnerInput.Default, CancellationToken.None);

    public static async Task<RunResult> RunAsync(AllureSampleRegistry sample, CancellationToken token) =>
        await RunAsync(sample, SampleRunnerInput.Default, token);

    public static async Task<RunResult> RunAsync(AllureSampleRegistry sample, SampleRunnerInput input) =>
        await RunAsync(sample, input, CancellationToken.None);

    public static async Task<RunResult> RunAsync(AllureSampleRegistry sample, SampleRunnerInput input, CancellationToken token)
    {

        var projectDir = Path.Combine(
            AllureBuildProperties.Allure_SampleSolutionDir,
            $"{AllureBuildProperties.Allure_SampleSolutionName}.{sample.Name}"
        );

        var psi = new ProcessStartInfo(
            "dotnet",
            [
                "test",
                projectDir,
                "--framework",
                AllureBuildProperties.Allure_SampleSelectedTargetFramework,
                "--configuration",
                AllureBuildProperties.Allure_SampleConfiguration,
                ..input.Arguments
            ]
        )
        {
            CreateNoWindow = true,
            RedirectStandardError = true,
            RedirectStandardOutput = true,
            StandardErrorEncoding = encoding,
            StandardOutputEncoding = encoding,
            UseShellExecute = false,
        };

        foreach (var (name, value) in input.Environment)
        {
            psi.Environment.Add(new (name, value));
        }

        string? configPath = null;
        DirectoryInfo? resultsDir = null;

        try
        {
            if (input.Configuration is not null)
            {
                configPath = Path.GetTempFileName();
                using var fs = new FileStream(configPath, FileMode.Create, FileAccess.Write);
                await JsonSerializer.SerializeAsync(fs, input.Configuration, jsonSerializerOptions, token);
                psi.Environment["ALLURE_CONFIG"] = configPath;
            }

            resultsDir = Directory.CreateTempSubdirectory("allure-results-");
            psi.Environment["ALLURE_RESULTSDIR"] = resultsDir.FullName;

            using var process = new Process() { StartInfo = psi, EnableRaisingEvents = true };

            process.Start();

            var stdoutTask = Task.Factory.StartNew(() => process.StandardOutput.ReadToEnd(), TaskCreationOptions.LongRunning);
            var stderrTask = Task.Factory.StartNew(() => process.StandardError.ReadToEnd(), TaskCreationOptions.LongRunning);

            await process.WaitForExitAsync(token);

            var resultFiles = resultsDir.GetFiles();

            var testResultFiles = resultFiles.Where(f => f.Name.EndsWith("-result.json")).ToArray();
            var testResults = ImmutableArray.CreateBuilder<JsonObject>(testResultFiles.Length);
            foreach (var resultFile in resultFiles)
            {
                using var resultStream = resultFile.OpenRead();
                if (await JsonNode.ParseAsync(resultStream, cancellationToken: token) is JsonObject testResult)
                {
                    testResults.Add(testResult);
                }
            }

            var containerFiles = resultFiles.Where(f => f.Name.EndsWith("-container.json")).ToArray();
            var containers = ImmutableArray.CreateBuilder<JsonObject>(containerFiles.Length);
            foreach (var containerFile in containerFiles)
            {
                using var resultStream = containerFile.OpenRead();
                if (await JsonNode.ParseAsync(resultStream, cancellationToken: token) is JsonObject container)
                {
                    containers.Add(container);
                }
            }

            var attachmentFiles = resultFiles
                .Where(f =>
                    Path.GetFileNameWithoutExtension(f.Name).EndsWith("-attachment"))
                .ToArray();
            var attachments = ImmutableDictionary.CreateBuilder<string, ReadOnlyMemory<byte>>();
            foreach (var attachmentFile in attachmentFiles)
            {
                var bytes = await File.ReadAllBytesAsync(attachmentFile.FullName, token);
                attachments.Add(attachmentFile.Name, new ReadOnlyMemory<byte>(bytes));
            }

            return new(
                process.ExitCode,
                await stdoutTask,
                await stderrTask,
                new(testResults.MoveToImmutable(), containers.MoveToImmutable(), attachments.ToImmutable())
            );
        }
        finally
        {
            if (configPath is not null)
            {
                File.Delete(configPath);
            }

            resultsDir?.Delete(true);
        }
    }
}

public record class AllureResults(
    ImmutableArray<JsonObject> TestResults,
    ImmutableArray<JsonObject> Containers,
    ImmutableDictionary<string, ReadOnlyMemory<byte>> Attachments
);

public record class RunResult(
    int ExitCode,
    string StdOut,
    string StdErr,
    AllureResults AllureResults
);
