using Allure.Commons;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin
{
    [Binding]
    public class AllureBindings
    {
        private FeatureContext featureContext;
        private ScenarioContext scenarioContext;

        public AllureBindings(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [BeforeFeature(Order = int.MinValue)]
        public static void FirstBeforeFeature()
        {
            // start feature container in BindingInvoker
        }

        [AfterFeature(Order = int.MaxValue)]
        public static void LastAfterFeature()
        {
            // write feature container in BindingInvoker
        }

        [BeforeScenario(Order = int.MaxValue)]
        public void LastBeforeScenario()
        {
            // start scenario
            AllureLifecycle.Instance.StartTestCase(
                Allure.FeatureContainerId(featureContext),
                Allure.GetTestResult(featureContext?.FeatureInfo, scenarioContext?.ScenarioInfo)
            );
        }

        [AfterScenario(Order = int.MinValue)]
        public void FirstAfterScenario()
        {
            // update status if empty and stop scenario
            AllureLifecycle.Instance
                .UpdateTestCase(Allure.ScenarioId(scenarioContext?.ScenarioInfo),
                    x=>
                    {
                        x.status = (x.status == Status.none) ? Status.passed : x.status;
                    })
                .StopTestCase(Allure.ScenarioId(scenarioContext?.ScenarioInfo));
        }

        [AfterScenario(Order = int.MaxValue)]
        public void LastAfterScenario()
        {
            // write scenario
            AllureLifecycle.Instance
                .WriteTestCase(Allure.ScenarioId(scenarioContext?.ScenarioInfo));
        }
    }
}
