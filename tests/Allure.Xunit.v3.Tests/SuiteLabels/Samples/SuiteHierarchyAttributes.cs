using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.SuiteLabels.SuiteHierarchyAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureSuiteHierarchy("Parent Suite", "Suite", "Sub Suite")]
        public void TestMethod() { }
    }

    [AllureSuiteHierarchy("Parent Suite", "Suite", "Sub Suite")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSuiteHierarchy("Parent Suite", "Suite", "Sub Suite")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSuiteHierarchy("Parent Suite", "Suite", "Sub Suite")]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
