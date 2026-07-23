using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Sdk.Results;

/// <summary>
/// Represents a destination of Allure results.
/// </summary>
public interface IAllureResultsDestination
{
    /// <summary>
    /// Writes a test result to the destination.
    /// </summary>
    void WriteTestResult(TestResult testResult);

    /// <summary>
    /// Writes a test result to the destination.
    /// </summary>
    Task WriteTestResultAsync(TestResult testResult, CancellationToken cancellationToken);

    /// <summary>
    /// Writes a container to the destination.
    /// </summary>
    void WriteContainer(TestResultScope scope);

    /// <summary>
    /// Writes a container to the destination.
    /// </summary>
    Task WriteContainerAsync(TestResultScope scope, CancellationToken cancellationToken);

    /// <summary>
    /// Writes a globals object to the destination.
    /// </summary>
    void WriteGlobals(Globals globals);

    /// <summary>
    /// Writes a globals object to the destination.
    /// </summary>
    Task WriteGlobalsAsync(Globals globals, CancellationToken cancellationToken);

    /// <summary>
    /// Writes an attachment file to the destination.
    /// </summary>
    /// <param name="outputFileName">A name of the attachment file in the output location.</param>
    /// <param name="content">A stream thad defines the content of the attachment.</param>
    void WriteAttachment(string outputFileName, Stream content);

    /// <summary>
    /// Writes an attachment file to the destination.
    /// </summary>
    /// <param name="outputFileName">A name of the attachment file in the output location.</param>
    /// <param name="content">A stream thad defines the content of the attachment.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task WriteAttachmentAsync(
        string outputFileName,
        Stream content,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Copies an attachment file from an existing file to the destination.
    /// </summary>
    /// <param name="destinationFileName">A name of the file in the output location.</param>
    /// <param name="sourceFilePath">The path of the file.</param>
    void CopyAttachment(string destinationFileName, string sourceFilePath);

    /// <summary>
    /// Copies an attachment file from an existing file to the destination.
    /// </summary>
    /// <param name="destinationFileName">A name of the file in the output location.</param>
    /// <param name="sourceFilePath">The path of the file.</param>
    /// <param name="cancellationToken">A cancellation token.</param>
    Task CopyAttachmentAsync(
        string destinationFileName,
        string sourceFilePath,
        CancellationToken cancellationToken
    );
}