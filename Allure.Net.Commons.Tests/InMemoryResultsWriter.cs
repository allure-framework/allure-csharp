using System.Collections.Generic;
using Allure.Net.Commons.Writer;

namespace Allure.Net.Commons.Tests
{
    class InMemoryResultsWriter : IAllureResultsWriter
    {
        internal List<TestResult> testResults = new();
        internal List<TestResultContainer> testContainers = new();
        internal List<(string Source, byte[] Content)> attachments = new();

        public void CleanUp()
        {
            this.testResults.Clear();
            this.testContainers.Clear();
            this.attachments.Clear();
        }

        public void Write(TestResult testResult)
        {
            this.testResults.Add(testResult);
        }

        public void Write(TestResultContainer testResult)
        {
            this.testContainers.Add(testResult);
        }

        public void Write(string source, byte[] attachment)
        {
            this.attachments.Add((source, attachment));
        }
    }
}
