using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Internal;

internal class SessionContextState(AllureLifecycle lifecycle)
{
    readonly Dictionary<IAllureContextUid, AllureContext> contexts = [];
    readonly Dictionary<TestContextUid, AllureContext> testScopeContexts = [];
    readonly Dictionary<ScopeContextUid, ImmutableArray<TestContextUid>> scopeTests = [];
    readonly Dictionary<IAllureContextUid, Queue<Action>> pendingUpdates = [];

    public bool IsEmpty => this.contexts.Count == 0 && this.pendingUpdates.Count == 0;

    public bool TryGetContext(
        IAllureContextUid contextUid,
        [NotNullWhen(true)] out AllureContext? context
    ) =>
        this.contexts.TryGetValue(contextUid, out context);

    public bool TryGetPendingUpdates(
        IAllureContextUid contextUid,
        [NotNullWhen(true)] out Queue<Action>? updates
    ) =>
        this.pendingUpdates.TryGetValue(contextUid, out updates);

    public void SetContext(IAllureContextUid contextUid, AllureContext context)
    {
        this.contexts[contextUid] = context;
        this.ConsumePendingUpdates(contextUid);
    }

    public void ConsumePendingUpdates(IAllureContextUid contextUid)
    {
        if (TryRemove(this.pendingUpdates, contextUid, out var updates))
        {
            foreach (var update in updates)
            {
                this.UpdateContext(contextUid, update);
            }
        }
    }

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

    public void RemoveContext(IAllureContextUid contextUid)
    {
        TryRemove(this.contexts, contextUid, out var _);
    }

    public void RemoveTestContext(TestContextUid testUid)
    {
        this.RemoveContext(testUid);
        if (this.contexts.ContainsKey(new ScopeContextUid(testUid.Value)))
        {
            // Test-level scope is active. The association is done by the UID.
            return;
        }

        // Test UIDs can be reused due to parameterization, retries, etc.
        // If the test's scope context is present, we reintroduce it as
        // the initial test context.
        if (this.testScopeContexts.TryGetValue(testUid, out var scope))
        {
            this.SetContext(testUid, scope);
        }
    }

    public void CaptureContext(IAllureContextUid contextUid)
    {
        this.SetContext(contextUid, lifecycle.Context);
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
                this.SetContext(
                    contextUid,
                    lifecycle.RunInContext(parentContext, init)
                );
            }
            else
            {
                // TODO: Cover with tests
                this.AddPendingUpdate(parentContextUid, () =>
                {
                    init();
                    this.SetContext(contextUid, lifecycle.Context);
                });
            }
        }
        else
        {
            this.SetContext(
                contextUid,
                lifecycle.RunInContext(new(), init)
            );
        }
    }

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
            // TODO: Cover with tests
            this.AddPendingUpdate(contextUid, update);
        }
    }

    public void ReleaseContext(IAllureContextUid contextUid, Action commit)
    {
        if (TryRemove(this.contexts, contextUid, out var context))
        {
            lifecycle.RunInContext(context, commit);
        }
    }

    public void ReleaseScopeContext(ScopeContextUid scopeUid, Action commit)
    {
        if (TryRemove(this.scopeTests, scopeUid, out var testUids))
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
            return;
        }

        this.scopeTests[scopeUid] = testUids;

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
            else
            {
                this.SetContext(testUid, scopeContext);
            }
        }
    }

    public void EnterContext(AllureContext context) =>
        lifecycle.RestoreContext(context);

    public void EnterContextIfExists(IAllureContextUid contextUid)
    {
        if (this.TryGetContext(contextUid, out var context))
        {
            this.EnterContext(context);
        }
    }

    static bool TryRemove<K, V>(Dictionary<K, V> dictionary, K key, out V value)
    {
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }
        return false;
    }
}
