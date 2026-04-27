using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.SetAllureIdFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.SetAllureId(1001);
        }
    }
}



