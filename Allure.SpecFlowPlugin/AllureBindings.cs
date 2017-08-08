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
        static AllureLifecycle allure = AllureLifecycle.Instance;

        private FeatureContext featureContext;
        private ScenarioContext scenarioContext;

        string featureContainerId => AllureHelper.GetFeatureContainerId(featureContext?.FeatureInfo);
        string scenarioContainerId => featureContext.Get<TestResultContainer>()?.uuid;
        string scenarioId => featureContext.Get<TestResult>()?.uuid;


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
            var scenarioContainer = new TestResultContainer()
            {
                uuid = AllureHelper.GetScenarioContainerId(featureContext?.FeatureInfo, scenarioContext?.ScenarioInfo)
            };
            allure.StartTestContainer(featureContainerId, scenarioContainer);
            featureContext.Set(scenarioContainer);
            featureContext.Get<HashSet<TestResultContainer>>().Add(scenarioContainer);

            var scenario = AllureHelper.GetTestResult(featureContext?.FeatureInfo, scenarioContext?.ScenarioInfo);
            allure.StartTestCase(scenarioContainerId, scenario);
            featureContext.Set(scenario);
            featureContext.Get<HashSet<TestResult>>().Add(scenario);
        }

        [AfterScenario(Order = int.MinValue)]
        public void FirstAfterScenario()
        {
            // update status to passed if there were no step of binding failures
            allure
                .UpdateTestCase(scenarioId,
                    x => x.status = (x.status != Status.none) ? x.status : Status.passed)
                .StopTestCase(scenarioId);
        }
    }
}
