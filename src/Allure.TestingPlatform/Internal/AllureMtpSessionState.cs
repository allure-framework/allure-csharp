using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Internal;

internal class AllureMtpSessionState(AllureLifecycle lifecycle)
{
    readonly Dictionary<string, AllureContext> contexts = [];
    readonly Dictionary<string, Queue<Action>> pendingUpdates = [];
    readonly Dictionary<string, bool> sharedUids = [];
    readonly Dictionary<string, AllureContext> testScopes = [];

    public bool IsEmpty => this.contexts.Count == 0;

    public bool TryGetContext(string contextUid, [NotNullWhen(true)] out AllureContext? context) =>
        this.contexts.TryGetValue(contextUid, out context);

    public bool TryGetPendingUpdates(string contextUid, [NotNullWhen(true)] out Queue<Action>? updates) =>
        this.pendingUpdates.TryGetValue(contextUid, out updates);

    public void SetContext(string contextUid, AllureContext context)
    {
        this.contexts[contextUid] = context;
        this.ConsumePendingUpdates(contextUid);
    }

    public void ConsumePendingUpdates(string contextUid)
    {
        if (TryRemove(this.pendingUpdates, contextUid, out var updates))
        {
            foreach (var update in updates)
            {
                this.UpdateContext(contextUid, update);
            }
        }
    }

    public void AddPendingUpdate(string contextUid, Action update)
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

    public void RemoveContext(string contextUid)
    {
        TryRemove(this.contexts, contextUid, out var _);
    }

    public void CaptureContext(string contextUid)
    {
        this.SetContext(contextUid, lifecycle.Context);
    }

    public void InheritContext(string contextUid, string? parentContextUid, Action init)
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

    public void UpdateContext(string contextUid, Action update)
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

    public void ReleaseContext(string contextUid, Action commit)
    {
        if (TryRemove(this.contexts, contextUid, out var context))
        {
            lifecycle.RunInContext(context, commit);
        }
    }

    public void ReleaseScopeContext(string contextUid, Action commit)
    {
        this.testScopes.Remove(contextUid);
        this.sharedUids.Remove(contextUid);

        this.ReleaseContext(contextUid, commit);
    }

    public void MakeUidShared(string uid)
    {
        this.sharedUids[uid] = true;
    }

    public void RemoveTestContext(string testUid)
    {
        if (TryRemove(this.sharedUids, testUid, out _))
        {
            // A scope with the same Uid is active. The context will be removed via AllureScopeStopMessage.
            // We need to update the context to make sure it has no test result in it.
            this.CaptureContext(testUid);
        }
        else
        {
            // If no scope with the same Uid is active, we don't need the context anymore
            this.RemoveContext(testUid);
        }
    }

    public void AssociateTestsWithScope(string scopeUid, IEnumerable<string> testUids)
    {
        if (!this.TryGetContext(scopeUid, out var scope))
        {
            return;
        }

        // Remember the association for future test results
        foreach (var testUid in testUids)
        {
            this.testScopes[testUid] = scope;
        }

        // Move the current tests to the scope
        foreach (var testUid in testUids)
        {
            string? testUuid = null;
            if (this.TryGetContext(testUid, out var testContext))
            {
                lifecycle.RunInContext(testContext, () =>
                {
                    lifecycle.UpdateTestCase(tr => testUuid = tr.uuid);
                });

                lifecycle.RunInContext(scope, () =>
                {
                    lifecycle.UpdateTestContainers((c) => c.children.Add(testUuid));
                });
            }
        }
    }

    public void TryEnterTestScope(string testUid)
    {
        if (!this.sharedUids.ContainsKey(testUid)
            && this.testScopes.TryGetValue(testUid, out var scope))
        {
            lifecycle.RestoreContext(scope);
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
