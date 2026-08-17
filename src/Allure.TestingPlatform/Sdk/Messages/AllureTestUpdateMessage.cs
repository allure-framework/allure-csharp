using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports updates for an Allure test result.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="testUid">The identifier of the test context to update.</param>
public sealed class AllureTestUpdateMessage(
    CorrelationUid correlationUid,
    TestExecutionStateUid testUid
) :
    AllureModelUpdateMessage(
        "Allure test result update",
        "This message reports that some data needs to be associated with an Allure test result.",
        correlationUid,
        testUid
    )
{
    /// <summary>
    /// Gets the test context identifier.
    /// </summary>
    public TestExecutionStateUid TestUid { get; } = testUid;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        allureRuntime.ModelApi.UpdateTestResult((test) =>
        {
            this.ApplyProperties(allureRuntime, test);
        });
    }
}
