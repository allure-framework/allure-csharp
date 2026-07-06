using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SubSuiteAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureSubSuite("Sub Suite")]
        public void TestMethod() { }
    }

    [AllureSubSuite("Sub Suite")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSubSuite("Sub Suite")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSubSuite("Sub Suite")]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
