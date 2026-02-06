using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitAuthorPropertyOnTestFixture
{
    [AllureNUnit]
    [TestFixture(Author = "John Doe")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
