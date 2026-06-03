using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.NUnitAuthorPropertyOnTestFixture
{
    [AllureNUnit]
    [TestFixture(Author = "John Doe")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
