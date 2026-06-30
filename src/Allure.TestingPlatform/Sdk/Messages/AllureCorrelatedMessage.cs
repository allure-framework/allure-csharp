using Allure.TestingPlatform.Sdk.Correlation;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for Allure.TestingPlatform messages associated with a correlation identifier.
/// </summary>
public abstract class AllureCorrelatedMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid
) : IData
{
    /// <inheritdoc />
    public string DisplayName => displayName;

    /// <inheritdoc />
    public string? Description => description;

    /// <summary>
    /// Gets the correlation identifier used to route the message.
    /// </summary>
    public CorrelationUid CorrelationUid => correlationUid;
}
