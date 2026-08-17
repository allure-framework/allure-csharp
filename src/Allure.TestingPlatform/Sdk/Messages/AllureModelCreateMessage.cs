using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.ExecutionState;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that create an Allure lifecycle context.
/// </summary>
/// <param name="displayName">The message display name.</param>
/// <param name="description">The message description.</param>
/// <param name="correlationUid">The identifier used to correlate the message.</param>
/// <param name="contextUid">The identifier of the context to create.</param>
/// <param name="parentContextUid">The parent context identifier, if one exists.</param>
public abstract class AllureModelCreateMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid,
    IAllureExecutionStateUid contextUid,
    IAllureExecutionStateUid? parentContextUid
) :
    AllureModelMessage(displayName, description, correlationUid),
    IAllureModelOperationMessage
{
    /// <summary>
    /// Gets the context identifier created by the message.
    /// </summary>
    public IAllureExecutionStateUid ContextUid { get; } = contextUid;

    /// <summary>
    /// Gets the parent context identifier, if one exists.
    /// </summary>
    public IAllureExecutionStateUid? ParentContextUid { get; } = parentContextUid;

    /// <inheritdoc />
    public abstract void ApplyTo(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime);
}
