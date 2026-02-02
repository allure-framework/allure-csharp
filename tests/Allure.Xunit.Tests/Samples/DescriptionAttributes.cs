using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Samples.DescriptionAttributes
{
    [AllureDescription("Lorem Ipsum")]
    [AllureDescriptionHtml("<p>Dolor Sit Amet</p>")]
    public interface IMetadataInterface { }

    [AllureDescription("Consectetur Adipiscing Elit", Append = true)]
    [AllureDescriptionHtml("<p>Sed Do Eiusmod</p>", Append = true)]
    public class TestClassBase { }

    [AllureDescription("Tempor Incididunt", Append = true)]
    [AllureDescriptionHtml("<p>Ut Labore</p>", Append = true)]
    public class TestsClass : TestClassBase, IMetadataInterface
    {
        [Fact]
        [AllureDescription("Et Dolore", Append = true)]
        [AllureDescriptionHtml("<p>Magna Aliqua</p>", Append = true)]
        public void TestMethod() { }
    }
}
