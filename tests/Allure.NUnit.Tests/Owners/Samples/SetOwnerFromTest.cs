using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.SetOwnerFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.SetOwner("John Doe");
        }
    }
}
