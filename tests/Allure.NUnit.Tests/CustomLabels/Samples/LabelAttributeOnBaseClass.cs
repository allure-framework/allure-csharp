using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.LabelAttributeOnBaseClass
{
    [AllureLabel("foo", "bar")]
    public class BaseClass {}

    [AllureNUnit]
    public class TestsClass : BaseClass
    {
        [Test]
        public void TestMethod() { }
    }
}
