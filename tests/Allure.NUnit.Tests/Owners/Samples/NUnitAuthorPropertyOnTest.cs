using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.NUnitAuthorPropertyOnTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test(Author = "John Doe")]
        public void TestMethod() { }
    }
}
