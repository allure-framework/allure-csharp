using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that the integration has finished publishing operations for a test
/// execution.
/// </summary>
/// <remarks>
/// <para>
/// This message completes the integration-controlled execution stream identified
/// by <paramref name="executionUid"/>. It does not replace the corresponding
/// Microsoft Testing Platform test-node finish message.
/// </para>
/// <para>
/// Publish this message after all other messages referencing the execution
/// identifier. Once both the execution and its corresponding test-node occurrence
/// have finished, the test execution coordinator may finalize the associated
/// Allure test result.
/// </para>
/// </remarks>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="executionUid">
/// The unique identifier of the test execution that has finished.
/// </param>
public sealed class AllureTestExecutionFinishMessage(
    CorrelationUid correlationUid,
    TestExecutionStateUid executionUid
) :
    AllureCorrelatedMessage(
        "Test execution finished",
        "Signals that no more operations will be published for a test execution.",
        correlationUid
    )
{
    /// <summary>
    /// Gets the unique identifier of the finished test execution.
    /// </summary>
    public TestExecutionStateUid ExecutionUid => executionUid;
}
