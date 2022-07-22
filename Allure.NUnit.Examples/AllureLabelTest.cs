using Allure.Net.Commons;
using NUnit.Allure.Attributes;
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
            AllureLifecycle.Instance.UpdateTestCase(t => t.labels.Add(Label.Epic("dynamic epic")));
            AllureLifecycle.Instance.UpdateTestCase(t => t.labels.Add(Label.Feature("dynamic feature")));
            AllureLifecycle.Instance.UpdateTestCase(t => t.labels.Add(Label.Story("dynamic story")));
            AllureLifecycle.Instance.UpdateTestCase(t => t.labels.Add(Label.Owner("dynamic owner")));
        }

        [Test]
        [AllureLabel("custom", "static value")]
        public void CustomStaticLabelTest()
        {
        }

        [Test]
        public void CustomDynamicLabelTest()
        {
            AllureLifecycle.Instance.UpdateTestCase(t =>
                t.labels.Add(new Label {name = "custom", value = "dynamic value"}));
        }
    }
}