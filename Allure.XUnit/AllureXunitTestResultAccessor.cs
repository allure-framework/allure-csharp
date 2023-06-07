using Allure.Net.Commons;
using Allure.Net.Commons.Storage;

namespace Allure.XUnit
{
    class AllureXunitTestResultAccessor : ITestResultAccessor
    {
        public TestResultContainer TestResultContainer { get; set; }

        public TestResult TestResult { get; set; }

        public object[] Arguments { get; set; }
    }
}