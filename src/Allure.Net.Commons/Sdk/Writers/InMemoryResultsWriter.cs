using System.Collections.Generic;

namespace Allure.Net.Commons.Sdk.Writers;

/// <summary>
/// A writer that stores all results in memory. Useful for testing.
/// </summary>
public class InMemoryResultsWriter : IAllureResultsWriter
{
    readonly object monitor = new();
    internal List<TestResult> TestResults { get; } = [];
    internal List<TestResultContainer> TestContainers { get; } = [];
    internal List<Globals> Globals { get; } = [];
    internal Dictionary<string, byte[]> ByteAttachments { get; } = [];
    internal Dictionary<string, string> FileAttachments { get; } = [];

    public void CleanUp()
    {
        lock (this.monitor)
        {
            this.TestResults.Clear();
            this.TestContainers.Clear();
            this.Globals.Clear();
            this.ByteAttachments.Clear();
            this.FileAttachments.Clear();
        }
    }

    public void Write(TestResult testResult)
    {
        lock (this.monitor)
        {
            this.TestResults.Add(testResult);
        }
    }

    public void Write(TestResultContainer testResult)
    {
        lock (this.monitor)
        {
            this.TestContainers.Add(testResult);
        }
    }

    public void Write(Globals globals)
    {
        lock (this.monitor)
        {
            this.Globals.Add(globals);
        }
    }

    public void Write(string source, byte[] attachment)
    {
        lock (this.monitor)
        {
            this.ByteAttachments.Add(source, attachment);
        }
    }

    public void Write(string source, string filePath)
    {
        lock (this.monitor)
        {
            this.FileAttachments.Add(source, filePath);
        }
    }
}
