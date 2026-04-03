using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.SetOwnerFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.SetOwner("John Doe");
        }

        [Test]
        public void TestMethod() { }
    }
}
