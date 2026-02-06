using Xunit;

namespace Allure.Xunit.Tests.Samples.LegacyNameAttribute
{
    public class TestsClass
    {
        [Theory]
        [InlineData(1)]
        public void TestMethod(int _) { }
    }
}
