using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Descriptions.DescriptionAttributes
{
    [AllureDescription("Interface description", Append = true)]
    [AllureDescriptionHtml("<p>Interface HTML</p>", Append = true)]
    public interface IInterface { }

    [AllureDescription("Base class description", Append = true)]
    [AllureDescriptionHtml("<p>Base class HTML</p>", Append = true)]
    public class BaseClass { }

    [AllureDescription("Test class description", Append = true)]
    [AllureDescriptionHtml("<p>Test class HTML</p>", Append = true)]
    public class TestClass : BaseClass, IInterface
    {
        [Fact]
        [AllureDescription("Test method description", Append = true)]
        [AllureDescriptionHtml("<p>Test method HTML</p>", Append = true)]
        public void TestMethod() { }
    }
}
