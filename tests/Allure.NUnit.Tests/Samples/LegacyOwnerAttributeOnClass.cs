using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyOwnerAttributeOnClass
{
    [AllureNUnit]
    [AllureOwner("John Doe")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
