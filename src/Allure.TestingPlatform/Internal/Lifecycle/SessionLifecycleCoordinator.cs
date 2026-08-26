using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Internal.Functions;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal.Lifecycle;

sealed class SessionLifecycleCoordinator(
    IAllureExecutionContext context,
    ITestExecutionCoordinator testExecutionCoordinator
)
{
    readonly Dictionary<IAllureExecutionStateUid, AllureExecutionState> states = [];
    readonly Dictionary<TestExecutionStateUid, AllureExecutionState> testScopeStates = [];
    readonly Dictionary<ScopeExecutionStateUid, HashSet<TestExecutionStateUid>> scopeTests = [];
    readonly Dictionary<IAllureExecutionStateUid, Queue<Action>> pendingUpdates = [];
    readonly HashSet<IAllureExecutionStateUid> pendingReleases = [];

    public void InheritState(
        IAllureExecutionStateUid stateUid,
        IAllureExecutionStateUid? parentStateUid,
        Action init
    )
    {
        if (parentStateUid is TestExecutionStateUid testUid)
        {
            testExecutionCoordinator.Route(
                testUid,
                (testNodeUid) => this.InheritStateCore(stateUid, testNodeUid, init)
            );
        }
        else
        {
            this.InheritStateCore(stateUid, parentStateUid, init);
        }
    }

    public void StartTestState(TestExecutionStateUid testNodeUid, Action startTest) =>
        testExecutionCoordinator.StartTestNode(
            testNodeUid,
            () => this.StartTestStateCore(testNodeUid, startTest)
        );

    public void FinishTestState(
        TestExecutionStateUid testNodeUid,
        Action startTestIfMissing,
        Action<IAllureRuntimeBase> finishTest
    ) =>
        testExecutionCoordinator.FinishTestNode(
            testNodeUid,
            () => this.FinishTestStateCore(testNodeUid, startTestIfMissing, finishTest)
        );

    public void BindTestExecution(
        TestExecutionStateUid testNodeUid,
        TestExecutionStateUid executionUid
    ) =>
        testExecutionCoordinator.BindTestExecution(
            testNodeUid,
            executionUid
        );

    public void FinishTestExecution(TestExecutionStateUid executionUid) =>
        testExecutionCoordinator.FinishTestExecution(executionUid);

    public void UpdateState(IAllureExecutionStateUid stateUid, Action update)
    {
        if (stateUid is TestExecutionStateUid testUid)
        {
            testExecutionCoordinator.Route(
                testUid,
                (testNodeUid) => this.UpdateStateCore(testNodeUid, update)
            );
        }
        else
        {
            this.UpdateStateCore(stateUid, update);
        }
    }

    public void ReleaseState(IAllureExecutionStateUid stateUid, Action<IAllureRuntimeBase> commit)
    {
        if (Dictionaries.TryRemoveAndGet(this.states, stateUid, out var state))
        {
            context.RunWithState(state, commit);
        }
        else
        {
            this.AddPendingUpdate(stateUid, () => commit(context.Runtime));
            this.pendingReleases.Add(stateUid);
        }
    }

    public void ReleaseScopeState(ScopeExecutionStateUid scopeUid, Action<IAllureRuntimeBase> commit)
    {
        if (Dictionaries.TryRemoveAndGet(this.scopeTests, scopeUid, out var testUids))
        {
            foreach (var testUid in testUids)
            {
                this.testScopeStates.Remove(testUid);
            }
        }
        this.ReleaseState(scopeUid, commit);
    }

    public void AssociateTestsWithScope(ScopeExecutionStateUid scopeUid, ImmutableArray<TestExecutionStateUid> testUids)
    {
        if (!this.TryGetState(scopeUid, out var scopeState))
        {
            this.AddPendingUpdate(scopeUid, () =>
            {
                this.AddTestScopeAssociations(scopeUid, context.CurrentState, testUids);
            });
            return;
        }

        this.AddTestScopeAssociations(scopeUid, scopeState, testUids);
    }

    void StartTestStateCore(TestExecutionStateUid testNodeUid, Action startTest)
    {
        if (this.TryGetState(testNodeUid, out var existingState) && existingState.HasTest)
        {
            throw new InvalidOperationException(
                $"Test node {testNodeUid} is already running."
            );
        }

        this.ForkState(testNodeUid, this.GetNewTestState(testNodeUid), startTest);
    }

    void FinishTestStateCore(
        TestExecutionStateUid testNodeUid,
        Action startTestIfMissing,
        Action<IAllureRuntimeBase> finishTest
    )
    {
        if (!this.states.TryGetValue(testNodeUid, out var state) || !state.HasTest)
        {
            // FinishTestState ensures we're inside the occurrence now.
            // We can call StartTestStateCore directly to create the state.
            this.StartTestStateCore(testNodeUid, startTestIfMissing);
        }

        this.ReleaseState(testNodeUid, finishTest);
    }

    void UpdateStateCore(IAllureExecutionStateUid stateUid, Action update)
    {
        if (this.TryGetState(stateUid, out var state))
        {
            this.SetState(
                stateUid,
                context.RunWithState(state, (_) => update())
            );
        }
        else
        {
            this.AddPendingUpdate(stateUid, update);
        }
    }

    void InheritStateCore(
        IAllureExecutionStateUid stateUid,
        IAllureExecutionStateUid? parentStateUid,
        Action init
    )
    {
        if (parentStateUid is not null)
        {
            if (this.TryGetState(parentStateUid, out var parentState))
            {
                this.ForkState(stateUid, parentState, init);
            }
            else
            {
                this.AddPendingUpdate(parentStateUid, () =>
                {
                    this.ForkCurrentState(stateUid, init);
                });
            }
        }
        else
        {
            this.ForkState(stateUid, new(), init);
        }
    }

    void AddPendingUpdate(IAllureExecutionStateUid stateUid, Action update)
    {
        if (this.TryGetPendingUpdates(stateUid, out var updates))
        {
            updates.Enqueue(update);
        }
        else
        {
            this.pendingUpdates[stateUid] = new([update]);
        }
    }

    AllureExecutionState ApplyPendingUpdates(IAllureExecutionStateUid stateUid, AllureExecutionState state)
    {
        if (Dictionaries.TryRemoveAndGet(this.pendingUpdates, stateUid, out var updates))
        {
            foreach (var update in updates)
            {
                state = context.RunWithState(state, (_) => update());
            }
        }

        return state;
    }

    void AddTestScopeAssociations(
        ScopeExecutionStateUid scopeUid,
        AllureExecutionState scopeState,
        ImmutableArray<TestExecutionStateUid> testUids
    )
    {
        foreach (var testUid in testUids)
        {
            testExecutionCoordinator.Route(testUid, AssociateTestWithScope);
        }

        void AssociateTestWithScope(TestExecutionStateUid testNodeUid)
        {
            if (this.scopeTests.TryGetValue(scopeUid, out var currentScopeTests))
            {
                currentScopeTests.Add(testNodeUid);
            }
            else
            {
                this.scopeTests[scopeUid] = [testNodeUid];
            }

            this.testScopeStates[testNodeUid] = scopeState;

            if (this.TryGetState(testNodeUid, out var testState) && testState.HasTest)
            {
                string testUuid = context.GetWithState(
                    testState,
                    (runtime) => runtime.ModelApi.ReadTestResult(static (tr) => tr.Uuid)
                );

                context.RunWithState(scopeState, (runtime) =>
                {
                    runtime.ModelApi.UpdateScope((c) => c.Children.Add(testUuid));
                });
            }
        }
    }

    AllureExecutionState ForkState(
        IAllureExecutionStateUid stateUid,
        AllureExecutionState state,
        Action mutations
    )
    {
        var newState = context.RunWithState(state, (_) => mutations());
        this.SetState(stateUid, newState);
        return newState;
    }

    void ForkCurrentState(IAllureExecutionStateUid stateUid, Action update) =>
        this.SetState(stateUid, context.RunWithState(context.CurrentState, (_) => update()));

    bool TryGetPendingUpdates(
        IAllureExecutionStateUid stateUid,
        [NotNullWhen(true)] out Queue<Action>? updates
    ) =>
        this.pendingUpdates.TryGetValue(stateUid, out updates);

    bool TryGetTestScope(TestExecutionStateUid testStateUid, [MaybeNullWhen(false)] out AllureExecutionState scopeState) =>
        this.TryGetTestLevelScope(testStateUid, out scopeState)
            || this.TryGetExplicitTestScope(testStateUid, out scopeState);

    bool TryGetTestLevelScope(TestExecutionStateUid testStateUid, [MaybeNullWhen(false)] out AllureExecutionState scopeState) =>
        this.TryGetState(new ScopeExecutionStateUid(testStateUid.Value), out scopeState);

    bool TryGetExplicitTestScope(TestExecutionStateUid testStateUid, [MaybeNullWhen(false)] out AllureExecutionState scopeState) =>
        this.testScopeStates.TryGetValue(testStateUid, out scopeState);

    bool TryGetState(
        IAllureExecutionStateUid stateUid,
        [MaybeNullWhen(false)] out AllureExecutionState state
    ) =>
        this.states.TryGetValue(stateUid, out state);

    void SetState(IAllureExecutionStateUid stateUid, AllureExecutionState state)
    {
        var updatedState = this.ApplyPendingUpdates(stateUid, state);
        if (!this.pendingReleases.Remove(stateUid))
        {
            this.states[stateUid] = updatedState;
        }
    }

    AllureExecutionState GetNewTestState(TestExecutionStateUid testStateUid) =>
        this.TryGetTestScope(testStateUid, out var scopeState)
            ? scopeState
            : new();
}
