using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddFeatureFromTest
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public void TestMethod()
        {
            AllureApi.AddFeature("foo");
        }
    }
}

