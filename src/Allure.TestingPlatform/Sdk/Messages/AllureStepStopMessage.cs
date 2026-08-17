using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure step has stopped.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="stepUid">The identifier of the step context to stop.</param>
public sealed class AllureStepStopMessage(
    CorrelationUid correlationUid,
    StepExecutionStateUid stepUid
) :
    AllureModelRemoveMessage(
        "Allure step stop",
        "This message reports that an Allure test step has stopped.",
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
        allureRuntime.LifecycleApi.StopStep();
    }
}
