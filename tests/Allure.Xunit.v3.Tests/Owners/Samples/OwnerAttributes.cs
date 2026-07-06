using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Owners.OwnerAttributes
{
    public class OnTestMethod
    {
        [Fact]
        [AllureOwner("John Doe")]
        public void TestMethod() { }
    }

    [AllureOwner("John Doe")]
    public class OnTestClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureOwner("John Doe")]
    public class BaseClass { }

    public class OnBaseClass : BaseClass
    {
        [Fact]
        public void TestMethod() { }
    }

    [AllureOwner("John Doe")]
    public interface IInterface { }

    public class OnInterface : IInterface
    {
        [Fact]
        public void TestMethod() { }
    }
}
