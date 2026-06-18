using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk;

namespace Allure.TestingPlatform.Internal;

internal class AllureDataConsumerState(AllureLifecycle lifecycle)
{
    readonly Dictionary<CorrelationUid, AllureMtpSessionState> sessions = [];

    public AllureMtpSessionState GetOrCreateSessionState(CorrelationUid correlationUid)
    {
        if (!this.sessions.TryGetValue(correlationUid, out var state))
        {
            this.sessions[correlationUid] = state = new(lifecycle);
        }
        return state;
    }
}
