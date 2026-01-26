using Allure.Net.Commons;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.AddFeatureFromSetUp
{
    [AllureNUnit]
    public class TestsClass
    {
        [SetUp]
        public void SetUp()
        {
            AllureApi.AddFeature("foo");
        }

        [Test]
        public void TestMethod() { }
    }
}

