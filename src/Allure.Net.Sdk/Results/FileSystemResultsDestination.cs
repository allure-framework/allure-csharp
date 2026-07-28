using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using System.Text.Json;
using System.Text.Json.Serialization;

namespace Allure.Sdk.Results;

/// <summary>
/// A destination that represents a directory in the local file system.
/// </summary>
public class FileSystemResultsDestination : IAllureResultsDestination
{
    const int DefaultCopyBufferSize = 81920;
    private readonly string outputDirectory;
    private readonly JsonSerializerOptions serializerOptions;

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

    public void WriteTestResult(TestResult testResult)
    {
        this.WriteAllureObject(testResult, "-result.json");
    }

    public async Task WriteTestResultAsync(TestResult testResult, CancellationToken cancellationToken)
    {
        await this.WriteAllureObjectAsync(testResult, "-result.json", cancellationToken);
    }

    public void WriteContainer(TestResultScope container)
    {
        this.WriteAllureObject(container, "-container.json");
    }

    public async Task WriteContainerAsync(TestResultScope container, CancellationToken cancellationToken)
    {
        await this.WriteAllureObjectAsync(container, "-container.json", cancellationToken);
    }

    public void WriteGlobals(Globals globals)
    {
        this.WriteAllureObject(globals, "-globals.json");
    }

    public async Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken)
    {
        await this.WriteAllureObjectAsync(globals, "-globals.json", cancellationToken);
    }

    public void WriteAttachment(string outputFileName, Stream content)
    {
        using FileStream output = this.CreateOutput(outputFileName);
        content.CopyTo(output);
    }

    public async Task WriteAttachmentAsync(
        string outputFileName,
        Stream content,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        using FileStream output = this.CreateOutput(outputFileName);

        await content.CopyToAsync(output, DefaultCopyBufferSize, cancellationToken);
    }

    public void CopyAttachment(string destinationFileName, string sourceFilePath)
    {
        var outputFilePath = Path.Combine(outputDirectory, destinationFileName);

        this.EnsureDirectory();

        File.Copy(sourceFilePath, outputFilePath);
    }

    public async Task CopyAttachmentAsync(
        string destinationFileName,
        string sourceFilePath,
        CancellationToken cancellationToken
    )
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var source = File.OpenRead(sourceFilePath);
        using var output = this.CreateOutput(destinationFileName);

        await source.CopyToAsync(output, DefaultCopyBufferSize, cancellationToken);
    }

    FileStream CreateOutput(string name)
    {
        var outputFilePath = Path.Combine(outputDirectory, name);

        this.EnsureDirectory();

        return File.OpenWrite(outputFilePath);
    }

    void WriteAllureObject(object allureObject, string suffix)
    {
        using var fileStream = this.CreateAllureObjectOutputStream(suffix);
        JsonSerializer.Serialize(fileStream, allureObject, serializerOptions);
    }

    async Task WriteAllureObjectAsync(object allureObject, string suffix, CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        using var fileStream = this.CreateAllureObjectOutputStream(suffix);

        await JsonSerializer.SerializeAsync(fileStream, allureObject, serializerOptions, cancellationToken);
    }

    FileStream CreateAllureObjectOutputStream(string suffix)
    {
        var uuid = Guid.NewGuid();
        var outputFileName = $"{uuid:N}{suffix}";
        var outputFilePath = Path.Combine(outputDirectory, outputFileName);

        this.EnsureDirectory();

        return File.OpenWrite(outputFilePath);
    }

    void EnsureDirectory()
    {
        if (!Directory.Exists(this.outputDirectory))
        {
            Directory.CreateDirectory(this.outputDirectory);
        }
    }
}