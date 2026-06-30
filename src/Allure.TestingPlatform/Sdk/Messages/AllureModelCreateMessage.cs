using Allure.TestingPlatform.Sdk.ContextIdentifiers;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that create an Allure lifecycle context.
/// </summary>
public abstract class AllureModelCreateMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureContextUid contextUid,
    IAllureContextUid? parentContextUid
) :
    AllureModelMessage(displayName, description, correlationUid),
    IAllureModelOperationMessage
{
    /// <summary>
    /// Gets the context identifier created by the message.
    /// </summary>
    public IAllureContextUid ContextUid { get; } = contextUid;

    /// <summary>
    /// Gets the parent context identifier, if one exists.
    /// </summary>
    public IAllureContextUid? ParentContextUid { get; } = parentContextUid;

    /// <inheritdoc />
    public abstract void ApplyTo(LiveAllureTestingPlatformRuntime allureRuntime);
}
