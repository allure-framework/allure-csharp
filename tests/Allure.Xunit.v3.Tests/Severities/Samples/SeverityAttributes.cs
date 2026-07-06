using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Severities.SeverityAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureSeverity(SeverityLevel.critical)]
        public void TestMethod() { }
    }

    [AllureSeverity(SeverityLevel.critical)]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSeverity(SeverityLevel.critical)]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSeverity(SeverityLevel.critical)]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
