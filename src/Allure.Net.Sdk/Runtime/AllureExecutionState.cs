using System;
using System.Collections.Immutable;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Allure.Model;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Represents an immutable snapshot of the current Allure execution state,
/// including the active test result scope stack, fixture, test, and
/// step stack.
/// </summary>
[DebuggerDisplay(
    "Scopes = {ScopeDepth}, HasFixture = {HasFixture}, " +
        "HasTest = {HasTest}, Steps = {StepDepth}"
)]
public readonly record struct AllureExecutionState()
{
    /// <summary>
    /// Returns <see langword="true"/> if a scope is running.
    /// Otherwise, returns <see langword="false"/>.
    /// </summary>
    public bool HasScope => !this.ScopeStack.IsEmpty;

    /// <summary>
    /// Returns the number of nested scopes that are currently running.
    /// </summary>
    public int ScopeDepth => this.ScopeStack.Count();

    /// <summary>
    /// Returns <see langword="true"/> if a fixture is running.
    /// Otherwise, returns <see langword="false"/>.
    /// </summary>
    public bool HasFixture => this.FixtureResult is not null;

    /// <summary>
    /// Returns <see langword="true"/> if a test is running.
    /// Otherwise, returns <see langword="false"/>.
    /// </summary>
    public bool HasTest => this.TestResult is not null;

    /// <summary>
    /// Returns <see langword="true"/> if a step is running.
    /// Otherwise, returns <see langword="false"/>.
    /// </summary>
    public bool HasStep => !this.StepStack.IsEmpty;

    /// <summary>
    /// Returns the number of nested steps that are currently running.
    /// </summary>
    public int StepDepth => this.StepStack.Count();

    internal ImmutableStack<TestResultScope> ScopeStack
    {
        get;
        private init;
    } = [];

    internal FixtureResult? FixtureResult { get; private init; }

    internal TestResult? TestResult { get; private init; }

    internal ImmutableStack<StepResult> StepStack
    {
        get;
        private init;
    } = ImmutableStack<StepResult>.Empty;

    internal TestResultScope CurrentScope
    {
        get => this.ScopeStack.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "No scope is running."
            );
    }

    internal FixtureResult CurrentFixture =>
        this.FixtureResult ?? throw new InvalidOperationException(
            "No fixture is running."
        );

    internal TestResult CurrentTest =>
        this.TestResult ?? throw new InvalidOperationException(
            "No test is running."
        );

    internal StepResult CurrentStep =>
        this.StepStack.FirstOrDefault()
            ?? throw new InvalidOperationException(
                "No step is running."
            );

    internal ExecutableItem CurrentExecutableItem =>
        this.StepStack.FirstOrDefault() as ExecutableItem
            ?? this.RootStepContainer
            ?? throw new InvalidOperationException(
                "No fixture, test, or step is running."
            );

#pragma warning disable IDE0051
    private bool PrintMembers(StringBuilder stringBuilder)
#pragma warning restore IDE0051
    {
        var scopes =
            RepresentStack(this.ScopeStack, c => c.Name ?? c.Uuid);
        var fixture = this.FixtureResult?.Name ?? "null";
        var test = this.TestResult?.Name
            ?? this.TestResult?.Uuid
            ?? "null";
        var steps = RepresentStack(this.StepStack, s => s.Name);

        stringBuilder.AppendFormat("Scopes = [{0}], ", scopes);
        stringBuilder.AppendFormat("Fixture = {0}, ", fixture);
        stringBuilder.AppendFormat("Test = {0}, ", test);
        stringBuilder.AppendFormat("Steps = [{0}]", steps);
        return true;
    }

    internal AllureExecutionState PushScope(TestResultScope scope) =>
        this.ValidateCanChangeScopeStack() with
        {
            ScopeStack = this.ScopeStack.Push(
                scope ?? throw new ArgumentNullException(
                    nameof(scope)
                )
            )
        };

    internal AllureExecutionState PopScope() =>
        this with
        {
            ScopeStack = this.ValidateCanPopScope()
                .ScopeStack.Pop()
        };

    internal AllureExecutionState SetFixtureResult(FixtureResult fixtureResult) =>
        this with
        {
            FixtureResult = this.ValidateNewFixtureResult(
                fixtureResult ?? throw new ArgumentNullException(
                    nameof(fixtureResult)
                )
            ),
            StepStack = this.StepStack.Clear()
        };

    internal AllureExecutionState ClearFixtureResult() =>
        this with
        {
            FixtureResult = null,
            StepStack = this.StepStack.Clear()
        };

    internal AllureExecutionState SetTestResult(TestResult testResult) =>
        this with
        {
            TestResult = this.ValidateNewTestResult(
                testResult ?? throw new ArgumentNullException(
                    nameof(testResult)
                )
            )
        };

    internal AllureExecutionState ClearTestResult() =>
        this with
        {
            FixtureResult = null,
            TestResult = null,
            StepStack = this.StepStack.Clear()
        };

    internal AllureExecutionState PushStepResult(StepResult stepResult) =>
        this with
        {
            StepStack = this.StepStack.Push(
                this.ValidateNewStep(
                    stepResult ?? throw new ArgumentNullException(
                        nameof(stepResult)
                    )
                )
            )
        };

    internal AllureExecutionState PopStepResult() =>
        this with
        {
            StepStack = this.HasStep
                ? this.StepStack.Pop()
                : throw new InvalidOperationException(
                    "Cannot pop a step from the step stack because "
                        + "no step is currently running."
                )
        };

    AllureExecutionState ValidateCanChangeScopeStack()
    {
        if (this.FixtureResult is not null)
        {
            throw new InvalidOperationException(
                "Cannot change the scope stack because a fixture "
                    + "is currently running."
            );
        }

        if (this.TestResult is not null)
        {
            throw new InvalidOperationException(
                "Cannot change the scope stack because a test "
                    + "is currently running."
            );
        }

        return this;
    }

    AllureExecutionState ValidateCanPopScope()
    {
        if (!this.HasScope)
        {
            throw new InvalidOperationException(
                "Cannot pop a scope from the scope stack becasue no "
                    + "scope is currently running."
            );
        }

        return this.ValidateCanChangeScopeStack();
    }

    ExecutableItem? RootStepContainer
    {
        get => this.FixtureResult as ExecutableItem ?? this.TestResult;
    }

    FixtureResult ValidateNewFixtureResult(FixtureResult fixture)
    {
        if (!this.HasScope)
        {
            throw new InvalidOperationException(
                "Cannot start a new fixture because "
                    + "there is no scope to attach it to."
            );
        }

        if (this.HasFixture)
        {
            throw new InvalidOperationException(
                "Cannot start a new fixture because "
                    + "another fixture is currently running."
            );
        }

        return fixture;
    }

    TestResult ValidateNewTestResult(TestResult testResult)
    {
        if (this.HasFixture)
        {
            throw new InvalidOperationException(
                "Cannot start a new test because "
                    + "a fixture is currently running."
            );
        }

        if (this.HasTest)
        {
            throw new InvalidOperationException(
                "Cannot start a new test because "
                    + "another test is currently running."
            );
        }

        return testResult;
    }

    StepResult ValidateNewStep(StepResult stepResult)
    {
        if (!this.HasTest && !this.HasFixture)
        {
            throw new InvalidOperationException(
                "Cannot start a new step becasue there is"
                    + "neither test, nor fixture to receive the step."
            );
        }

        return stepResult;
    }

    static string RepresentStack<T>(
        IImmutableStack<T> stack,
        Func<T, string> projection
    ) => string.Join(
        " <- ",
        stack.Select(projection)
    );
}
