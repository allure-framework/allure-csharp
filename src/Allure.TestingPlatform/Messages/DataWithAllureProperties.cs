using System.Collections.Generic;
using System.Linq;
using Allure.TestingPlatform.Properties;
using Microsoft.Testing.Platform.Extensions.Messages;
using Microsoft.Testing.Platform.TestHost;

namespace Allure.TestingPlatform.Messages;

public abstract class DataWithAllureProperties(
    string displayName,
    string description,
    SessionUid sessionUid
) : IData
{
    public string DisplayName => displayName;

    public string? Description => description;

    public SessionUid Session => sessionUid;

    public List<IAllureProperty> Properties { get; init; } = [];

    public void ApplyProperties<T>(IAllureInfrastructure allure, T target)
    {
        foreach (var property in this.Properties.OfType<IAllureProperty<T>>())
        {
            property.Apply(allure, target);
        }
    }
}