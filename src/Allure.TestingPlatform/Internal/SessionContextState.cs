using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Allure.Net.Commons;
using Allure.TestingPlatform.Functions;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Internal;

internal class SessionContextState(AllureLifecycle lifecycle)
{
    readonly Dictionary<IAllureContextUid, AllureContext> contexts = [];
    readonly Dictionary<TestContextUid, AllureContext> testScopeContexts = [];
    readonly Dictionary<ScopeContextUid, ImmutableHashSet<TestContextUid>> scopeTests = [];
    readonly Dictionary<IAllureContextUid, Queue<Action>> pendingUpdates = [];

    public bool TryGetContext(
        IAllureContextUid contextUid,
        [NotNullWhen(true)] out AllureContext? context
    ) =>
        this.contexts.TryGetValue(contextUid, out context);

    public void SetContext(IAllureContextUid contextUid, AllureContext context)
    {
        this.contexts[contextUid] = this.ApplyPendingUpdates(contextUid, context);
    }

    public AllureContext GetNewTestContext(TestContextUid testContextUid) =>
        this.TryGetTestScope(testContextUid, out var scopeContext)
            ? scopeContext
            : new();

    public void AddPendingUpdate(IAllureContextUid contextUid, Action update)
    {
        if (this.TryGetPendingUpdates(contextUid, out var updates))
        {
            updates.Enqueue(update);
        }
        else
        {
            this.pendingUpdates[contextUid] = new([update]);
        }
    }

    public void InheritContext(
        IAllureContextUid contextUid,
        IAllureContextUid? parentContextUid,
        Action init
    )
    {
        if (parentContextUid is not null)
        {
            if (this.TryGetContext(parentContextUid, out var parentContext))
            {
                this.ForkContext(contextUid, parentContext, init);
            }
            else
            {
                this.AddPendingUpdate(parentContextUid, () =>
                {
                    this.ForkCurrentContext(contextUid, init);
                });
            }
        }
        else
        {
            this.ForkContext(contextUid, new(), init);
        }
    }

    public AllureContext ForkContext(IAllureContextUid contextUid, AllureContext context, Action mutations)
    {
        var newContext = lifecycle.RunInContext(context, mutations);
        this.SetContext(contextUid, newContext);
        return newContext;
    }

    public AllureContext ForkNewTestContext(TestContextUid testContextUid, Action startTest) =>
        this.ForkContext(testContextUid, this.GetNewTestContext(testContextUid), startTest);

    public AllureContext GetRunningTestContext(TestContextUid testContextUid) =>
        this.TryGetContext(testContextUid, out var context)
            ? context
            : this.GetNewTestContext(testContextUid);

    public void ForkCurrentContext(IAllureContextUid contextUid, Action update) =>
        this.SetContext(contextUid, lifecycle.RunInContext(lifecycle.Context, update));

    public void UpdateContext(IAllureContextUid contextUid, Action update)
    {
        if (this.TryGetContext(contextUid, out var context))
        {
            this.SetContext(
                contextUid,
                lifecycle.RunInContext(context, update)
            );
        }
        else
        {
            this.AddPendingUpdate(contextUid, update);
        }
    }

    public void ReleaseContext(IAllureContextUid contextUid, Action commit)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.contexts, contextUid, out var context))
        {
            lifecycle.RunInContext(context, commit);
        }
    }

    public void ReleaseScopeContext(ScopeContextUid scopeUid, Action commit)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.scopeTests, scopeUid, out var testUids))
        {
            foreach (var testUid in testUids)
            {
                this.testScopeContexts.Remove(testUid);
            }
        }
        this.ReleaseContext(scopeUid, commit);
    }

    public void AssociateTestsWithScope(ScopeContextUid scopeUid, ImmutableArray<TestContextUid> testUids)
    {
        if (!this.TryGetContext(scopeUid, out var scopeContext))
        {
            this.AddPendingUpdate(scopeUid, () =>
            {
                this.AddTestScopeAssociations(scopeUid, lifecycle.Context, testUids);
            });
            return;
        }

        this.AddTestScopeAssociations(scopeUid, scopeContext, testUids);
    }

    AllureContext ApplyPendingUpdates(IAllureContextUid contextUid, AllureContext context)
    {
        if (CollectionAlgorithms.TryRemoveAndGet(this.pendingUpdates, contextUid, out var updates))
        {
            foreach (var update in updates)
            {
                context = lifecycle.RunInContext(context, update);
            }
        }

        return context;
    }

    void AddTestScopeAssociations(ScopeContextUid scopeUid, AllureContext scopeContext, ImmutableArray<TestContextUid> testUids)
    {
        this.scopeTests[scopeUid] = this.scopeTests.TryGetValue(scopeUid, out var currentScopeTestUids)
            ? currentScopeTestUids.Union(testUids)
            : testUids.ToImmutableHashSet();

        foreach (var testUid in testUids)
        {
            this.testScopeContexts[testUid] = scopeContext;

            if (this.TryGetContext(testUid, out var testContext) && testContext.HasTest)
            {
                string? testUuid = null;
                lifecycle.RunInContext(testContext, () =>
                {
                    lifecycle.UpdateTestCase(tr => testUuid = tr.uuid);
                });

                lifecycle.RunInContext(scopeContext, () =>
                {
                    lifecycle.UpdateTestContainers((c) => c.children.Add(testUuid));
                });
            }
        }
    }

    bool TryGetPendingUpdates(
        IAllureContextUid contextUid,
        [NotNullWhen(true)] out Queue<Action>? updates
    ) =>
        this.pendingUpdates.TryGetValue(contextUid, out updates);

    bool TryGetTestScope(TestContextUid testContextUid, [NotNullWhen(true)] out AllureContext? scopeContext) =>
        this.TryGetTestLevelScope(testContextUid, out scopeContext)
            || this.TryGetExplicitTestScope(testContextUid, out scopeContext);

    bool TryGetTestLevelScope(TestContextUid testContextUid, [NotNullWhen(true)] out AllureContext? scopeContext) =>
        this.TryGetContext(new ScopeContextUid(testContextUid.Value), out scopeContext);

    bool TryGetExplicitTestScope(TestContextUid testContextUid, [NotNullWhen(true)] out AllureContext? scopeContext) =>
        this.testScopeContexts.TryGetValue(testContextUid, out scopeContext);
}
