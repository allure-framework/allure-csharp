using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Threading;
using System.Threading.Tasks;

namespace Allure.Testing;

/// <summary>
/// Allows running samples defined in the test project and accessing test results for assertions.
/// </summary>
public class AllureSampleRunner
{
    /// <summary>
    /// Runs a test sample project and reads the results.
    /// </summary>
    /// <param name="sample">
    /// An entry from the sample registry of the test project.
    /// The registry is auto-generated for each test project that has at least one sample
    /// source defined.
    /// </param>
    /// <returns>The results of the run, including Allure results.</returns>
    /// <exception cref="InvalidOperationException" />
    public static async Task<AllureSampleRunOutput> RunAsync(AllureSampleRegistryEntry sample) =>
        await RunAsync(sample, AllureSampleRunInput.Default, CancellationToken.None);

    /// <summary>
    /// Runs a test sample project and reads the results.
    /// </summary>
    /// <param name="sample">
    /// An entry from the sample registry of the test project.
    /// The registry is auto-generated for each test project that has at least one sample
    /// source defined.
    /// </param>
    /// <param name="ct">A cancellation token to interrupt the sample run.</param>
    /// <returns>The results of the run, including Allure results.</returns>
    /// <exception cref="InvalidOperationException" />
    public static async Task<AllureSampleRunOutput> RunAsync(
        AllureSampleRegistryEntry sample,
        CancellationToken token
    ) =>
        await RunAsync(sample, AllureSampleRunInput.Default, token);

    /// <summary>
    /// Runs a test sample project and reads the results.
    /// </summary>
    /// <param name="sample">
    /// An entry from the sample registry of the test project.
    /// The registry is auto-generated for each test project that has at least one sample
    /// source defined.
    /// </param>
    /// <param name="input">Input data for the sample run.</param>
    /// <returns>The results of the run, including Allure results.</returns>
    /// <exception cref="InvalidOperationException" />
    public static async Task<AllureSampleRunOutput> RunAsync(
        AllureSampleRegistryEntry sample,
        AllureSampleRunInput input
    ) =>
        await RunAsync(sample, input, CancellationToken.None);

    /// <summary>
    /// Runs a test sample project and reads the results.
    /// </summary>
    /// <param name="sample">
    /// An entry from the sample registry of the test project.
    /// The registry is auto-generated for each test project that has at least one sample
    /// source defined.
    /// </param>
    /// <param name="input">Input data for the sample run.</param>
    /// <param name="ct">A cancellation token to interrupt the sample run.</param>
    /// <returns>The results of the run, including Allure results.</returns>
    /// <exception cref="InvalidOperationException" />
    public static async Task<AllureSampleRunOutput> RunAsync(
        AllureSampleRegistryEntry sample,
        AllureSampleRunInput input,
        CancellationToken ct
    )
    {
        var psi = CreateProcessStartInfo(sample, input.ProcessArguments);

        ApplyExtraEnvironmentVariables(input.EnvironmentVariables, psi);

        using var _ = await MaybeApplyAllureConfig(input.AllureConfiguration, psi, ct);
        using var resultsDirGuard = ApplyAllureResultsDirectory(psi, input.AllureResultsDirectory);

        using var process = Process.Start(psi) ??
            throw new InvalidOperationException("Unable to start a process");

        var stdStreamsTask = SetProcessStreamCollection(process, ct);

        await process.WaitForExitAsync(ct);

        return await ReadSampleOutput(process, stdStreamsTask, resultsDirGuard.Directory, ct);
    }

