using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// A message that updates an Allure execution item.
/// </summary>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="contextUid">The identifier of the execution-item context to update.</param>
public sealed class AllureExecutableItemUpdateMessage(
    CorrelationUid correlationUid,
    IAllureExecutionStateUid contextUid
) :
    AllureModelUpdateMessage(
        "Allure execution item update",
        "This message reports that an Allure execution item needs to be updated.",
        correlationUid,
        contextUid
    ),
    IAllureModelOperationMessage
{
    /// <inheritdoc />
    public override void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime)
    {
        allureRuntime.ModelApi.UpdateCurrentExecutableItem((item) =>
        {
            this.ApplyProperties(allureRuntime, item);
        });
    }
}
