using Allure.Net.Commons;
using Xunit;

namespace Allure.Xunit.Tests.Samples.AddEpicFromTest
{
    public class TestsClass
    {
        [Fact]
        public void TestMethod()
        {
            AllureApi.AddEpic("foo");
        }
    }
}
