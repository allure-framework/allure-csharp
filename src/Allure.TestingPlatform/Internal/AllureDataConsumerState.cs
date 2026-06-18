using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using Allure.Net.Commons;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Internal;

internal class AllureDataConsumerState(AllureLifecycle lifecycle)
{
    readonly ConcurrentDictionary<(string, string), AllureContext> contexts = [];
    readonly ConcurrentDictionary<(string, string), ConcurrentQueue<Action>> pendingUpdates = [];
    readonly ConcurrentDictionary<(string, string), bool> sharedUids = [];
    readonly ConcurrentDictionary<(string, string), AllureContext> testScopes = [];

    public bool TryGetContext(SessionUid session, string contextUid, [NotNullWhen(true)] out AllureContext? context) =>
        this.contexts.TryGetValue((session.Value, contextUid), out context);

    public bool TryGetPendingUpdates(
        SessionUid session,
        string contextUid,
        [NotNullWhen(true)] out ConcurrentQueue<Action>? updates
    ) =>
        this.pendingUpdates.TryGetValue((session.Value, contextUid), out updates);

    public void SetContext(SessionUid session, string contextUid, AllureContext context)
    {
        this.contexts[(session.Value, contextUid)] = context;
        this.ConsumePendingUpdates(session, contextUid);
    }

    public void ConsumePendingUpdates(SessionUid session, string contextUid)
    {
        if (this.pendingUpdates.TryRemove((session.Value, contextUid), out var updates))
        {
            foreach (var update in updates)
            {
                this.UpdateContext(session, contextUid, update);
            }
        }
    }

    public void AddPendingUpdate(SessionUid session, string contextUid, Action update)
    {
        if (this.TryGetPendingUpdates(session, contextUid, out var updates))
        {
            updates.Enqueue(update);
        }
        else
        {
            this.pendingUpdates[(session.Value, contextUid)] = new([update]);
        }
    }

    public void RemoveContext(SessionUid session, string contextUid)
    {
        this.contexts.TryRemove((session.Value, contextUid), out var _);
    }

    public void CaptureContext(SessionUid session, string contextUid)
    {
        this.SetContext(session, contextUid, lifecycle.Context);
    }

    public void InheritContext(SessionUid session, string contextUid, string? parentContextUid, Action init)
    {
        if (parentContextUid is not null)
        {
            if (this.TryGetContext(session, parentContextUid, out var parentContext))
            {
                this.SetContext(
                    session,
                    contextUid,
                    lifecycle.RunInContext(parentContext, init)
                );
            }
            else
            {
                // TODO: Cover with tests
                this.AddPendingUpdate(session, parentContextUid, () =>
                {
                    init();
                    this.SetContext(session, contextUid, lifecycle.Context);
                });
            }
        }
        else
        {
            this.SetContext(
                session,
                contextUid,
                lifecycle.RunInContext(new(), init)
            );
        }
    }

    public void UpdateContext(SessionUid session, string contextUid, Action update)
    {
        if (this.TryGetContext(session, contextUid, out var context))
        {
            this.SetContext(
                session,
                contextUid,
                lifecycle.RunInContext(context, update)
            );
        }
        else
        {
            // TODO: Cover with tests
            this.AddPendingUpdate(session, contextUid, update);
        }
    }

    public void ReleaseContext(SessionUid session, string contextUid, Action commit)
    {
        if (this.TryGetContext(session, contextUid, out var context))
        {
            lifecycle.RunInContext(context, commit);
        }
    }

    public void ReleaseScopeContext(SessionUid session, string contextUid, Action commit)
    {
        var key = (session.Value, contextUid);
        this.testScopes.TryRemove(key, out _);
        this.sharedUids.TryRemove(key, out _);

        this.ReleaseContext(session, contextUid, commit);
    }

    public void MakeUidShared(SessionUid session, string uid)
    {
        this.sharedUids[(session.Value, uid)] = true;
    }

    public void RemoveTestContext(SessionUid session, string testUid)
    {
        if (this.sharedUids.TryRemove((session.Value, testUid), out _))
        {
            // A scope with the same Uid is active. The context will be removed via AllureScopeStopMessage.
            // We need to update the context to make sure it has no test result in it.
            this.CaptureContext(session, testUid);
        }
        else
        {
            // If no scope with the same Uid is active, we don't need the context anymore
            this.RemoveContext(session, testUid);
        }
    }

    public void AssociateTestsWithScope(SessionUid session, string scopeUid, IEnumerable<string> testUids)
    {
        if (!this.TryGetContext(session, scopeUid, out var scope))
        {
            return;
        }

        // Remember the association for future test results
        foreach (var testUid in testUids)
        {
            this.testScopes[(session.Value, testUid)] = scope;
        }

        // Move the current tests to the scope
        foreach (var testUid in testUids)
        {
            string? testUuid = null;
            if (this.TryGetContext(session, testUid, out var testContext))
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

    public void TryEnterTestScope(SessionUid session, string testUid)
    {
        if (!this.sharedUids.ContainsKey((session.Value, testUid))
            && this.testScopes.TryGetValue((session.Value, testUid), out var scope))
        {
            lifecycle.RestoreContext(scope);
        }
    }
}
