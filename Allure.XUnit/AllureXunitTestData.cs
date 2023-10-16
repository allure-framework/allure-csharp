using Allure.Net.Commons;

#nullable enable

namespace Allure.XUnit
{
    class AllureXunitTestData
    {
        public AllureContext? Context { get; set; }
        public object[]? Arguments { get; set; }
        public TestResult? TestResult { get; set; }
        public bool IsSelected { get; set; } = true;
    }
}