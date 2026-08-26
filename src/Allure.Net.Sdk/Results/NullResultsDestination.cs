using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;
using Allure.Sdk.Internal.Functions;

namespace Allure.Sdk.Results;

/// <summary>
/// A destination that discards all results and attachments written to it.
/// </summary>
public class NullResultsDestination : IAllureResultsDestination
{
    /// <inheritdoc/>
    public string CopyAttachment(string sourceFilePath, string fileExtension) =>
        AttachmentSource.CreateName(fileExtension);

    /// <inheritdoc/>
    public Task<string> CopyAttachmentAsync(
        string sourceFilePath,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(
            AttachmentSource.CreateName(fileExtension)
        );

    /// <inheritdoc/>
    public string WriteAttachment(Stream content, string fileExtension) =>
        AttachmentSource.CreateName(fileExtension);

    /// <inheritdoc/>
    public Task<string> WriteAttachmentAsync(
        Stream content,
        string fileExtension,
        CancellationToken cancellationToken
    ) =>
        Task.FromResult(
            AttachmentSource.CreateName(fileExtension)
        );

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
    /// Gets the shared null destination instance.
    /// </summary>
    public static NullResultsDestination Instance { get; } = new();
}
