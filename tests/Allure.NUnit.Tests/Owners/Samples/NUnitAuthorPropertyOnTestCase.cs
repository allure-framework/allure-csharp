using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.NUnitAuthorPropertyOnTestCase
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase(1, Author = "John Doe")]
        public void TestMethod(int _) { }
    }
}
