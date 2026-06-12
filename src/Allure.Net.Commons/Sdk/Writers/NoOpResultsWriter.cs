namespace Allure.Net.Commons.Sdk.Writers;

/// <summary>
/// A writer that supresses all results from being written.
/// </summary>
public class NoOpResultsWriter : IAllureResultsWriter
{
    public void CleanUp() { }

    public void Write(TestResult testResult) { }

    public void Write(TestResultContainer container) { }

    public void Write(Globals globals) { }

    public void Write(string outputFileName, byte[] content) { }

    public void Write(string destinationFileName, string sourceFilePath) { }

    public static NoOpResultsWriter Instance { get; } = new();
}