namespace Allure.Net.Commons.Storage
{
    public interface ITestResultAccessor
    {
        TestResultContainer TestResultContainer { get; set; }
        TestResult TestResult { get; set; }
    }
}