using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports updates for an active Allure step.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="stepUid">The identifier of the step context to update.</param>
public sealed class AllureStepUpdateMessage(
    CorrelationUid correlationUid,
    StepExecutionStateUid stepUid
) :
    AllureModelUpdateMessage(
        "Allure step result update",
        "This message reports that some data needs to be associated with an Allure step result.",
        correlationUid,
        stepUid
    )
{
    /// <summary>
    /// Gets the step context identifier.
    /// </summary>
    public StepExecutionStateUid StepUid { get; } = stepUid;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        allureRuntime.ModelApi.UpdateStepResult((step) =>
        {
            this.ApplyProperties(allureRuntime, step);
        });
    }
}
