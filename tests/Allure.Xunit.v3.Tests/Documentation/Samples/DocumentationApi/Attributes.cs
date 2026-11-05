using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Documentation.DocumentationApi
{
    [AllureDescription("Interface description", Append = true)]
    [AllureDescriptionHtml("<p>Interface HTML</p>", Append = true)]
    [AllurePreconditions("Interface preconditions", Append = true)]
    [AllurePreconditionsHtml("<p>Interface preconditions HTML</p>", Append = true)]
    [AllureExpectedResults("Interface expected results", Append = true)]
    [AllureExpectedResultsHtml("<p>Interface expected results HTML</p>", Append = true)]
    public interface IInterface { }

    [AllureDescription("Base class description", Append = true)]
    [AllureDescriptionHtml("<p>Base class HTML</p>", Append = true)]
    [AllurePreconditions("Base class preconditions", Append = true)]
    [AllurePreconditionsHtml("<p>Base class preconditions HTML</p>", Append = true)]
    [AllureExpectedResults("Base class expected results", Append = true)]
    [AllureExpectedResultsHtml("<p>Base class expected results HTML</p>", Append = true)]
    public class BaseClass { }

    [AllureDescription("Test class description", Append = true)]
    [AllureDescriptionHtml("<p>Test class HTML</p>", Append = true)]
    [AllurePreconditions("Test class preconditions", Append = true)]
    [AllurePreconditionsHtml("<p>Test class preconditions HTML</p>", Append = true)]
    [AllureExpectedResults("Test class expected results", Append = true)]
    [AllureExpectedResultsHtml("<p>Test class expected results HTML</p>", Append = true)]
    public class AttributeTestClass : BaseClass, IInterface
    {
        [Fact]
        [AllureDescription("Test method description", Append = true)]
        [AllureDescriptionHtml("<p>Test method HTML</p>", Append = true)]
        [AllurePreconditions("Test method preconditions", Append = true)]
        [AllurePreconditionsHtml("<p>Test method preconditions HTML</p>", Append = true)]
        [AllureExpectedResults("Test method expected results", Append = true)]
        [AllureExpectedResultsHtml("<p>Test method expected results HTML</p>", Append = true)]
        public void TestMethod() { }
    }
}
