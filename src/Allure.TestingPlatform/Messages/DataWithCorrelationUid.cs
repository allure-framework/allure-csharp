using Allure.TestingPlatform.Sdk;
using Microsoft.Testing.Platform.Extensions.Messages;

namespace Allure.TestingPlatform.Messages;

public abstract class DataWithCorrelationUid(
    string displayName,
    string description,
    CorrelationUid correlationUid
) : IData
{
    public string DisplayName => displayName;

    public string? Description => description;

    public CorrelationUid CorrelationUid => correlationUid;
}