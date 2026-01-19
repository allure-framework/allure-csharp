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
using Allure.Testing.Internal;

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
    public static async Task<AllureResults> RunAsync(AllureSampleRegistryEntry sample) =>
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
    public static async Task<AllureResults> RunAsync(
        AllureSampleRegistryEntry sample,
        CancellationToken ct
    ) =>
        await RunAsync(sample, AllureSampleRunInput.Default, ct);

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
    public static async Task<AllureResults> RunAsync(
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
    public static async Task<AllureResults> RunAsync(
        AllureSampleRegistryEntry sample,
        AllureSampleRunInput input,
        CancellationToken ct
    )
    {
        using var allureResultsDir = await EnsureSampleResults(sample, input, ct);

        var allureResults = await ReadAllureResults(allureResultsDir.Value, ct);

        return allureResults;
    }

    static async Task<Guard<DirectoryInfo>> EnsureSampleResults(
        AllureSampleRegistryEntry sample,
        AllureSampleRunInput input,
        CancellationToken ct
    ) =>
        sample.IsAssertionOnly
            ? new DirectoryInfo(sample.DefaultResultsPath)
            : await ProduceSampleResults(sample, input, ct);

    static async Task<Guard<DirectoryInfo>> ProduceSampleResults(
        AllureSampleRegistryEntry sample,
        AllureSampleRunInput input,
        CancellationToken ct
    )
    {
        var psi = CreateProcessStartInfo(sample, input.ProcessArguments);

        ApplyExtraEnvironmentVariables(input.EnvironmentVariables, psi);

        using var allureConfigGuard
            = await MaybeApplyAllureConfig(input.AllureConfiguration, psi, ct);
        using var resultsDirGuard
            = ApplyAllureResultsDirectory(psi, input.AllureResultsDirectory);

        LogProcessStart(psi);

        using var process = Process.Start(psi) ??
            throw new InvalidOperationException("Unable to start a process");

        // Make sure the process tree is stopped if an exception occurs.
        using var processGuard = Guard.WrapProcess(process);

        var stdStreamsTask = SetProcessStreamCollection(process, ct);

        await WaitForExit(process, input.Timeout, ct);

        LogProcessFinish(process);

        var (stdout, stderr) = await stdStreamsTask;

        LogStdStreams(stdout, stderr);

        return resultsDirGuard.Transfer();
    }

    static void LogProcessStart(ProcessStartInfo psi)
    {
        Console.WriteLine(
            "Running {0} {1}",
            psi.FileName,
            string.Join(" ", psi.Arguments.Select(a => $"'{a}'"))
        );

        Console.WriteLine("  Working directory: {0}", psi.WorkingDirectory);

        if (psi.Environment.Any())
        {
            Console.WriteLine("  Environment variables:");
            foreach (var (name, value) in psi.Environment)
            {
                Console.WriteLine("    {0}={1}", name, value);
            }
        }
    }

    static void LogProcessFinish(Process process)
    {
        Console.WriteLine(
            "{0} finished with exit code {1}",
            process.StartInfo.FileName,
            process.ExitCode
        );
    }

    static void LogStdStreams(string stdout, string stderr)
    {
        LogStdStream("Standard output", stdout);
        LogStdStream("Standard error", stdout);
    }

    static void LogStdStream(string name, string output)
    {
        if (!string.IsNullOrEmpty(output))
        {
            Console.WriteLine("{0}:", name);
            Console.WriteLine("");
            Console.WriteLine(output);
            Console.WriteLine("");
        }
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

    static async Task<Guard<string>?> MaybeApplyAllureConfig(
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

        return Guard.WrapFile(configPath);
    }

    static Guard<DirectoryInfo> ApplyAllureResultsDirectory(
        ProcessStartInfo psi,
        string? explicitAllureResultsDirectory
    )
    {
        bool useTempDir = explicitAllureResultsDirectory is null;
        var resultsDir = useTempDir ?
            Directory.CreateTempSubdirectory("allure-results-")
                ?? throw new InvalidOperationException("Can't create the Allure result directory")
            : new(explicitAllureResultsDirectory!);
        psi.Environment["ALLURE_RESULTSDIR"] = resultsDir.FullName;
        return Guard.WrapDirectory(resultsDir, useTempDir);
    }

    static async Task<(string, string)> SetProcessStreamCollection(
        Process process,
        CancellationToken ct
    ) => (
        await CollectProcessStream(process.StandardOutput, ct),
        await CollectProcessStream(process.StandardError, ct)
    );

    static async Task WaitForExit(Process process, TimeSpan timeout, CancellationToken ct)
    {
        using var cts = ApplyTimeout(timeout, ct);

        try
        {
            await process.WaitForExitAsync(cts.Token);
        }
        catch (TaskCanceledException e)
        {
            if (e.CancellationToken.IsCancellationRequested && !ct.IsCancellationRequested)
            {
                throw new TimeoutException($"A timeout of {timeout} was reached.");
            }

            throw;
        }
    }

    static CancellationTokenSource ApplyTimeout(TimeSpan timeout, CancellationToken ct)
    {
        var cts = CancellationTokenSource.CreateLinkedTokenSource(ct);
        cts.CancelAfter(timeout);
        return cts;
    }

    static Task<string> CollectProcessStream(StreamReader reader, CancellationToken ct) =>
        Task.Factory.StartNew(
            () => reader.ReadToEndAsync(ct).Result,
            TaskCreationOptions.LongRunning
        );

    static async Task<AllureResults> ReadAllureResults(
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
}
