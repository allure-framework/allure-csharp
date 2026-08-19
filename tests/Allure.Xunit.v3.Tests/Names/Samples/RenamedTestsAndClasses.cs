using Allure;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.RenamedTestsAndClasses
{
    [AllureName("Lorem Ipsum on NamedClass")]
    public class NamedClass
    {
        [Fact]
        public void TestMethod() { }
    }

    public class TestClass
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
