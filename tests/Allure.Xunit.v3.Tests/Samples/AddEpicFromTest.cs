using Allure.Net.Commons;
using Allure.Xunit;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddEpicFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            using (AllureRuntimeScope.Begin())
            {
                AllureApi.AddEpic("foo");
            }
        }
    }
}



