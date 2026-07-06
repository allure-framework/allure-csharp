using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Tags.TagAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureTag("foo", "bar")]
        [AllureTag("baz")]
        public void TestMethod() { }
    }

    [AllureTag("foo", "bar")]
    [AllureTag("baz")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureTag("foo", "bar")]
    [AllureTag("baz")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureTag("foo", "bar")]
    [AllureTag("baz")]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
