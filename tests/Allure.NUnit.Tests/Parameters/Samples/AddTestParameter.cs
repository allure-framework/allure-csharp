using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Parameters.Samples.AddTestParameters
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddTestParameter("name1", "value-1");
        }

        [TestCase]
        public void TestMethod(

        )
        {
            AllureApi.AddTestParameter("name2", "value-2", ParameterMode.Masked);
            AllureApi.AddTestParameter("name3", "value-3", ParameterMode.Hidden);
            AllureApi.AddTestParameter("name4", "value-4", excluded: true);
        }

        [TearDown]
        public void TearDown()
        {
            AllureApi.AddTestParameter("name5", "value-5", ParameterMode.Masked, true);
        }
    }
}
