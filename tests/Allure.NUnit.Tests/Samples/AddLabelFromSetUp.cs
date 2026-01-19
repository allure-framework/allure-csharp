using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class AllureLabelTest
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddLabel("foo", "bar");
        }

        [Test]
        public void RunTest() { }
    }
}
