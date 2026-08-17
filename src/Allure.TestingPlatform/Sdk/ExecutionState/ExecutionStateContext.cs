using System;
using System.Collections.Immutable;
using System.Threading;

namespace Allure.TestingPlatform.Sdk.ExecutionState;

/// <summary>
/// Tracks the Allure execution-state identifiers associated with the current asynchronous
/// control flow.
/// </summary>
/// <remarks>
/// Framework integrations provide the current framework-managed scope, test, fixture, and step.
/// Allure API-managed fixture and step scopes are tracked by the base class.
/// </remarks>
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

    /// <summary>
    /// Gets the identifier of the current test scope, or <see langword="null"/> when no scope
    /// is active.
    /// </summary>
    public abstract ScopeExecutionStateUid? CurrentScopeUid { get; }

    /// <summary>
    /// Gets the identifier of the current test, or <see langword="null"/> when no test is active.
    /// </summary>
    public abstract TestExecutionStateUid? CurrentTestUid { get; }

    /// <summary>
    /// Gets the identifier of the current framework-managed fixture, or
    /// <see langword="null"/> when no framework fixture is active.
    /// </summary>
    protected abstract FixtureExecutionStateUid? CurrentFrameworkFixtureUid { get; }

    /// <summary>
    /// Gets the identifier of the current framework-managed step, or
    /// <see langword="null"/> when no framework step is active.
    /// </summary>
    protected abstract StepExecutionStateUid? CurrentFrameworkStepUid { get; }

    /// <summary>
    /// Gets the identifier of the current fixture, including fixtures started through the
    /// Allure API, or <see langword="null"/> when no fixture is active.
    /// </summary>
    public FixtureExecutionStateUid? CurrentFixtureUid =>
        this.fixture.Value ?? this.CurrentFrameworkFixtureUid;

    /// <summary>
    /// Gets the identifier of the innermost current step, including steps started through the
    /// Allure API, or <see langword="null"/> when no step is active.
    /// </summary>
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
