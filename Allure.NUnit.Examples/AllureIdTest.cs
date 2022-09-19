using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Id")]
    public class AllureIdTest : BaseTest
    {
        [Test]
        [AllureId(345)]
        public void StaticIdTest()
        {
        }
    }
}