    static ProcessStartInfo CreateProcessStartInfo(
        AllureSampleRegistryEntry sample,
        IEnumerable<string> args
    ) => new(
        "dotnet",
        [
            "test",
            sample.ProjectPath,
            "--framework",
            sample.TargetFramework,
            "--configuration",
            sample.BuildConfiguration,
            ..args,
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

    static void ApplyExtraEnvironmentVariables(
        IEnumerable<KeyValuePair<string, string>> envVars,
        ProcessStartInfo psi
    )
    {
        foreach (var (name, value) in envVars)
        {
            psi.Environment.Add(new (name, value));
        }
    }

    static async Task<TempFile?> MaybeApplyAllureConfig(
        object? allureConfig,
        ProcessStartInfo psi,
        CancellationToken ct
    )
    {
        if (allureConfig is null)
        {
            return null;
        }

        var configPath = Path.GetTempFileName();

        using var fs = new FileStream(configPath, FileMode.Create, FileAccess.Write);
        await JsonSerializer.SerializeAsync(
            fs,
            allureConfig,
            jsonSerializerOptions,
            ct
        );

        psi.Environment["ALLURE_CONFIG"] = configPath;

        return new(configPath);
    }

    static TempDir ApplyAllureResultsDirectory(
        ProcessStartInfo psi,
        string? explicitAllureResultsDirectory
    )
    {
        var resultsDir = explicitAllureResultsDirectory is null
            ? Directory.CreateTempSubdirectory("allure-results-")
                ?? throw new InvalidOperationException("Can't create the Allure result directory")
            : new(explicitAllureResultsDirectory);
        psi.Environment["ALLURE_RESULTSDIR"] = resultsDir.FullName;
        return new(resultsDir);
    }

    static async Task<(string, string)> SetProcessStreamCollection(
        Process process,
        CancellationToken ct
    ) => (
        await CollectProcessStream(process.StandardOutput, ct),
        await CollectProcessStream(process.StandardError, ct)
    );

    static async Task<AllureSampleRunOutput> ReadSampleOutput(
        Process process,
        Task<(string, string)> stdStreamsTask,
        DirectoryInfo resultsDirectory,
        CancellationToken ct
    )
    {
        var allureResults = await ReadAllureResults(resultsDirectory, ct);

        var (stdout, stderr) = await stdStreamsTask;

        return new(process.ExitCode, stdout, stderr, allureResults);
    }

    static Task<string> CollectProcessStream(StreamReader reader, CancellationToken ct) =>
        Task.Factory.StartNew(
            () => reader.ReadToEndAsync(ct).Result,
            TaskCreationOptions.LongRunning
        );

    static async Task<AllureSampleRunOutput.AllureResultData> ReadAllureResults(
        DirectoryInfo resultsDirectory,
        CancellationToken ct
    )
    {
        var resultFiles = resultsDirectory.GetFiles();

        var testResults = await ReadJsonObjectResults(resultFiles, "-result.json", ct);
        var containers = await ReadJsonObjectResults(resultFiles, "-container.json", ct);
        var attachments = await ReadAttachments(resultFiles, ct);

        return new(testResults, containers, attachments);
    }

    static async Task<ImmutableArray<JsonObject>> ReadJsonObjectResults(
        IEnumerable<FileInfo> allOutputFiles,
        string suffix,
        CancellationToken ct
    )
    {
        var jsonResultFiles = allOutputFiles
            .Where((outputFile) => outputFile.Name.EndsWith(suffix))
            .ToArray();
        var jsonObjectResults =
            ImmutableArray.CreateBuilder<JsonObject>(jsonResultFiles.Length);
        foreach (var jsonResultFile in jsonResultFiles)
        {
            using var jsonResultStream = jsonResultFile.OpenRead();
            var jsonNode = await JsonNode.ParseAsync(jsonResultStream, cancellationToken: ct);
            if (jsonNode is JsonObject jsonObject)
            {
                jsonObjectResults.Add(jsonObject);
            }
        }
        return jsonObjectResults.MoveToImmutable();
    }

    static async Task<ImmutableDictionary<string, ReadOnlyMemory<byte>>> ReadAttachments(
        IEnumerable<FileInfo> allOutputFiles,
        CancellationToken ct
    )
    {
        var attachmentFiles = allOutputFiles
            .Where(static (outputFile) =>
                Path.GetFileNameWithoutExtension(outputFile.Name)
                    .EndsWith("-attachment"))
            .ToArray();

        var attachments =
            ImmutableDictionary.CreateBuilder<string, ReadOnlyMemory<byte>>();
        foreach (var attachmentFile in attachmentFiles)
        {
            var bytes = await File.ReadAllBytesAsync(
                attachmentFile.FullName,
                ct
            );
            attachments.Add(attachmentFile.Name, new(bytes));
        }
        return attachments.ToImmutableDictionary();
    }

    static readonly Encoding encoding = new UTF8Encoding(false, false);

    static readonly JsonSerializerOptions jsonSerializerOptions = new()
    {
        WriteIndented = true,
    };

    class TempFile(string path) : IDisposable
    {
        public void Dispose()
        {
            File.Delete(path);
        }
    }

    class TempDir(DirectoryInfo dInfo) : IDisposable
    {
        public DirectoryInfo Directory { get; init; } = dInfo;

        public void Dispose()
        {
            this.Directory.Delete(true);
        }
    }
}
