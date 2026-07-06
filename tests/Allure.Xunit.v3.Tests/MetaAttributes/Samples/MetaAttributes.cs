using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.MetaAttributes.MetaAttributes
{
    [AllureEpic("Foo")]
    [AllureOwner("John Doe")]
    [AllureFeature("Bar")]
    [AllureTag("foo", "bar")]
    [AllureStory("Baz")]
    [AllureSeverity(SeverityLevel.critical)]
    [AllureSuite("Qux")]
    [AllureLink("https://foo.bar/")]
    public class CustomAllureAttribute : AllureMetaAttribute { }

    public class TestClass
    {
        [Fact]
        [CustomAllure]
        public void TestMethod() { }
    }
}
