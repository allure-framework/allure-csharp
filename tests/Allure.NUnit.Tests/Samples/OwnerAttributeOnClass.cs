using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.OwnerAttributeOnClass
{
    [AllureNUnit]
    [AllureOwner("John Doe")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
