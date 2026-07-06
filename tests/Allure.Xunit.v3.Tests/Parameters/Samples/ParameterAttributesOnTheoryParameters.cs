using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using Xunit;

#pragma warning disable xUnit1026

namespace Allure.Xunit.v3.Tests.Samples.Parameters.ParameterAttributesOnTheoryParameters
{
    public class TestClass
    {
        [Theory]
        [InlineData("value-1", "ignored", "value-2", "value-3", "value-4", "value-5")]
        public void TestMethod(
            string name1,
            [AllureParameter(Ignore = true)] string ignored,
            [AllureParameter(Name = "name2", Mode = ParameterMode.Masked)] string renamed1,
            [AllureParameter(Mode = ParameterMode.Hidden)] string name3,
            [AllureParameter(Excluded = true)] string name4,
            [AllureParameter(Name = "name5", Mode = ParameterMode.Masked, Excluded = true)] string renamed2
        )
        {
            _ = name1 = ignored = renamed1 = name3 = name4 = renamed2;
        }
    }
}
