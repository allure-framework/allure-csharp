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

        [BeforeScenario(Order = int.MinValue)]
        public void FirstBeforeScenario()
        {
            // start scenario container as child of feature container
            AllureLifecycle.Instance.StartTestContainer(
                Allure.FeatureContainerId(featureContext),
                new TestResultContainer()
                {
                    uuid = Allure.ScenarioContainerId(scenarioContext)
                });

            
        }
        [BeforeScenario(Order = int.MaxValue)]
        public void LastBeforeScenario()
        {
            // start scenario
            AllureLifecycle.Instance.StartTestCase(
                Allure.ScenarioContainerId(scenarioContext),
                Allure.GetTestResult(featureContext, scenarioContext)
            );
        }

        [AfterScenario(Order = int.MinValue)]
        public void FirstAfterScenario()
        {
            // update status if empty and stop scenario
            AllureLifecycle.Instance
                .UpdateTestCase(Allure.ScenarioId(scenarioContext),
                    x=>
                    {
                        x.status = (x.status == Status.none) ? Status.passed : x.status;
                    })
                .StopTestCase(Allure.ScenarioId(scenarioContext));
        }

        [AfterScenario(Order = int.MaxValue)]
        public void LastAfterScenario()
        {
            // write scenario
            // stop and write scenario container
            AllureLifecycle.Instance
                .WriteTestCase(Allure.ScenarioId(scenarioContext))
                .StopTestContainer(Allure.ScenarioContainerId(scenarioContext))
                .WriteTestContainer(Allure.ScenarioContainerId(scenarioContext));
        }
    }
}
