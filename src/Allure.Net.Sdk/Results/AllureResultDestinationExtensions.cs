using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Sdk.Results;

/// <summary>
/// Convenience overloads for
/// <see cref="IAllureResultsDestination"/> methods.
/// </summary>
public static class AllureResultDestinationExtensions
{
    extension (IAllureResultsDestination destination)
    {
        /// <summary>
        /// Writes a test result to the destination.
        /// </summary>
        public Task WriteTestResultAsync(TestResult testResult) =>
            destination.WriteTestResultAsync(testResult, default);

        /// <summary>
        /// Writes a container to the destination.
        /// </summary>
        public Task WriteContainerAsync(TestResultScope scope) =>
            destination.WriteContainerAsync(scope, default);

        /// <summary>
        /// Writes a globals object to the destination.
        /// </summary>
        public Task WriteGlobalsAsync(Globals globals) =>
            destination.WriteGlobalsAsync(globals, default);

        /// <summary>
        /// Writes an attachment file to the destination.
        /// </summary>
        /// <param name="content">The stream containing the attachment data.</param>
        /// <param name="fileExtension">
        /// The file extension of the attachment file in the output location.
        /// </param>
        /// <returns>The attachment file name in the output location.</returns>
        public Task<string> WriteAttachmentAsync(
            Stream content,
            string fileExtension
        ) =>
            destination.WriteAttachmentAsync(content, fileExtension, default);

        /// <summary>
        /// Copies an attachment file from an existing file to the destination.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>The attachment file name in the output location.</returns>
        public string CopyAttachment(string sourceFilePath) =>
            destination.CopyAttachment(
                sourceFilePath,
                fileExtension: Path.GetExtension(sourceFilePath)
            );

        /// <summary>
        /// Copies an attachment file from an existing file to the destination.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="fileExtension">
        /// The file extension of the attachment file in the output location.
        /// </param>
        /// <returns>The attachment file name in the output location.</returns>
        public Task<string> CopyAttachmentAsync(
            string sourceFilePath,
            string fileExtension
        ) =>
            destination.CopyAttachmentAsync(
                sourceFilePath,
                fileExtension,
                default
            );

        /// <summary>
        /// Copies an attachment file from an existing file to the destination.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <param name="cancellationToken">A cancellation token.</param>
        /// <returns>The attachment file name in the output location.</returns>
        public Task<string> CopyAttachmentAsync(
            string sourceFilePath,
            CancellationToken cancellationToken
        ) =>
            destination.CopyAttachmentAsync(
                sourceFilePath,
                fileExtension: Path.GetExtension(sourceFilePath),
                cancellationToken
            );

        /// <summary>
        /// Copies an attachment file from an existing file to the destination.
        /// </summary>
        /// <param name="sourceFilePath">The source file path.</param>
        /// <returns>The attachment file name in the output location.</returns>
        public Task<string> CopyAttachmentAsync(string sourceFilePath) =>
            destination.CopyAttachmentAsync(
                sourceFilePath,
                fileExtension: Path.GetExtension(sourceFilePath),
                default
            );
    }
}
