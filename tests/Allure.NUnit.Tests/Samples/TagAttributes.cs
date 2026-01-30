using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.TagAttributes
{
    [AllureTag("foo")]
    public interface IMetadata { }

    [AllureTag("bar")]
    public class BaseClass { }

    [AllureNUnit]
    [AllureTag("baz")]
    public class TestsClass : BaseClass, IMetadata
    {
        [Test]
        [AllureTag("qux", "qut")]
        public void TestMethod() { }
    }
}
