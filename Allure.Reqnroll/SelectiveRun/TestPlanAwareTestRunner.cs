using System;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.TestPlan;
using Allure.ReqnrollPlugin.Functions;
using Allure.ReqnrollPlugin.State;
using Reqnroll;
using Reqnroll.UnitTestProvider;

namespace Allure.ReqnrollPlugin.SelectiveRun;

class TestPlanAwareTestRunner : ITestRunner
{
    const string TESTPLAN_DESELECTION_CACHE_KEY
        = "DESELECTED_BY_ALLURE_TESTPLAN";

    static AllureTestPlan TestPlan =>
        AllureLifecycle.Instance.TestPlan;

    readonly IUnitTestRuntimeProvider unitTestRuntimeApi;
    readonly ITestRunnerManager runnerManager;
    readonly ITestRunner underlyingRunner;

    public string TestWorkerId => this.underlyingRunner.TestWorkerId;

    public FeatureContext FeatureContext =>
        this.underlyingRunner.FeatureContext;

    public ScenarioContext ScenarioContext =>
        this.underlyingRunner.ScenarioContext;

    public ITestThreadContext TestThreadContext =>
        this.underlyingRunner.TestThreadContext;


    public TestPlanAwareTestRunner(
        IUnitTestRuntimeProvider unitTestRuntimeApi,
        ITestRunnerManager runnerManager,
        ITestRunner underlyingRunner
    )
    {
        this.unitTestRuntimeApi = unitTestRuntimeApi;
        this.runnerManager = runnerManager;
        this.underlyingRunner = underlyingRunner;
    }

    public void InitializeTestRunner(string testWorkerId) =>
        this.underlyingRunner.InitializeTestRunner(testWorkerId);

    public async Task OnTestRunStartAsync() =>
        await this.underlyingRunner.OnTestRunStartAsync();

    public async Task OnTestRunEndAsync() =>
        await this.underlyingRunner.OnTestRunEndAsync();

    public async Task OnFeatureStartAsync(FeatureInfo featureInfo) =>
        await this.underlyingRunner.OnFeatureStartAsync(featureInfo);

    public async Task OnFeatureEndAsync() =>
        await this.underlyingRunner.OnFeatureEndAsync();

    public void OnScenarioInitialize(ScenarioInfo scenarioInfo)
    {
        this.underlyingRunner.OnScenarioInitialize(scenarioInfo);
        this.ApplyTestPlanToCurrentScenario();
    }

    public async Task OnScenarioStartAsync()
    {
        if (this.IsCurrentScenarioSelected)
        {
            await this.underlyingRunner.OnScenarioStartAsync();
        }
        else
        {
            // This call will set the scenario's status to skipped.
            this.SkipScenario();

            // We're skipping scenarios not in the test plan using the unit
            // test framework's runtime API. Neither BeforeScenario nor
            // AfterScenario hooks are executed because we've skipped
            // OnScenarioStartAsync and called SkipScenario. AllureContext of
            // the scenario will be discarded after ScenarioFinishedEvent.
            this.unitTestRuntimeApi.TestIgnore(AllureTestPlan.SkipReason);
        }
    }

    public async Task CollectScenarioErrorsAsync() =>
        await this.underlyingRunner.CollectScenarioErrorsAsync();

    public async Task OnScenarioEndAsync() =>
        await this.underlyingRunner.OnScenarioEndAsync();

    public void SkipScenario() => this.underlyingRunner.SkipScenario();

    public async Task GivenAsync(
        string text,
        string multilineTextArg,
        Table tableArg,
        string? keyword = null
    ) =>
        await this.CallStepOfSelectedScenario(
            this.underlyingRunner.GivenAsync,
            text,
            multilineTextArg,
            tableArg,
            keyword
        );

    public async Task WhenAsync(
        string text,
        string multilineTextArg,
        Table tableArg,
        string? keyword = null
    ) =>
        await this.CallStepOfSelectedScenario(
            this.underlyingRunner.WhenAsync,
            text,
            multilineTextArg,
            tableArg,
            keyword
        );

    public async Task ThenAsync(
        string text,
        string multilineTextArg,
        Table tableArg,
        string? keyword = null
    ) =>
        await this.CallStepOfSelectedScenario(
            this.underlyingRunner.ThenAsync,
            text,
            multilineTextArg,
            tableArg,
            keyword
        );

    public async Task AndAsync(
        string text,
        string multilineTextArg,
        Table tableArg,
        string? keyword = null
    ) =>
        await this.CallStepOfSelectedScenario(
            this.underlyingRunner.AndAsync,
            text,
            multilineTextArg,
            tableArg,
            keyword
        );

    public async Task ButAsync(
        string text,
        string multilineTextArg,
        Table tableArg,
        string? keyword = null
    ) =>
        await this.CallStepOfSelectedScenario(
            this.underlyingRunner.ButAsync,
            text,
            multilineTextArg,
            tableArg,
            keyword
        );

    public void Pending() => this.underlyingRunner.Pending();
    
    internal static bool IsScenarioSelected(ScenarioContext? scenarioContext) =>
        scenarioContext?.ContainsKey(TESTPLAN_DESELECTION_CACHE_KEY) is false;

    bool IsCurrentScenarioSelected =>
        IsScenarioSelected(this.ScenarioContext);

    void ApplyTestPlanToCurrentScenario()
    {
        var fullName = MappingFunctions.CreateFullName(
            this.runnerManager.TestAssembly,
            this.FeatureContext.FeatureInfo,
            this.ScenarioContext.ScenarioInfo.Title
        );
        var allureId = AllureReqnrollStateFacade.GetAllureId(
            this.FeatureContext.FeatureInfo,
            this.ScenarioContext
        );
        if (!TestPlan.IsSelected(fullName, allureId))
        {
            this.ScenarioContext.Set(true, TESTPLAN_DESELECTION_CACHE_KEY);
        }
    }

    async Task CallStepOfSelectedScenario(
        Func<string, string, Table, string?, Task> stepFn,
        string text,
        string multilineTextArg,
        Table tableArg,
        string? keyword
    )
    {
        if (this.IsCurrentScenarioSelected)
        {
            await stepFn(text, multilineTextArg, tableArg, keyword);
        }
    }
}
