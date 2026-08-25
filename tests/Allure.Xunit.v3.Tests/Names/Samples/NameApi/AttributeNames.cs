using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.NameApi
{
    [AllureName("Lorem Ipsum on NamedClass")]
    public class AttributeNamedClass
    {
        [Fact]
        public void TestMethod() { }
    }

    public class AttributeTestClass
    {
        [Fact]
        [AllureName("Lorem Ipsum on FactMethodRenamedInAllure")]
        public void FactMethodRenamedInAllure() { }

        [Theory]
        [InlineData("foo")]
        [AllureName("Lorem Ipsum on TheoryMethodRenamedInAllure")]
        public void TheoryMethodRenamedInAllure(string value)
        {
            _ = value;
        }

        [Fact(DisplayName = "Lorem Ipsum on FactMethodRenamedInXunit")]
        public void FactMethodRenamedInXunit() { }

        [Theory(DisplayName = "Lorem Ipsum on TheoryMethodRenamedInXunit")]
        [InlineData("foo")]
        public void TheoryMethodRenamedInXunit(string value)
        {
            _ = value;
        }
    }
}
