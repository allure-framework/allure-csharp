using Allure.Net.Commons;

namespace NUnit.Allure.Core
{
    public interface ITestResultAccessor
    {
        TestResultContainer TestResultContainer { get; set; }
        TestResult TestResult { get; set; }
    }
}