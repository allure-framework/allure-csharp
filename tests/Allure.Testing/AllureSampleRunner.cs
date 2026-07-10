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
using Allure.Testing.Assertions.Model;
using Allure.Testing.Execution;
using Allure.Testing.Internal;
using TUnit.Assertions.Core;
using TUnit.Assertions.Exceptions;

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

    /// <summary>
    /// Reads the Allure result files in the provided directory.
    /// </summary>
    /// <param name="resultsDirectory">A directory to read the files from.</param>
    public static async Task<AllureResults> ReadAllureResults(string resultsDirectory) =>
        await ReadAllureResults(new DirectoryInfo(resultsDirectory), CancellationToken.None);

    /// <summary>
    /// Reads the Allure result files in the provided directory.
    /// </summary>
    /// <param name="resultsDirectory">A directory to read the files from.</param>
    /// <param name="ct">A cancellation token.</param>
    public static async Task<AllureResults> ReadAllureResults(
        string resultsDirectory,
        CancellationToken ct
    ) =>
        await ReadAllureResults(new DirectoryInfo(resultsDirectory), ct);

    /// <summary>
    /// Reads the Allure result files in the provided directory.
    /// </summary>
    /// <param name="resultsDirectory">A directory to read the files from.</param>
    public static async Task<AllureResults> ReadAllureResults(DirectoryInfo resultsDirectory) =>
        await ReadAllureResults(resultsDirectory, CancellationToken.None);

    /// <summary>
    /// Reads the Allure result files in the provided directory.
    /// </summary>
    /// <param name="resultsDirectory">A directory to read the files from.</param>
    /// <param name="ct">A cancellation token.</param>
    public static async Task<AllureResults> ReadAllureResults(
        DirectoryInfo resultsDirectory,
        CancellationToken ct
    )
    {
        var resultFiles = resultsDirectory.Exists
            ? resultsDirectory.GetFiles()
            : [];

        var testResults = await ReadJsonObjectResults2<AllureTestResult>(resultFiles, "-result.json", ct);
        var containers = await ReadJsonObjectResults2<AllureContainer>(resultFiles, "-container.json", ct);
        var globals = await ReadJsonObjectResults2<AllureGlobals>(resultFiles, "-globals.json", ct);

        return (testResults, containers, globals) switch
        {
            ({IsPassed: true, Value: var trs}, {IsPassed: true, Value: var conts}, { IsPassed: true, Value: var globs }) => new(
                TestResults: trs,
                Containers: conts,
                Attachments: await ReadAttachments(resultFiles, ct),
                Globals: globs
            ),

            ({IsPassed: false, Message: var err1}, {IsPassed: false, Message: var err2}, { IsPassed: false, Message: var err3 }) =>
                throw new AssertionException($"{err1}{Environment.NewLine}{err2}{Environment.NewLine}{err3}"),

            ({IsPassed: false, Message: var err1}, {IsPassed: false, Message: var err2}, _) =>
                throw new AssertionException($"{err1}{Environment.NewLine}{err2}"),

            ({IsPassed: false, Message: var err1}, _, {IsPassed: false, Message: var err2}) =>
                throw new AssertionException($"{err1}{Environment.NewLine}{err2}"),

            (_, {IsPassed: false, Message: var err1}, {IsPassed: false, Message: var err2}) =>
                throw new AssertionException($"{err1}{Environment.NewLine}{err2}"),

            ({IsPassed: false, Message: var error}, _, _) =>
                throw new AssertionException(error),

            (_, {IsPassed: false, Message: var error}, _) =>
                throw new AssertionException(error),

            (_, _, {IsPassed: false, Message: var error}) =>
                throw new AssertionException(error),
        };
    }

    static async Task<Guard<DirectoryInfo>> EnsureSampleResults(
        AllureSampleRegistryEntry sample,
        AllureSampleRunInput input,
        CancellationToken ct
    ) =>
        sample.IsPreRunFlow && input.IsPreRunCompatible
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

        using var resultsDirGuard
            = EnsureAllureResultsDirectory(input.AllureResultsDirectory);
        using var allureConfigGuard
            = await ApplyAllureConfig(
                input.AllureConfiguration,
                resultsDirGuard.Value.FullName,
                psi,
                ct
            );

        LogProcessStart(psi, input);

        using var process = Process.Start(psi) ??
            throw new InvalidOperationException("Unable to start a process");

        // Make sure the process tree is stopped if an exception occurs.
        using var processGuard = Guard.WrapProcess(process);

        var stdStreamsTask = CollectProcessOutput(process, ct);

        await WaitForExit(process, input.Timeout, ct);

        LogProcessFinish(process);

        var (stdout, stderr) = await stdStreamsTask;

        LogStdStreams(stdout, stderr);

        return resultsDirGuard.Own
            ? resultsDirGuard.Transfer()
            : resultsDirGuard.Value;
    }

    static void LogProcessStart(ProcessStartInfo psi, AllureSampleRunInput input)
    {
        Console.WriteLine(
            "Running {0} {1}",
            psi.FileName,
            string.Join(" ", psi.ArgumentList.Select(a => $"'{a}'"))
        );

        Console.WriteLine("  Working directory: {0}", psi.WorkingDirectory);

        if (input.EnvironmentVariables.Any())
        {
            Console.WriteLine("  Extra environment variables:");
            foreach (var (name, value) in input.EnvironmentVariables)
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
        LogStdStream("Standard error", stderr);
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
    )
    {
        ProcessStartInfo psi = new(
            "dotnet",
            [
                ..ResolveTestingPlatformSpecificArguments(sample.TestingPlatform),
                sample.ProjectFilePath,
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
        psi.Environment["DOTNET_CLI_TELEMETRY_OPTOUT"] = "1";
        return psi;
    }

    static IEnumerable<string> ResolveTestingPlatformSpecificArguments(TestingPlatform testingPlatform) =>
        testingPlatform switch
        {
            TestingPlatform.VsTest => ["test"],
            TestingPlatform.MicrosoftTestingPlatform => ["run", "--project"],
            var value => throw new InvalidOperationException(
                $"Unknown testing platform '{value}'"
            ),
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

    static async Task<Guard<string>?> ApplyAllureConfig(
        object? allureConfigInput,
        string resultsDir,
        ProcessStartInfo psi,
        CancellationToken ct
    )
    {
        var allureConfig = ResolveAllureConfig(allureConfigInput, resultsDir);
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

    static JsonObject ResolveAllureConfig(object? config, string resultsDir)
    {
        if (config is null)
        {
            return GetDirectoryOnlyAllureConfig(resultsDir);
        }

        var allureConfigJson = JsonSerializer.SerializeToNode(config);
        if (allureConfigJson is not JsonObject allureConfigJsonObject)
        {
            throw new InvalidOperationException("Allure config must be an object");
        }

        var allure = allureConfigJsonObject["allure"];
        if (allure is not null)
        {
            allure["directory"] = resultsDir;
        }
        else
        {
            allureConfigJsonObject["allure"] = new JsonObject([new("directory", resultsDir)]);
        }

        return allureConfigJsonObject;
    }

    static JsonObject GetDirectoryOnlyAllureConfig(string resultsDir) => new ([
        new (
            "allure",
            new JsonObject([new("directory", resultsDir)])
        ),
    ]);


    static Guard<DirectoryInfo> EnsureAllureResultsDirectory(
        string? explicitAllureResultsDirectory
    )
    {
        bool useTempDir = explicitAllureResultsDirectory is null;
        var resultsDir = useTempDir ?
            Directory.CreateTempSubdirectory("allure-results-")
                ?? throw new InvalidOperationException("Can't create the Allure result directory")
            : new(explicitAllureResultsDirectory!);
        return Guard.WrapDirectory(resultsDir, own: useTempDir);
    }

    static async Task<(string, string)> CollectProcessOutput(
        Process process,
        CancellationToken ct
    )
    {
        var streams = await Task.WhenAll(
            CollectProcessStream(process.StandardOutput, ct),
            CollectProcessStream(process.StandardError, ct)
        );
        return (streams[0], streams[1]);
    }

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

    static async Task<AssertionResult<ImmutableArray<T>>> ReadJsonObjectResults2<T>(
        IEnumerable<FileInfo> allOutputFiles,
        string suffix,
        CancellationToken ct
    ) where T : IAllureModelObject<T>
    {
        var jsonResultFiles = allOutputFiles
            .Where((outputFile) => outputFile.Name.EndsWith(suffix))
            .ToArray();
        var items = ImmutableArray.CreateBuilder<T>(jsonResultFiles.Length);
        List<string> errors = [];
        foreach (var jsonResultFile in jsonResultFiles)
        {
            string? parsingFailureMessage = null;

            try
            {
                await using var jsonStream = jsonResultFile.OpenRead();
                using var jsonDocument = await JsonDocument.ParseAsync(jsonStream, cancellationToken: ct);
                var objCreationResult = T.Create(jsonDocument.RootElement.Clone());

                if (objCreationResult.IsPassed)
                {
                    items.Add(objCreationResult.Value!);
                }
                else
                {
                    errors.Add($"{jsonResultFile.FullName}: {objCreationResult.Message}");
                }
            }
            catch (JsonException e)
            {
                parsingFailureMessage = e.Message;
                errors.Add($"{jsonResultFile.FullName}: {parsingFailureMessage}");
            }
        }

        return errors.Count == 0
            ? AssertionResult<ImmutableArray<T>>.Passed(items.MoveToImmutable())
            : AssertionResult.Failed(
                $"Errors while parsing {suffix} files:"
                    + string.Join("", errors.Select(e => $"{Environment.NewLine}  - {e}")));
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
