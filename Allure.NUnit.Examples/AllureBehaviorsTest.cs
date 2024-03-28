using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Behaviors")]
    [AllureEpic("Epic")]
    public class AllureBehaviorsTest : BaseTest
    {
        [Test]
        [AllureFeature("f1")]
        public void F1()
        {
            Assert.That(true);
        }

        [Test]
        [AllureFeature("f2")]
        [AllureStory("A")]
        public void F2()
        {
            Assert.That(true);
        }
    }
}