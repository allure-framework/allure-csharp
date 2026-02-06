using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.ParameterAttributesOnTestCase
{
    [AllureNUnit]
    public class TestsClass
    {
        [TestCase("value-1", "ignored", "value-2", "value-3", "value-4", "value-5")]
        public void TestMethod(
            string name1,
            [AllureParameter(Ignore = true)] string ignored,
            [AllureParameter(Name = "name2", Mode = ParameterMode.Masked)] string renamed1,
            [AllureParameter(Mode = ParameterMode.Hidden)] string name3,
            [AllureParameter(Excluded = true)] string name4,
            [AllureParameter(Name = "name5", Mode = ParameterMode.Masked, Excluded = true)] string renamed2
        ) { }
    }
}
