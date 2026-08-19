using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureSuite("Suite")]
        public void TestMethod() { }
    }

    [AllureSuite("Suite")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSuite("Suite")]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSuite("Suite")]
    public interface IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
