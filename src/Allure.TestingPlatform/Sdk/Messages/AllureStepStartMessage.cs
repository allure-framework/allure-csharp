using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure step has started.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="parentUid">The identifier of the context that owns the step.</param>
/// <param name="stepUid">The identifier of the step context to create.</param>
/// <param name="stepName">The step name.</param>
public sealed class AllureStepStartMessage(
    CorrelationUid correlationUid,
    IAllureExecutionStateUid parentUid,
    StepExecutionStateUid stepUid,
    string stepName
) :
    AllureModelCreateMessage(
        "Allure step start",
        "This message reports that an Allure step has started.",
        correlationUid,
        stepUid,
        parentUid
    )
{
    /// <summary>
    /// Gets the step context identifier.
    /// </summary>
    public StepExecutionStateUid StepUid { get; } = stepUid;

    /// <summary>
    /// Gets the parent context identifier.
    /// </summary>
    public IAllureExecutionStateUid ParentUid { get; } = parentUid;

    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        StepResult step = new () { Name = stepName };
        this.ApplyProperties(allureRuntime, step);
        allureRuntime.LifecycleApi.StartStep(step);
    }
}
