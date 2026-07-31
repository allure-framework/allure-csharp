using System.Collections.Generic;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

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
    public void CopyAttachment(string destinationFileName, string sourceFilePath)
    {
        lock (this.monitor)
        {
            this.FileAttachments.Add(destinationFileName, sourceFilePath);
        }
    }

    /// <inheritdoc/>
    public Task CopyAttachmentAsync(string destinationFileName, string sourceFilePath, CancellationToken cancellationToken)
    {
        this.CopyAttachment(destinationFileName, sourceFilePath);
        return Task.CompletedTask;
    }

    /// <inheritdoc/>
    public void WriteAttachment(string outputFileName, Stream content)
    {
        using MemoryStream memoryStream = new();
        content.CopyTo(memoryStream);
        var data = memoryStream.ToArray();

        lock (this.monitor)
        {
            this.ByteAttachments.Add(outputFileName, data);
        }
    }

    /// <inheritdoc/>
    public Task WriteAttachmentAsync(string outputFileName, Stream content, CancellationToken cancellationToken)
    {
        this.WriteAttachment(outputFileName, content);
        return Task.CompletedTask;
    }

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
