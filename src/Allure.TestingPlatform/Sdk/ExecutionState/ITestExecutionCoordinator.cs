using System;

namespace Allure.TestingPlatform.Sdk.ExecutionState;

public interface ITestExecutionCoordinator
{
    void StartTestNode(
        TestExecutionStateUid testNodeUid,
        Action start
    );

    void FinishTestNode(
        TestExecutionStateUid testNodeUid,
        Action finish
    );

    void BindTestExecution(
        TestExecutionStateUid testNodeUid,
        TestExecutionStateUid executionUid
    );

    void FinishTestExecution(TestExecutionStateUid executionUid);

    void Route(
        TestExecutionStateUid uid,
        Action<TestExecutionStateUid> operation
    );
}
