namespace Allure.Net.Commons.Writer
{
    /// <summary>
    /// An implementation of this interface is responsible for storing Allure results
    /// in a persistent location.
    /// </summary>
    public interface IAllureResultsWriter
    {
        /// <summary>
        /// Writes a test result after it's fully populated.
        /// </summary>
        void Write(TestResult testResult);

        /// <summary>
        /// Writes a container after its test results have all been writtern and
        /// its fixtures are fully populated.
        /// </summary>
        void Write(TestResultContainer container);

        /// <summary>
        /// Writes a globals object that contains global attachments and errors.
        /// </summary>
        void Write(Globals globals);

        /// <summary>
        /// Writes an attachment's content.
        /// </summary>
        /// <param name="outputFileName">A name of the attachment file in the output location.</param>
        /// <param name="content">The content of the attachment.</param>
        void Write(string outputFileName, byte[] content);

        /// <summary>
        /// Copies a file attachment to the output location.
        /// </summary>
        /// <param name="destinationFileName">A name of the file in the output location.</param>
        /// <param name="sourceFilePath">The path of the file.</param>
        void Write(string destinationFileName, string sourceFilePath);

        /// <summary>
        /// Clears the output location removing all Allure results from it.
        /// </summary>
        void CleanUp();
    }
}