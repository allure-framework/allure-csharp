using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class AllureLabelTest
    {
        [Test]
        public void RunTest()
        {
            AllureApi.AddLabel("foo", "bar");
        }
    }
}
