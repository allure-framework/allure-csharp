using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureSubSuite("Sub Suite")]
        public void TestMethod() { }
    }

    [AllureSubSuite("Sub Suite")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSubSuite("Sub Suite")]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSubSuite("Sub Suite")]
    public interface IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
