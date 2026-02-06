using Allure.NUnit;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyFixtureAttributes
{
    [AllureNUnit]
    public class TestsClass
    {
        [OneTimeSetUp]
        [AllureBefore]
        public static void OneTimeSetUp() { }

        [SetUp]
        [AllureBefore("Foo")]
        public void SetUp() { }

        [Test]
        public void TestMethod() { }

        [TearDown]
        [AllureAfter]
        public void TearDown() { }

        [OneTimeTearDown]
        [AllureAfter("Bar")]
        public static void OneTimeTearDown() { }
    }
}
