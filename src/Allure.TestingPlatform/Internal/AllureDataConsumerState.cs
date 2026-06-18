using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.CodeAnalysis;
using Allure.Net.Commons;

namespace Allure.TestingPlatform.Internal;

internal class AllureDataConsumerState(AllureLifecycle lifecycle)
{
    readonly Dictionary<string, AllureMtpSessionState> sessions = [];

    public void CreateSessionState(string correlationUid)
    {
        if (this.sessions.ContainsKey(correlationUid))
        {
            throw new InvalidOperationException(
                $"A state for session {correlationUid} already exists."
            );
        }

        this.sessions[correlationUid] = new(lifecycle);
    }

    public void RemoveSessionState(string correlationUid)
    {
        if (!this.sessions.Remove(correlationUid))
        {
            throw new InvalidOperationException(
                $"No state for session {correlationUid} exists."
            );
        }
    }

    public bool TryGetContext(
        string correlationUid,
        string contextUid,
        [NotNullWhen(true)] out AllureContext? context
    )
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            return state.TryGetContext(contextUid, out context);
        }

        context = default;
        return false;
    }

    public void MakeUidShared(string correlationUid, string contextUid)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.MakeUidShared(contextUid);
        }
    }

    public void TryEnterTestScope(string correlationUid, string contextUid)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.TryEnterTestScope(contextUid);
        }
    }

    public void SetContext(string correlationUid, string contextUid, AllureContext context)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.SetContext(contextUid, context);
        }
    }

    public void RemoveTestContext(string correlationUid, string contextUid)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.RemoveTestContext(contextUid);
        }
    }

    public void InheritContext(
        string correlationUid,
        string contextUid,
        string? parentContextUid,
        Action init
    )
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.InheritContext(contextUid, parentContextUid, init);
        }
    }

    public void UpdateContext(string correlationUid, string contextUid, Action update)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.UpdateContext(contextUid, update);
        }
    }

    public void ReleaseContext(string correlationUid, string contextUid, Action commit)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.ReleaseContext(contextUid, commit);
            if (state.IsEmpty)
            {
                this.sessions.Remove(correlationUid);
            }
        }
    }

    public void ReleaseScopeContext(string correlationUid, string contextUid, Action commit)
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.ReleaseScopeContext(contextUid, commit);
            if (state.IsEmpty)
            {
                this.sessions.Remove(correlationUid);
            }
        }
    }

    public void AssociateTestsWithScope(
        string correlationUid,
        string scopeUid,
        ImmutableArray<string> testUids
    )
    {
        if (this.TryGetSessionState(correlationUid, out var state))
        {
            state.AssociateTestsWithScope(scopeUid, testUids);
        }
    }

    bool TryGetSessionState(
        string correlationUid,
        [NotNullWhen(true)] out AllureMtpSessionState state
    )
    {
        if (!this.sessions.TryGetValue(correlationUid, out state))
        {
            this.sessions[correlationUid] = state = new(lifecycle);
        }
        return true;
    }
}
