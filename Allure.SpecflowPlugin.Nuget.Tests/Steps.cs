using Allure.Commons;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;

namespace SpecFlowAllureNuget.Tests
{
    [Binding]
    public class Steps
    {
        [StepDefinition("Allure folder shouldn not be empty")]
        public void CheckAllure()
        {
            Assert.IsTrue(Directory.GetFiles(AllureLifecycle.Instance.ResultsDirectory).Count() == 0);

        }
        [Given(@"I have entered (.*) into the calculator")]
        public void GivenIHaveEnteredIntoTheCalculator(int p0)
        {

        }

        [When(@"I press add")]
        public void WhenIPressAdd()
        {

        }

        [Then(@"the result should be (.*) on the screen")]
        public void ThenTheResultShouldBeOnTheScreen(int p0)
        {

        }
    }
}
