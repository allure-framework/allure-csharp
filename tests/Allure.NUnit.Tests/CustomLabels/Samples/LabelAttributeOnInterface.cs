using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.CustomLabels.Samples.LabelAttributeOnInterface
{
    [AllureLabel("foo", "bar")]
    public interface IMetadataInterface {}

    [AllureNUnit]
    public class TestsClass : IMetadataInterface
    {
        [Test]
        public void TestMethod() { }
    }
}
