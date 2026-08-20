using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Tags.TagApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureTag("foo", "bar")]
        [AllureTag("baz")]
        public void TestMethod() { }
    }

    [AllureTag("foo", "bar")]
    [AllureTag("baz")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureTag("foo", "bar")]
    [AllureTag("baz")]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureTag("foo", "bar")]
    [AllureTag("baz")]
    public interface IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
