using System;
using System.Collections.Immutable;
using Allure.Model;

namespace Allure.Sdk.Runtime;

public interface IAllureModelApi
{
    void UpdateScope(Action<TestResultScope> update);

    void UpdateScope(int level, Action<TestResultScope> update);

    TResult ReadScope<TResult>(Func<TestResultScope, TResult> read);

    TResult ReadScope<TResult>(int level, Func<TestResultScope, TResult> read);

    void UpdateAllScopes(Action<TestResultScope> update);

    ImmutableArray<TResult> ReadAllScopes<TResult>(Func<TestResultScope, TResult> read);

    void UpdateFixtureResult(Action<FixtureResult> update);

    TResult ReadFixtureResult<TResult>(Func<FixtureResult, TResult> read);

    void UpdateTestResult(Action<TestResult> update);

    TResult ReadTestResult<TResult>(Func<TestResult, TResult> read);

    void UpdateStepResult(Action<StepResult> update);

    void UpdateStepResult(int level, Action<StepResult> update);

    TResult ReadStepResult<TResult>(Func<StepResult, TResult> read);

    TResult ReadStepResult<TResult>(int level, Func<StepResult, TResult> read);

    void UpdateAllStepResults(Action<StepResult> update);

    ImmutableArray<TResult> ReadAllSteps<TResult>(Func<StepResult, TResult> read);

    void UpdateCurrentExecutableItem(Action<ExecutableItem> update);

    TResult ReadCurrentExecutableItem<TResult>(Func<ExecutableItem, TResult> read);
}
