using System;
using System.Threading;
using Allure.Net.Commons;
using Reqnroll;
using Reqnroll.Bindings;
using Reqnroll.Bindings.Reflection;
using Reqnroll.Infrastructure;

namespace Allure.ReqnrollPlugin.State;

internal class CrossBindingContextTransport
{
    const string SNAPSHOT_KEY = $"ALLURE_STATE_SNAPSHOT";

    readonly IContextManager reqnrollContexts;

    static AllureLifecycle Lifecycle
    {
        get => AllureLifecycle.Instance;
    }

    FeatureContext? Feature
    {
        get => reqnrollContexts.FeatureContext;
    }

    ScenarioContext? Scenario
    {
        get => reqnrollContexts.ScenarioContext;
    }

    ReqnrollContext? CurrentReqnrollContext
    {
        get => this.Scenario as ReqnrollContext ?? this.Feature;
    }

    ExecutionContextHolder? ExecutionContextBox
    {
        get =>
            this.Scenario?.ScenarioContainer.Resolve<ExecutionContextHolder>();
    }

    public CrossBindingContextTransport(IContextManager reqnrollContexts)
    {
        this.reqnrollContexts = reqnrollContexts;
    }

    internal void PropagateState() =>
        this.PropagateState(() => { });

    internal void PropagateState(Action update) =>
        this.SaveState(
            this.ApplyStateTransition(
                this.ExecutionContextBox,
                this.ResolveState(),
                update
            )
        );

    internal void MakeSnapshot(IBindingMethod associatedMethod) =>
        this.CurrentReqnrollContext?.Set(
            new StateSnapshot(Lifecycle.Context, associatedMethod),
            SNAPSHOT_KEY
        );

    internal StateSnapshot? GetLastSnapshot()
    {
        StateSnapshot? snapshot = null;
        bool? success =
            this.CurrentReqnrollContext?.TryGetValue(
                SNAPSHOT_KEY,
                out snapshot
            );
        return success is true ? snapshot : null;
    }

    internal void ClearSnapshot() =>
        this.CurrentReqnrollContext?.Remove(SNAPSHOT_KEY);

    AllureContext ApplyStateTransition(
        ExecutionContextHolder? executionContextBox,
        AllureContext state,
        Action update
    ) =>
        executionContextBox is null
            ? this.ApplyDirectStateTransition(state, update)
            : this.ApplyPersistentStateTransition(
                executionContextBox,
                state,
                update
            );

    AllureContext ApplyDirectStateTransition(AllureContext state, Action update) =>
        Lifecycle.RunInContext(state, update);

    AllureContext ApplyPersistentStateTransition(
        ExecutionContextHolder executionContextBox,
        AllureContext state,
        Action update
    ) =>
        executionContextBox.Value is null
            ? this.UpdateStateAndSaveExecutionContext(
                executionContextBox,
                state,
                update
            )
            : this.UpdateExecutionContext(
                executionContextBox,
                state,
                update
            );

    AllureContext UpdateStateAndSaveExecutionContext(
        ExecutionContextHolder executionContextBox,
        AllureContext state,
        Action update
    ) =>
        Lifecycle.RunInContext(state, () =>
        {
            update();
            executionContextBox.Value = ExecutionContext.Capture();
        });

    AllureContext UpdateExecutionContext(
        ExecutionContextHolder executionContextBox,
        AllureContext state,
        Action update
    )
    {
        AllureContext newState = state;
        ExecutionContext.Run(
            executionContextBox.Value,
            _ => newState = this.UpdateStateAndSaveExecutionContext(
                executionContextBox,
                state,
                update
            ),
            null
        );
        return newState;
    }

    AllureContext ResolveState()
    {
        if (this.Scenario?.TryGetValue<AllureContext>(out var allureContext) != true)
        {
            if (this.Feature?.TryGetValue<AllureContext>(out allureContext) != true)
            {
                allureContext = Lifecycle.Context;
                this.Feature?.Set(allureContext);
            }
            this.Scenario?.Set(allureContext);
        }
        return allureContext;
    }

    void SaveState(AllureContext newState) =>
        this.CurrentReqnrollContext?.Set(newState);
}
