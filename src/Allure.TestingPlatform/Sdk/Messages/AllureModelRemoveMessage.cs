using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that complete and remove an Allure lifecycle context.
/// </summary>
/// <param name="displayName">The message display name.</param>
/// <param name="description">The message description.</param>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="contextUid">The identifier of the context to remove.</param>
public abstract class AllureModelRemoveMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureExecutionStateUid contextUid
) :
    AllureModelMessage(displayName, description, correlationUid),
    IAllureModelOperationMessage
{
    /// <summary>
    /// Gets the context identifier removed by the message.
    /// </summary>
    public IAllureExecutionStateUid ContextUid { get; } = contextUid;

    /// <inheritdoc />
    public abstract void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime);
}
