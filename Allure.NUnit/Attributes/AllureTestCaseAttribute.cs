using Allure.Net.Commons;
using NUnit.Framework;

namespace NUnit.Allure.Attributes
{
    public abstract class AllureTestCaseAttribute : NUnitAttribute
    {
        public abstract void UpdateTestResult(TestResult testResult);
    }
}