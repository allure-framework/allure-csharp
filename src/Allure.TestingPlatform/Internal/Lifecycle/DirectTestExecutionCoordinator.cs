using System;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Internal.Lifecycle;

sealed class DirectTestExecutionCoordinator : ITestExecutionCoordinator
{
    public void BindTestExecution(TestExecutionStateUid testNodeUid, TestExecutionStateUid executionUid)
    {
    }

    public void FinishTestExecution(TestExecutionStateUid executionUid)
    {
    }

    public void FinishTestNode(TestExecutionStateUid testNodeUid, Action finish) =>
        finish();

    public void Route(TestExecutionStateUid uid, Action<TestExecutionStateUid> operation) =>
        operation(uid);

    public void StartTestNode(TestExecutionStateUid testNodeUid, Action start) =>
        start();

    public static DirectTestExecutionCoordinator Instance { get; } = new();
}
