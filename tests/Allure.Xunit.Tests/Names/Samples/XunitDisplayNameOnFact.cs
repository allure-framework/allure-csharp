using Allure.Xunit.Attributes;
using Xunit;

namespace Allure.Xunit.Tests.Names.Samples.XunitDisplayNameOnFact
{
    public class TestsClass
    {
        [Fact(DisplayName = "Lorem Ipsum")]
        public void TestMethod() { }
    }
}
