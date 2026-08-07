using System;
using System.Collections.Immutable;
using System.Threading;

namespace Allure.TestingPlatform.Sdk.ExecutionState;

public abstract class ExecutionStateContext
{
    readonly AsyncLocal<ImmutableStack<StepExecutionStateUid>> substeps = new()
    {
        Value = [],
    };

    readonly AsyncLocal<FixtureExecutionStateUid?> fixture = new()
    {
        Value = default,
    };

    public abstract ScopeExecutionStateUid? CurrentScopeUid { get; }

    public abstract TestExecutionStateUid? CurrentTestUid { get; }

    protected abstract FixtureExecutionStateUid? CurrentFrameworkFixtureUid { get; }

    protected abstract StepExecutionStateUid? CurrentFrameworkStepUid { get; }

    public FixtureExecutionStateUid? CurrentFixtureUid =>
        this.fixture.Value ?? this.CurrentFrameworkFixtureUid;

    public StepExecutionStateUid? CurrentStepUid =>
        this.substeps.Value.IsEmpty
            ? this.CurrentFrameworkStepUid
            : this.substeps.Value.Peek();

    internal IDisposable EnterApiFixtureScope(FixtureExecutionStateUid fixtureUid)
    {
        if (this.CurrentFixtureUid.HasValue)
        {
            throw new InvalidOperationException("Another fixture is currently running.");
        }

        this.fixture.Value = fixtureUid;
        return new FixtureScope(this, fixtureUid);
    }

    internal void ExitApiFixtureScope(FixtureExecutionStateUid fixtureUid)
    {
        if (this.fixture.Value == fixtureUid)
        {
            this.fixture.Value = default;
        }
    }

    internal IDisposable EnterApiStepScope(StepExecutionStateUid stepUid)
    {
        if (!this.CurrentTestUid.HasValue && !this.CurrentFixtureUid.HasValue)
        {
            throw new InvalidOperationException(
                "Neither test nor fixture is currently running."
            );
        }

        this.substeps.Value = this.substeps.Value.Push(stepUid);
        return new StepScope(this, stepUid);
    }

    internal void ExitApiStepScope(StepExecutionStateUid stepUid)
    {
        var apiSteps = this.substeps.Value;
        if (apiSteps.IsEmpty)
        {
            throw new InvalidOperationException(
                "The step is not running."
            );
        }

        if (apiSteps.Peek() != stepUid)
        {
            throw new InvalidOperationException(
                "A substep is still running."
            );
        }

        this.substeps.Value = apiSteps.Pop();
    }

    sealed class FixtureScope(ExecutionStateContext context, FixtureExecutionStateUid fixtureUid) : IDisposable
    {
        int disposed = 0;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) != 0)
            {
                return;
            }

            context.ExitApiFixtureScope(fixtureUid);
        }
    }

    sealed class StepScope(ExecutionStateContext context, StepExecutionStateUid stepUid) : IDisposable
    {
        int disposed = 0;

        public void Dispose()
        {
            if (Interlocked.Exchange(ref this.disposed, 1) != 0)
            {
                return;
            }

            context.ExitApiStepScope(stepUid);
        }
    }
}
