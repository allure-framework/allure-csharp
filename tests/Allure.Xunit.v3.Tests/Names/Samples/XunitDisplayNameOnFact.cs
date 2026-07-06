using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Names.XunitDisplayNameOnFact
{
    public class TestClass
    {
        [Fact(DisplayName = "Lorem Ipsum")]
        public void TestMethod() { }
    }
}
