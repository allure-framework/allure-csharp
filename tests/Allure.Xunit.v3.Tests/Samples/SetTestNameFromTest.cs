using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetTestNameFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetTestName("Lorem Ipsum");
        }
    }
}



