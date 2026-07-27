using System;
using Allure.Model;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

class RuntimeBoundLifecycleApi(
    IReadOnlyLateBoundReference<IAllureRuntime> runtimeReference
) :
    IAllureLifecycleApi
{
    AllureExecutionState CurrentState => runtimeReference.Value.ContextApi.CurrentState;

    IAllureModelApi ModelApi => runtimeReference.Value.ModelApi;

    IAllureRuntimeContext ContextApi => runtimeReference.Value.ContextApi;

    public void ScheduleTest(TestResult testResult)
    {
        var uuid = testResult.Uuid;
        testResult.Stage = Stage.Scheduled;
        this.ModelApi.UpdateAllScopes((scope) => scope.Children.Add(uuid));
        this.ContextApi.Update((state) => state.SetTestResult(testResult));
    }

    public void StartAfterFixture(FixtureResult fixtureResult) =>
        this.StartFixture(fixtureResult, (scope) => scope.Afters.Add(fixtureResult));

    public void StartBeforeFixture(FixtureResult fixtureResult) =>
        this.StartFixture(fixtureResult, (scope) => scope.Befores.Add(fixtureResult));

    public void StartScope(TestResultScope scope)
    {
        this.ContextApi.Update((state) => state.PushScope(scope));
    }

    public void StartStep(StepResult stepResult)
    {
        this.ModelApi.UpdateCurrentExecutableItem(
            (parent) => parent.Steps.Add(stepResult)
        );
        this.ContextApi.Update((state) => state.PushStepResult(stepResult));
        this.ModelApi.UpdateStepResult(startExecutableItem);
    }

    public void StartTest()
    {
        this.ModelApi.UpdateTestResult(startExecutableItem);
    }

    public void StartTest(TestResult testResult)
    {
        this.ScheduleTest(testResult);
        this.ModelApi.UpdateTestResult(startExecutableItem);
    }

    public FixtureResult StopFixture()
    {
        this.ModelApi.UpdateFixtureResult(stopExecutableItem);

        var fixtureResult = this.CurrentState.CurrentFixture;
        this.ContextApi.Update(static (state) => state.ClearFixtureResult());
        return fixtureResult;
    }

    public TestResultScope StopScope()
    {
        var scope = this.CurrentState.CurrentScope;
        this.ContextApi.Update(static (state) => state.PopScope());
        return scope;
    }

    public StepResult StopStep()
    {
        this.ModelApi.UpdateStepResult(stopExecutableItem);

        var stepResult = this.CurrentState.CurrentStep;
        this.ContextApi.Update(static (state) => state.PopStepResult());
        return stepResult;
    }

    public TestResult StopTest()
    {
        this.ModelApi.UpdateTestResult(stopExecutableItem);

        var testResult = this.CurrentState.CurrentTest;
        this.ContextApi.Update((state) => state.ClearTestResult());
        return testResult;
    }

    void StartFixture(FixtureResult fixtureResult, Action<TestResultScope> addFixtureToScope)
    {
        this.ContextApi.Update((state) => state.SetFixtureResult(fixtureResult));
        this.ModelApi.UpdateFixtureResult(startExecutableItem);
        this.ModelApi.UpdateScope(addFixtureToScope);
    }

    static readonly Action<ExecutableItem> startExecutableItem =
        static item =>
        {
            item.Stage = Stage.Running;
            if (item.Start == default)
            {
                item.Start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        };

    static readonly Action<ExecutableItem> stopExecutableItem =
        static item =>
        {
            item.Stage = Stage.Finished;
            if (item.Stop == default)
            {
                item.Stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        };
}
