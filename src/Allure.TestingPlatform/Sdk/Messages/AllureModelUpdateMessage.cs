using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that update an Allure lifecycle context.
/// </summary>
public abstract class AllureModelUpdateMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureContextUid contextUid
) :
    AllureModelMessage(displayName, description, correlationUid),
    IAllureModelOperationMessage
{
    /// <summary>
    /// Gets the context identifier updated by the message.
    /// </summary>
    public IAllureContextUid ContextUid { get; } = contextUid;

    /// <inheritdoc />
    public abstract void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime);
}
