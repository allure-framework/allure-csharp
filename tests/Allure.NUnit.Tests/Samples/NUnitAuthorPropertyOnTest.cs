using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.NUnitAuthorPropertyOnTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test(Author = "John Doe")]
        public void TestMethod() { }
    }
}
