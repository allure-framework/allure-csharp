using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitAuthorPropertyOnTestCase
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase(1, Author = "John Doe")]
        public void TestMethod(int _) { }
    }
}
