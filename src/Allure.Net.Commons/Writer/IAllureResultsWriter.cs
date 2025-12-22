namespace Allure.Net.Commons.Writer
{
    internal interface IAllureResultsWriter
    {
        void Write(TestResult testResult);
        void Write(TestResultContainer testResult);
        void Write(string source, byte[] attachment);
        void CleanUp();
    }
}