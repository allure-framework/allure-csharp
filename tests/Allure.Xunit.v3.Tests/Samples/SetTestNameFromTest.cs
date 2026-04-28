using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetTestNameFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.SetTestName("Lorem Ipsum");
            }
        }
    }
}



