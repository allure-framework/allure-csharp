using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class AllureLabelTest
    {
        [TearDown]
        public void SetUp()
        {
            AllureApi.AddLabel("foo", "bar");
        }

        [Test]
        public void RunTest() { }
    }
}
