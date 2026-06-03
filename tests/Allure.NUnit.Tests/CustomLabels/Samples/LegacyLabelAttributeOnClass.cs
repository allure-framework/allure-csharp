using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.LegacyLabelAttributeOnClass
{
    [AllureNUnit]
    [AllureLabel("foo", "bar")]
    public class TestsClass
    {
        [Test]
        public void TestMethod() { }
    }
}
