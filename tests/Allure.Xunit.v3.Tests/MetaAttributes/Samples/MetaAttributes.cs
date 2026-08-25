using Allure;
using Allure.Model;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.MetaAttributes.MetaAttributes
{
    [AllureEpic("Foo")]
    [AllureOwner("John Doe")]
    [AllureFeature("Bar")]
    [AllureTag("foo", "bar")]
    [AllureStory("Baz")]
    [AllureSeverity(Severity.Critical)]
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
