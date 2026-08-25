using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using System.Text.Json;
using System.Text.Json.Serialization;
using Allure.Sdk.Internal.Functions;


namespace Allure.Sdk.Results;

/// <summary>
/// A destination that writes Allure results to a local file-system directory.
/// </summary>
public class FileSystemResultsDestination : IAllureResultsDestination
{
    const int DefaultCopyBufferSize = 81920;
    const int DefaultCreateBufferSize = 1024 * 4;
    private readonly string outputDirectory;
    private readonly JsonSerializerOptions serializerOptions;

    /// <summary>
    /// Initializes a destination that writes to the specified directory.
    /// </summary>
    /// <param name="directoryPath">The output directory path.</param>
    /// <param name="indentJson">Whether to indent generated JSON files.</param>
    public FileSystemResultsDestination(string directoryPath, bool indentJson)
    {
        this.outputDirectory = directoryPath;

        this.serializerOptions = new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            WriteIndented = indentJson,
            DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
            Converters =
            {
                new JsonStringEnumConverter<Status>(
                    namingPolicy: JsonNamingPolicy.CamelCase,
                    allowIntegerValues: false
                ),
                new JsonStringEnumConverter<Stage>(
                    namingPolicy: JsonNamingPolicy.CamelCase,
                    allowIntegerValues: false
                ),
                new JsonStringEnumConverter<ParameterMode>(
                    namingPolicy: JsonNamingPolicy.CamelCase,
                    allowIntegerValues: false
                ),
                new JsonStringEnumConverter<Severity>(
                    namingPolicy: JsonNamingPolicy.CamelCase,
                    allowIntegerValues: false
                ),
            },
        };
    }

    /// <inheritdoc/>
    public void WriteTestResult(TestResult testResult)
    {
        this.WriteAllureObject(testResult, "-result.json");
    }

    /// <inheritdoc/>
    public async Task WriteTestResultAsync(TestResult testResult, CancellationToken cancellationToken)
    {
        await this.WriteAllureObjectAsync(testResult, "-result.json", cancellationToken);
    }

    /// <inheritdoc/>
    public void WriteContainer(TestResultScope container)
    {
        if (ShouldIgnoreScope(container))
        {
            return;
        }

        this.WriteAllureObject(container, "-container.json");
    }

    /// <inheritdoc/>
    public async Task WriteContainerAsync(TestResultScope container, CancellationToken cancellationToken)
    {
        if (ShouldIgnoreScope(container))
        {
            return;
        }

        await this.WriteAllureObjectAsync(container, "-container.json", cancellationToken);
    }

    /// <inheritdoc/>
    public void WriteGlobals(Globals globals)
    {
        this.WriteAllureObject(globals, "-globals.json");
    }

    /// <inheritdoc/>
    public async Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken)
    {
        await this.WriteAllureObjectAsync(globals, "-globals.json", cancellationToken);
    }

    /// <inheritdoc/>
    public string WriteAttachment(Stream content, string fileExtension)
    {
        var outputFileName = AttachmentSource.CreateName(fileExtension);

        using var writer = new AtomicFileWriter(this.outputDirectory, outputFileName);

        content.CopyTo(writer.Stream);
        writer.Commit();

        return outputFileName;
    }

    /// <inheritdoc/>
    public async Task<string> WriteAttachmentAsync(
        Stream content,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var outputFileName = AttachmentSource.CreateName(fileExtension);

        using var writer = new AtomicFileWriter(
            this.outputDirectory,
            outputFileName,
            FileOptions.Asynchronous
        );

        await content.CopyToAsync(writer.Stream, DefaultCopyBufferSize, cancellationToken);
        writer.Commit();

        return outputFileName;
    }

    /// <inheritdoc/>
    public string CopyAttachment(string sourceFilePath, string fileExtension)
    {
        var outputFileName = AttachmentSource.CreateName(fileExtension);

        using var writer = new AtomicFileWriter(this.outputDirectory, outputFileName);
        using var source = File.OpenRead(sourceFilePath);

        source.CopyTo(writer.Stream);
        writer.Commit();

        return outputFileName;
    }

    /// <inheritdoc/>
    public async Task<string> CopyAttachmentAsync(
        string sourceFilePath,
        string fileExtension,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        var outputFileName = AttachmentSource.CreateName(fileExtension);

        using var writer = new AtomicFileWriter(
            this.outputDirectory,
            outputFileName,
            FileOptions.Asynchronous
        );
        using var source = File.OpenRead(sourceFilePath);

        await source.CopyToAsync(writer.Stream, DefaultCopyBufferSize, cancellationToken);

        writer.Commit();

        return outputFileName;
    }

    static bool ShouldIgnoreScope(TestResultScope scope) => scope is
    {
        Befores.Count: 0,
        Afters.Count: 0,
    };

    void WriteAllureObject(object allureObject, string suffix)
    {
        using var writer = new AtomicFileWriter(this.outputDirectory, CreateResultFileName(suffix));
        JsonSerializer.Serialize(writer.Stream, allureObject, serializerOptions);
        writer.Commit();
    }

    async Task WriteAllureObjectAsync(object allureObject, string suffix, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var writer = new AtomicFileWriter(this.outputDirectory, CreateResultFileName(suffix), FileOptions.Asynchronous);

        await JsonSerializer.SerializeAsync(writer.Stream, allureObject, serializerOptions, cancellationToken);

        writer.Commit();
    }

    static string CreateResultFileName(string suffix) =>
        $"{Guid.NewGuid():N}{suffix}";

    class AtomicFileWriter : IDisposable
    {
        readonly string tmpPath;
        readonly string outputPath;

        public FileStream Stream { get; }

        public AtomicFileWriter(string directory, string fileName, FileOptions extraOptions = FileOptions.None)
        {
            var uuid = Guid.NewGuid().ToString("N");
            this.tmpPath = Path.Combine(directory, $".allure-write-{uuid}.tmp");
            this.outputPath = Path.Combine(directory, fileName);

            Directory.CreateDirectory(directory);

            this.Stream = new(
                this.tmpPath,
                mode: FileMode.CreateNew,
                access: FileAccess.Write,
                share: FileShare.None,
                bufferSize: DefaultCreateBufferSize,
                options: FileOptions.SequentialScan | extraOptions
            );
        }

        public void Commit()
        {
            this.Stream.Flush(flushToDisk: true);
            this.Stream.Close();

            File.Move(this.tmpPath, this.outputPath);
        }

        public void Dispose()
        {
            this.Stream.Dispose();

            try
            {
                File.Delete(this.tmpPath);
            }
            catch
            {
            }
        }
    }
}
