using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyOwnerAttributeOnBaseClass
{
    [AllureOwner("John Doe")]
    public class BaseClass { }

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
