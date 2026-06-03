using Allure.Xunit.Attributes;
using Xunit;

#pragma warning disable xUnit1026

namespace Allure.Xunit.Tests.Names.Samples.XunitDisplayNameOnTheory
{
    public class TestsClass
    {
        [Theory(DisplayName = "Lorem Ipsum")]
        [InlineData(1)]
        public void TestMethod(int foo) { }
    }
}
