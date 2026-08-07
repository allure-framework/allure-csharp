using System.Collections.Generic;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;

namespace Allure.TestingPlatform.Internal;

internal class TestHostAllureLifecycleState(IAllureExecutionContext context)
{
    readonly Dictionary<CorrelationUid, SessionLifecycleState> sessions = [];

    public SessionLifecycleState GetOrCreateSessionState(CorrelationUid correlationUid)
    {
        if (!this.sessions.TryGetValue(correlationUid, out var sessionState))
        {
            this.sessions[correlationUid] = sessionState = new(context);
        }
        return sessionState;
    }

    public void RemoveSession(CorrelationUid correlationUid)
    {
        this.sessions.Remove(correlationUid);
    }
}
