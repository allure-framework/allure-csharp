using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Sdk.Results;

/// <summary>
/// An absent destination that ignores everything written to it.
/// </summary>
public class NullResultsDestination : IAllureResultsDestination
{
    /// <inheritdoc/>
    public void CopyAttachment(string destinationFileName, string sourceFilePath)
    {
    }

    /// <inheritdoc/>
    public Task CopyAttachmentAsync(
        string destinationFileName,
        string sourceFilePath,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;

    /// <inheritdoc/>
    public void WriteAttachment(string outputFileName, Stream content)
    {
    }

    /// <inheritdoc/>
    public Task WriteAttachmentAsync(
        string outputFileName,
        Stream content,
        CancellationToken cancellationToken
    ) =>
        Task.CompletedTask;

    /// <inheritdoc/>
    public void WriteGlobals(Globals globals)
    {
    }

    /// <inheritdoc/>
    public Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    /// <inheritdoc/>
    public void WriteContainer(TestResultScope container)
    {
    }

    /// <inheritdoc/>
    public Task WriteContainerAsync(TestResultScope container, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    /// <inheritdoc/>
    public void WriteTestResult(TestResult testResult)
    {
    }

    /// <inheritdoc/>
    public Task WriteTestResultAsync(TestResult testResult, CancellationToken cancellationToken) =>
        Task.CompletedTask;

    /// <summary>
    /// A cached instance of the null destination.
    /// </summary>
    public static NullResultsDestination Instance { get; } = new();
}