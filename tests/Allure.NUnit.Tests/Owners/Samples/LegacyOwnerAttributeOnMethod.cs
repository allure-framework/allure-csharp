using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Owners.Samples.LegacyOwnerAttributeOnMethod
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        [AllureOwner("John Doe")]
        public void TestMethod() { }
    }
}
