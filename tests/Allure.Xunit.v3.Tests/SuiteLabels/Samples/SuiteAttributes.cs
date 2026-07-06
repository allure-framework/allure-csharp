using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureSuite("Suite")]
        public void TestMethod() { }
    }

    [AllureSuite("Suite")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSuite("Suite")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSuite("Suite")]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
