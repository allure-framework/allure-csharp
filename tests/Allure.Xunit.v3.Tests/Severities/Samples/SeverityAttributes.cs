using Allure;
using Allure.Model;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Severities.SeverityAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureSeverity(Severity.Critical)]
        public void TestMethod() { }
    }

    [AllureSeverity(Severity.Critical)]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSeverity(Severity.Critical)]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSeverity(Severity.Critical)]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
