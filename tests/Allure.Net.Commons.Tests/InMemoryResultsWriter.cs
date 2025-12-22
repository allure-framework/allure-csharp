using System.Collections.Generic;
using Allure.Net.Commons.Writer;

namespace Allure.Net.Commons.Tests
{
    class InMemoryResultsWriter : IAllureResultsWriter
    {
        readonly object monitor = new();
        internal List<TestResult> testResults = new();
        internal List<TestResultContainer> testContainers = new();
        internal List<(string Source, byte[] Content)> attachments = new();

        public void CleanUp()
        {
            lock (this.monitor)
            {
                this.testResults.Clear();
                this.testContainers.Clear();
                this.attachments.Clear();
            }
        }

        public void Write(TestResult testResult)
        {
            lock (this.monitor)
            {
                this.testResults.Add(testResult);
            }
        }

        public void Write(TestResultContainer testResult)
        {
            lock (this.monitor)
            {
                this.testContainers.Add(testResult);
            }
        }

        public void Write(string source, byte[] attachment)
        {
            lock (this.monitor)
            {
                this.attachments.Add((source, attachment));
            }
        }
    }
}
