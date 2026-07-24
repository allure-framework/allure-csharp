using System;
using System.Collections.Immutable;
using System.Linq;
using System.Threading;
using Allure.Model;
using Allure.Sdk.Registration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

class RuntimeBoundModelApi(
    ILateBoundReferenceView<IAllureRuntime> runtimeReference
) :
    IAllureModelApi
{
    AllureExecutionState CurrentState => runtimeReference.Value.ContextApi.CurrentState;

    readonly ReaderWriterLockSlim @lock = new(LockRecursionPolicy.NoRecursion);

    public TResult ReadCurrentExecutableItem<TResult>(Func<ExecutableItem, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(this.CurrentState.CurrentExecutableItem);
    }

    public TResult ReadFixtureResult<TResult>(Func<FixtureResult, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(this.CurrentState.CurrentFixture);
    }

    public TResult ReadScope<TResult>(Func<TestResultScope, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(this.CurrentState.CurrentScope);
    }

    public TResult ReadScope<TResult>(int level, Func<TestResultScope, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(ElementAtLevel(this.CurrentState.ScopeStack, level));
    }

    public ImmutableArray<TResult> ReadAllScopes<TResult>(Func<TestResultScope, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return this.CurrentState.ScopeStack.Select(read).ToImmutableArray();
    }

    public TResult ReadStepResult<TResult>(Func<StepResult, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(this.CurrentState.CurrentStep);
    }

    public TResult ReadStepResult<TResult>(int level, Func<StepResult, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(ElementAtLevel(this.CurrentState.StepStack, level));
    }

    public ImmutableArray<TResult> ReadAllSteps<TResult>(Func<StepResult, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return [.. this.CurrentState.StepStack.Select(read)];
    }

    public TResult ReadTestResult<TResult>(Func<TestResult, TResult> read)
    {
        using var _ = this.@lock.EnterReadScope();
        return read(this.CurrentState.CurrentTest);
    }

    public void UpdateCurrentExecutableItem(Action<ExecutableItem> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(this.CurrentState.CurrentExecutableItem);
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(this.CurrentState.CurrentFixture);
    }

    public void UpdateScope(Action<TestResultScope> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(this.CurrentState.CurrentScope);
    }

    public void UpdateScope(int level, Action<TestResultScope> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(ElementAtLevel(this.CurrentState.ScopeStack, level));
    }

    public void UpdateAllScopes(Action<TestResultScope> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        foreach (var scope in this.CurrentState.ScopeStack)
        {
            update(scope);
        }
    }

    public void UpdateStepResult(Action<StepResult> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(this.CurrentState.CurrentStep);
    }

    public void UpdateStepResult(int level, Action<StepResult> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(ElementAtLevel(this.CurrentState.StepStack, level));
    }

    public void UpdateAllStepResults(Action<StepResult> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        foreach (var stepResult in this.CurrentState.StepStack)
        {
            update(stepResult);
        }
    }

    public void UpdateTestResult(Action<TestResult> update)
    {
        using var _ = this.@lock.EnterWriteScope();
        update(this.CurrentState.CurrentTest);
    }

    static TElement ElementAtLevel<TElement>(ImmutableStack<TElement> stack, int level) =>
        stack.ElementAt(stack.Count() - level - 1);
}
