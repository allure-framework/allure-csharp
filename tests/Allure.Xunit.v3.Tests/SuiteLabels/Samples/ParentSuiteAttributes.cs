using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.ParentSuiteAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureParentSuite("Parent Suite")]
        public void TestMethod() { }
    }

    [AllureParentSuite("Parent Suite")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureParentSuite("Parent Suite")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureParentSuite("Parent Suite")]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
