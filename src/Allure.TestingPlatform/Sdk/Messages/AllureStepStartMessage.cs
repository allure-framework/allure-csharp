using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Reports that an Allure step has started.
/// </summary>
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
