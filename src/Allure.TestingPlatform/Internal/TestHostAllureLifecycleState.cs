using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Internal;

internal class TestHostAllureLifecycleState(AllureLifecycle lifecycle)
{
    readonly Dictionary<CorrelationUid, SessionLifecycleState> sessions = [];

    public SessionLifecycleState GetOrCreateSessionState(CorrelationUid correlationUid)
    {
        if (!this.sessions.TryGetValue(correlationUid, out var state))
        {
            this.sessions[correlationUid] = state = new(lifecycle);
        }
        return state;
    }

    public void RemoveSession(CorrelationUid correlationUid)
    {
        this.sessions.Remove(correlationUid);
    }
}
