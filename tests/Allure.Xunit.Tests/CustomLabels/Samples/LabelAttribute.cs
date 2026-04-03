using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.CustomLabels.Samples.LabelAttribute
{
    [AllureLabel("interface", "foo")]
    public interface IMetadataInterface {}

    [AllureLabel("baseClass", "bar")]
    public class BaseClass {}

    [AllureLabel("class", "baz")]
    public class TestsClass : BaseClass, IMetadataInterface
    {
        [Fact]
        [AllureLabel("method", "qux")]
        public void TestMethod() { }
    }
}
