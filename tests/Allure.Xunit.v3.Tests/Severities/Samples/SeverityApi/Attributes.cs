using Allure;
using Allure.Model;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Severities.SeverityApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureSeverity(Severity.Critical)]
        public void TestMethod() { }
    }

    [AllureSeverity(Severity.Critical)]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSeverity(Severity.Critical)]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureSeverity(Severity.Critical)]
    public interface IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
