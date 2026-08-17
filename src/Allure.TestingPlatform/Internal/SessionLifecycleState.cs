using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal;

sealed class SessionLifecycleState(IAllureExecutionContext context)
{
    readonly Dictionary<IAllureExecutionStateUid, AllureExecutionState> states = [];
    readonly Dictionary<TestExecutionStateUid, AllureExecutionState> testScopeStates = [];
    readonly Dictionary<ScopeExecutionStateUid, ImmutableHashSet<TestExecutionStateUid>> scopeTests = [];
    readonly Dictionary<IAllureExecutionStateUid, Queue<Action>> pendingUpdates = [];

    public bool TryGetState(
        IAllureExecutionStateUid stateUid,
        [MaybeNullWhen(false)] out AllureExecutionState state
    ) =>
        this.states.TryGetValue(stateUid, out state);

    public void SetState(IAllureExecutionStateUid stateUid, AllureExecutionState state)
    {
        this.states[stateUid] = this.ApplyPendingUpdates(stateUid, state);
    }

    public AllureExecutionState GetNewTestState(TestExecutionStateUid testStateUid) =>
        this.TryGetTestScope(testStateUid, out var scopeState)
            ? scopeState
            : new();

    public void AddPendingUpdate(IAllureExecutionStateUid stateUid, Action update)
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

    public void InheritState(
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

    public AllureExecutionState ForkState(IAllureExecutionStateUid stateUid, AllureExecutionState state, Action mutations)
    {
        var newState = context.RunWithState(state, (_) => mutations());
        this.SetState(stateUid, newState);
        return newState;
    }

    public AllureExecutionState ForkNewTestState(TestExecutionStateUid testStateUid, Action startTest) =>
        this.ForkState(testStateUid, this.GetNewTestState(testStateUid), startTest);

    public AllureExecutionState GetRunningTestState(TestExecutionStateUid testStateUid) =>
        this.TryGetState(testStateUid, out var state)
            ? state
            : this.GetNewTestState(testStateUid);

    public void ForkCurrentState(IAllureExecutionStateUid stateUid, Action update) =>
        this.SetState(stateUid, context.RunWithState(context.CurrentState, (_) => update()));

    public void UpdateState(IAllureExecutionStateUid stateUid, Action update)
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

    public void ReleaseState(IAllureExecutionStateUid stateUid, Action<IAllureRuntimeBase> commit)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.states, stateUid, out var state))
        {
            context.RunWithState(state, commit);
        }
    }

    public void ReleaseScopeState(ScopeExecutionStateUid scopeUid, Action<IAllureRuntimeBase> commit)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.scopeTests, scopeUid, out var testUids))
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

    AllureExecutionState ApplyPendingUpdates(IAllureExecutionStateUid stateUid, AllureExecutionState state)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.pendingUpdates, stateUid, out var updates))
        {
            foreach (var update in updates)
            {
                state = context.RunWithState(state, (_) => update());
            }
        }

        return state;
    }

    void AddTestScopeAssociations(ScopeExecutionStateUid scopeUid, AllureExecutionState scopeState, ImmutableArray<TestExecutionStateUid> testUids)
    {
        this.scopeTests[scopeUid] = this.scopeTests.TryGetValue(scopeUid, out var currentScopeTestUids)
            ? currentScopeTestUids.Union(testUids)
            : [.. testUids];

        foreach (var testUid in testUids)
        {
            this.testScopeStates[testUid] = scopeState;

            if (this.TryGetState(testUid, out var testState) && testState.HasTest)
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
}
