using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - ScreenDiff")]
    public class AllureDiffTest : BaseTest
    {
        [Test]
        [AllureName("Simple test with diff")]
        public void DiffTest()
        {
            AddDiffs();
        }

        [Test]
        [AllureName("Diff test with step")]
        public void DiffStepsTest()
        {
            AddDiffs();
            Net.Commons.Allure.Step("StepOutSide", () =>
            {
                Net.Commons.Allure.Step("Step Inside", () => { AddDiffs(); });
                AddDiffs();
            });
        }

        public static void AddDiffs()
        {
            Net.Commons.Allure.AddScreenDiff("Allure-Color.png", "Allure-Color.png", "Allure-Color.png");
        }
    }
}