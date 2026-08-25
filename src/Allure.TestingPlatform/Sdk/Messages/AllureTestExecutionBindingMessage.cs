using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.ExecutionState;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Declares a uniquely identified test execution and binds it to a
/// Microsoft Testing Platform test-node identifier.
/// </summary>
/// <remarks>
/// <para>
/// The execution identifier addresses Allure operations associated with one
/// test execution, while the test-node identifier corresponds to the identifier
/// used by Microsoft Testing Platform lifecycle messages.
/// </para>
/// <para>
/// Some integrations reuse test-node identifiers across multiple executions.
/// Bindings that target the same test-node identifier must therefore be published
/// in execution order. Each execution identifier must be unique within its
/// correlated session and must eventually be completed with an
/// <see cref="AllureTestExecutionFinishMessage"/>.
/// </para>
/// </remarks>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="testNodeUid">
/// The identifier used by Microsoft Testing Platform for the corresponding test node.
/// </param>
/// <param name="executionUid">
/// The unique identifier assigned by the integration to this test execution.
/// </param>
public sealed class AllureTestExecutionBindingMessage(
    CorrelationUid correlationUid,
    TestExecutionStateUid testNodeUid,
    TestExecutionStateUid executionUid
) :
    AllureCorrelatedMessage(
        "Test execution UID binding",
        "Declares a unique test execution and binds it to a potentially reused test-node UID.",
        correlationUid
    )
{
    /// <summary>
    /// Gets the Microsoft Testing Platform test-node identifier.
    /// </summary>
    public TestExecutionStateUid TestNodeUid => testNodeUid;

    /// <summary>
    /// Gets the unique integration-provided test execution identifier.
    /// </summary>
    public TestExecutionStateUid ExecutionUid => executionUid;
}
