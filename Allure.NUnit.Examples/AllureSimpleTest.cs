using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Simple")]
    public class AllureSimpleTest : BaseTest
    {
        [Test(Author = "Author", Description = "Simple test")]
        [Category("Category")]
        [AllureSeverity(SeverityLevel.minor)]
        public void NUnitTagsTest()
        {
        }

        [Test]
        [Description("Simple test with allure tags")]
        [AllureOwner("Author")]
        [AllureTag("Category")]
        [AllureSeverity(SeverityLevel.critical)]
        public void AllureTagsTest()
        {
        }
    }
}