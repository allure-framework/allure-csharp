using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureParentSuite("Parent Suite")]
        public void TestMethod() { }
    }

    [AllureParentSuite("Parent Suite")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureParentSuite("Parent Suite")]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureParentSuite("Parent Suite")]
    public interface IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
