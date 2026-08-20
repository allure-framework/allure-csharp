using System;
using System.Collections.Generic;
using Allure.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal.Lifecycle;

sealed class SessionLifecycleRegistry(
    IAllureExecutionContext context,
    Func<ITestExecutionCoordinator> testExecutionCoordinatorFactory
)
{
    readonly Dictionary<CorrelationUid, SessionLifecycleCoordinator> sessions = [];

    public SessionLifecycleCoordinator GetOrCreate(CorrelationUid correlationUid)
    {
        if (!this.sessions.TryGetValue(correlationUid, out var sessionState))
        {
            this.sessions[correlationUid] = sessionState = new(
                context,
                testExecutionCoordinatorFactory()
            );
        }
        return sessionState;
    }

    public void Remove(CorrelationUid correlationUid)
    {
        this.sessions.Remove(correlationUid);
    }
}
