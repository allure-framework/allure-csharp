using Allure.Xunit;
using Xunit;

[assembly: AllureXunit]

namespace Allure.Xunit.v3.Tests.Samples.Generator.ManualAssemblyAttribute
{
    public class TestClass
    {
        [Fact]
        public void FirstTest() { }

        [Fact]
        public void SecondTest() { }
    }
}
