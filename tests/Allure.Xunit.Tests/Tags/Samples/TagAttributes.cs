using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Tags.Samples.TagAttributes
{
    [AllureTag("foo")]
    public interface IMetadata { }

    [AllureTag("bar")]
    public class BaseClass { }

    [AllureTag("baz")]
    public class TestsClass : BaseClass, IMetadata
    {
        [Fact]
        [AllureTag("qux", "qut")]
        public void TestMethod() { }
    }
}
