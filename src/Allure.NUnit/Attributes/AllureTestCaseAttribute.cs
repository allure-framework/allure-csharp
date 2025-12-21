using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Attributes
{
    public abstract class AllureTestCaseAttribute : NUnitAttribute
    {
        public abstract void UpdateTestResult(TestResult testResult);
    }
}