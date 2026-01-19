using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class AllureLabelTest
    {
        [Test]
        [AllureLabel("foo", "bar")]
        public void RunTest() { }
    }
}
