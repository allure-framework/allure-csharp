using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddFeatureFromTearDown
{
    [AllureNUnit]
    public class TestsClass
    {
        [TearDown]
        public void TearDown()
        {
            AllureApi.AddFeature("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}

