using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Owners.OwnerApi
{
    public class AttributeOnTestMethod
    {
        [Fact]
        [AllureOwner("John Doe")]
        public void TestMethod() { }
    }

    [AllureOwner("John Doe")]
    public class AttributeOnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureOwner("John Doe")]
    public class BaseClass { }

    public class AttributeOnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureOwner("John Doe")]
    public interface IInterface { }

    public class AttributeOnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
