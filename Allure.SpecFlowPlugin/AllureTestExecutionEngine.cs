using BoDi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Infrastructure;

namespace Allure.SpecFlowPlugin
{
    class AllureTestExecutionEngine : ITestExecutionEngine
    {
        private ITestExecutionEngine engine;
        public FeatureContext FeatureContext => engine.FeatureContext;

        public ScenarioContext ScenarioContext => engine.ScenarioContext;

        public AllureTestExecutionEngine(IObjectContainer container)
        {
            this.engine = container.Resolve<ITestExecutionEngine>();

            this.engine = container.Resolve<ITestExecutionEngine>("DefaultTestExecutionEngine");
        }
        public void OnAfterLastStep()
        {
            engine.OnAfterLastStep();
        }

        public void OnFeatureEnd()
        {
            engine.OnFeatureEnd();
        }

        public void OnFeatureStart(FeatureInfo featureInfo)
        {
            engine.OnFeatureStart(featureInfo);
        }

        public void OnScenarioEnd()
        {
            engine.OnScenarioEnd();
        }

        public void OnScenarioStart(ScenarioInfo scenarioInfo)
        {
            engine.OnScenarioStart(scenarioInfo);
        }

        public void OnTestRunEnd()
        {
            engine.OnTestRunEnd();
        }

        public void OnTestRunStart()
        {
            engine.OnTestRunStart();
        }

        public void Pending()
        {
            engine.Pending();
        }

        public void Step(StepDefinitionKeyword stepDefinitionKeyword, string keyword, string text, string multilineTextArg, Table tableArg)
        {
            engine.Step(stepDefinitionKeyword, keyword, text, multilineTextArg, tableArg);
        }
    }
}
