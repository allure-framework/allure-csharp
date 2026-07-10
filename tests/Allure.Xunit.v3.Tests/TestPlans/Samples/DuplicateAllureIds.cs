using Allure.Net.Commons.Attributes;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.TestPlans.DuplicateAllureIds
{
    public class TestClass
    {
        [Fact]
        [AllureId(3004)]
        public void FirstSelectedTest() { }

        [Fact]
        [AllureId(3004)]
        public void SecondSelectedTest() { }

        [Fact]
        [AllureId(3999)]
        public void UnselectedTest() { }
    }
}
