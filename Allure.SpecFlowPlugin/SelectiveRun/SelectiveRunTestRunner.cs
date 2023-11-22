using Allure.Net.Commons;
using Allure.Net.Commons.TestPlan;
using System;
using System.Threading;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin.SelectiveRun
{
    internal class SelectiveRunTestRunner : ITestRunner
    {
        const string TESTPLAN_DESELECTION_CACHE_KEY
            = "DESELECTED_BY_ALLURE_TESTPLAN";

        static AllureTestPlan TestPlan
        {
            get => AllureLifecycle.Instance.TestPlan;
        }

        static readonly AsyncLocal<SelectiveRunTestRunner> asyncLocalRunner
            = new();

        internal static SelectiveRunTestRunner CurrentRunner
        {
            get => asyncLocalRunner.Value;
            set => asyncLocalRunner.Value = value;
        }

        readonly ITestRunner underlyingRunner;

        public SelectiveRunTestRunner(ITestRunner underlyingRunner)
        {
            this.underlyingRunner = underlyingRunner;
        }

        public int ThreadId => this.underlyingRunner.ThreadId;

        public FeatureContext FeatureContext =>
            this.underlyingRunner.FeatureContext;

        public ScenarioContext ScenarioContext =>
            this.underlyingRunner.ScenarioContext;

        public void CollectScenarioErrors() =>
            this.underlyingRunner.CollectScenarioErrors();

        public void InitializeTestRunner(int threadId) =>
            this.underlyingRunner.InitializeTestRunner(threadId);

        public void OnFeatureEnd() =>
            this.underlyingRunner.OnFeatureEnd();

        public void OnFeatureStart(FeatureInfo featureInfo) =>
            this.underlyingRunner.OnFeatureStart(featureInfo);

        public void OnScenarioEnd() =>
            this.underlyingRunner.OnScenarioEnd();

        public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
        {
            this.underlyingRunner.OnScenarioInitialize(scenarioInfo);
            this.ApplyTestPlanToCurrentScenario();
            CurrentRunner = this;
        }

        public void OnScenarioStart()
        {
            if (this.IsCurrentScenarioSelected)
            {
                this.underlyingRunner.OnScenarioStart();
            }
            else
            {
                this.SkipScenario();
            }
        }

        public void OnTestRunEnd() =>
            this.underlyingRunner.OnTestRunEnd();

        public void OnTestRunStart() =>
            this.underlyingRunner.OnTestRunStart();

        public void Pending() =>
            this.underlyingRunner.Pending();

        public void SkipScenario() =>
            this.underlyingRunner.SkipScenario();

        public void And(
            string text,
            string multilineTextArg,
            Table tableArg,
            string? keyword = null
        ) =>
            this.CallStepOfSelectedScenario(
                this.underlyingRunner.And,
                text,
                multilineTextArg,
                tableArg,
                keyword
            );

        public void But(
            string text,
            string multilineTextArg,
            Table tableArg,
            string? keyword = null
        ) =>
            this.CallStepOfSelectedScenario(
                this.underlyingRunner.But,
                text,
                multilineTextArg,
                tableArg,
                keyword
            );

        public void Given(
            string text,
            string multilineTextArg,
            Table tableArg,
            string? keyword = null
        ) =>
            this.CallStepOfSelectedScenario(
                this.underlyingRunner.Given,
                text,
                multilineTextArg,
                tableArg,
                keyword
            );

        public void Then(
            string text,
            string multilineTextArg,
            Table tableArg,
            string? keyword = null
        ) =>
            this.CallStepOfSelectedScenario(
                this.underlyingRunner.Then,
                text,
                multilineTextArg,
                tableArg,
                keyword
            );

        public void When(
            string text,
            string multilineTextArg,
            Table tableArg,
            string? keyword = null
        ) =>
            this.CallStepOfSelectedScenario(
                this.underlyingRunner.When,
                text,
                multilineTextArg,
                tableArg,
                keyword
            );

        internal bool IsCurrentScenarioSelected
        {
            get => !this.ScenarioContext.ContainsKey(
                TESTPLAN_DESELECTION_CACHE_KEY)
            ;
        }

        void ApplyTestPlanToCurrentScenario()
        {
            var (labels, _) = PluginHelper.GetOrParseLabelsAndLinks(
                this.FeatureContext.FeatureInfo,
                this.ScenarioContext
            );
            var fullName = this.ScenarioContext.ScenarioInfo.Title;
            var allureId = AllureTestPlan.GetAllureId(labels);
            if (!TestPlan.IsSelected(fullName, allureId))
            {
                this.ScenarioContext.Set(true, TESTPLAN_DESELECTION_CACHE_KEY);
            }
        }

        void CallStepOfSelectedScenario(
            Action<string, string, Table, string?> stepFn,
            string text,
            string multilineTextArg,
            Table tableArg,
            string? keyword
        )
        {
            if (this.IsCurrentScenarioSelected)
            {
                stepFn(text, multilineTextArg, tableArg, keyword);
            }
        }
    }
}
