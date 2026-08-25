using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using Allure.Sdk.Internal.Functions;

namespace Allure.Sdk.Results;

/// <summary>
/// A destination that stores all results in memory. Useful for testing.
/// </summary>
public class InMemoryResultsDestination : IAllureResultsDestination
{
    readonly object monitor = new();

    /// <summary>
    /// Gets a list of all written test results.
    /// </summary>
    public List<TestResult> TestResults { get; } = [];

    /// <summary>
    /// Gets a list of all written containers.
    /// </summary>
    public List<TestResultScope> TestContainers { get; } = [];

    /// <summary>
    /// Gets a list of all written globals.
    /// </summary>
    public List<Globals> Globals { get; } = [];

    /// <summary>
    /// Gets attachment data indexed by output file name.
    /// </summary>
    public Dictionary<string, byte[]> ByteAttachments { get; } = [];

    /// <summary>
    /// Gets source file paths indexed by destination file name.
    /// </summary>
    public Dictionary<string, string> FileAttachments { get; } = [];

    /// <inheritdoc/>
    public string CopyAttachment(string sourceFilePath, string fileExtension)
    {
        var destinationFileName = AttachmentSource.CreateName(fileExtension);
        lock (this.monitor)
        {
            this.FileAttachments.Add(destinationFileName, sourceFilePath);
        }
        return destinationFileName;
    }

    /// <inheritdoc/>
    public Task<string> CopyAttachmentAsync(
        string sourceFilePath,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(
            this.CopyAttachment(sourceFilePath, fileExtension)
        );

    /// <inheritdoc/>
    public string WriteAttachment(Stream content, string fileExtension)
    {
        var outputFileName = AttachmentSource.CreateName(fileExtension);

        using MemoryStream memoryStream = new();
        content.CopyTo(memoryStream);
        var data = memoryStream.ToArray();

        lock (this.monitor)
        {
            this.ByteAttachments.Add(outputFileName, data);
        }

        return outputFileName;
    }

    /// <inheritdoc/>
    public Task<string> WriteAttachmentAsync(
        Stream content,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(
            this.WriteAttachment(content, fileExtension)
        );

    /// <inheritdoc/>
    public void WriteGlobals(Globals globals)
    {
        lock(this.monitor)
        {
            this.Globals.Add(globals);
        }
    }

    /// <inheritdoc/>
    public Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken)
    {
        this.WriteGlobals(globals);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void WriteContainer(TestResultScope container)
    {
        lock(this.monitor)
        {
            this.TestContainers.Add(container);
        }
    }

    /// <inheritdoc/>
    public Task WriteContainerAsync(TestResultScope container, CancellationToken cancellationToken)
    {
        this.WriteContainer(container);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void WriteTestResult(TestResult testResult)
    {
        lock(this.monitor)
        {
            this.TestResults.Add(testResult);
        }
    }

    /// <inheritdoc/>
    public Task WriteTestResultAsync(TestResult testResult, CancellationToken cancellationToken)
    {
        this.WriteTestResult(testResult);
        return Task.CompletedTask;
    }

    /// <summary>
    /// Removes all stored results and attachments.
    /// </summary>
    public void Clear()
    {
        lock(this.monitor)
        {
            this.TestContainers.Clear();
            this.TestResults.Clear();
            this.Globals.Clear();
            this.ByteAttachments.Clear();
            this.FileAttachments.Clear();
        }
    }
}
