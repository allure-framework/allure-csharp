using System;

namespace Allure.TestingPlatform.Sdk.ExecutionState;

/// <summary>
/// Coordinates integration-defined test executions with Microsoft Testing
/// Platform test-node lifecycle events.
/// </summary>
/// <remarks>
/// <para>
/// A test-node identifier may be reused for multiple test occurrences, while
/// an execution identifier represents one unique integration-defined execution.
/// A coordinator may invoke callbacks immediately or defer them until it can
/// associate an execution with the corresponding test-node occurrence.
/// </para>
/// <para>
/// Deferred callbacks must be invoked exactly once. Operations routed for the
/// same execution must be invoked in the order in which they were received.
/// Any mutable coordination state must be scoped to the correlated test
/// session for which the coordinator was created.
/// </para>
/// </remarks>
public interface ITestExecutionCoordinator
{
    /// <summary>
    /// Coordinates the start of a test-node occurrence.
    /// </summary>
    /// <remarks>
    /// The coordinator may invoke <paramref name="start"/> immediately or
    /// defer it, for example until an earlier occurrence that uses the same
    /// test-node identifier has completed.
    /// </remarks>
    /// <param name="testNodeUid">
    /// The identifier of the Microsoft Testing Platform test node.
    /// </param>
    /// <param name="start">
    /// The callback that starts the Allure state for the test-node occurrence.
    /// </param>
    void StartTestNode(
        TestExecutionStateUid testNodeUid,
        Action start
    );

    /// <summary>
    /// Coordinates the completion of a test-node occurrence.
    /// </summary>
    /// <remarks>
    /// The coordinator may invoke <paramref name="finish"/> immediately or
    /// defer it until the corresponding integration-defined execution has
    /// also finished.
    /// </remarks>
    /// <param name="testNodeUid">
    /// The identifier of the Microsoft Testing Platform test node.
    /// </param>
    /// <param name="finish">
    /// The callback that finishes the Allure state for the test-node occurrence.
    /// </param>
    void FinishTestNode(
        TestExecutionStateUid testNodeUid,
        Action finish
    );

    /// <summary>
    /// Associates an integration-defined test execution with a Microsoft
    /// Testing Platform test node.
    /// </summary>
    /// <remarks>
    /// The association may be reported before or after the corresponding
    /// test-node lifecycle events. An execution identifier must not be
    /// associated with more than one test-node identifier.
    /// </remarks>
    /// <param name="testNodeUid">
    /// The identifier of the Microsoft Testing Platform test node.
    /// </param>
    /// <param name="executionUid">
    /// The unique identifier of the integration-defined test execution.
    /// </param>
    void BindTestExecution(
        TestExecutionStateUid testNodeUid,
        TestExecutionStateUid executionUid
    );

    /// <summary>
    /// Reports that the integration has finished publishing operations for a
    /// test execution.
    /// </summary>
    /// <remarks>
    /// This method may be called before the execution is bound to a test node.
    /// No operations may be routed for <paramref name="executionUid"/> after
    /// this method is called.
    /// </remarks>
    /// <param name="executionUid">
    /// The unique identifier of the finished test execution.
    /// </param>
    void FinishTestExecution(TestExecutionStateUid executionUid);

    /// <summary>
    /// Routes an operation addressed to an integration-defined execution to
    /// the corresponding Microsoft Testing Platform test node.
    /// </summary>
    /// <remarks>
    /// The coordinator may invoke <paramref name="operation"/> immediately or
    /// defer it until the execution is bound and its test-node occurrence is
    /// active. A direct coordinator may treat <paramref name="uid"/> as both
    /// the execution identifier and the test-node identifier.
    /// </remarks>
    /// <param name="uid">
    /// The unique identifier of the integration-defined test execution.
    /// </param>
    /// <param name="operation">
    /// The operation to invoke with the corresponding test-node identifier.
    /// </param>
    void Route(
        TestExecutionStateUid uid,
        Action<TestExecutionStateUid> operation
    );
}
