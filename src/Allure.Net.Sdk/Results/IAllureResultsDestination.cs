using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Sdk.Results;

/// <summary>
/// Represents a destination for Allure results.
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
    /// <param name="content">The stream containing the attachment data.</param>
    /// <param name="fileExtension">
    /// The file extension of the attachment file in the output location.
    /// </param>
    /// <returns>The attachment file name in the output location.</returns>
    string WriteAttachment(Stream content, string fileExtension);

    /// <summary>
    /// Writes an attachment file to the destination.
    /// </summary>
    /// <param name="content">The stream containing the attachment data.</param>
    /// <param name="fileExtension">
    /// The file extension of the attachment file in the output location.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The attachment file name in the output location.</returns>
    Task<string> WriteAttachmentAsync(
        Stream content,
        string fileExtension,
        CancellationToken cancellationToken
    );

    /// <summary>
    /// Copies an attachment file from an existing file to the destination.
    /// </summary>
    /// <param name="sourceFilePath">The source file path.</param>
    /// <param name="fileExtension">
    /// The file extension of the attachment file in the output location.
    /// </param>
    /// <returns>The attachment file name in the output location.</returns>
    string CopyAttachment(string sourceFilePath, string fileExtension);

    /// <summary>
    /// Copies an attachment file from an existing file to the destination.
    /// </summary>
    /// <param name="sourceFilePath">The source file path.</param>
    /// <param name="fileExtension">
    /// The file extension of the attachment file in the output location.
    /// </param>
    /// <param name="cancellationToken">A cancellation token.</param>
    /// <returns>The attachment file name in the output location.</returns>
    Task<string> CopyAttachmentAsync(
        string sourceFilePath,
        string fileExtension,
        CancellationToken cancellationToken
    );
}
