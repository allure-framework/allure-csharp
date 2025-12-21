using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Labels")]
    public class AllureLabelTest : BaseTest
    {
        [Test]
        [AllureEpic("static epic")]
        [AllureFeature("static feature")]
        [AllureStory("static story")]
        [AllureOwner("static owner")]
        public void CommonStaticLabelTest()
        {
        }

        [Test]
        public void CommonDynamicLabelTest()
        {
            AllureApi.AddEpic("dynamic epic");
            AllureApi.AddFeature("dynamic feature");
            AllureApi.AddStory("dynamic story");
            AllureApi.SetOwner("dynamic owner");
        }

        [Test]
        [AllureLabel("custom", "static value")]
        public void CustomStaticLabelTest()
        {
        }

        [Test]
        public void CustomDynamicLabelTest()
        {
            AllureApi.AddLabel("custom", "dynamic value");
        }
    }
}