using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.SetOwnerFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.SetOwner("John Doe");
        }

        [Test]
        public void TestMethod() { }
    }
}
