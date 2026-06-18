using System.Collections.Generic;
using System.Linq;
using Allure.TestingPlatform.Sdk.Properties;

namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class DataWithAllureProperties(
    string displayName,
    string description,
    CorrelationUid correlationUid
) :
    DataWithCorrelationUid(displayName, description, correlationUid)
{
    public List<IAllureProperty> Properties { get; init; } = [];

    public void ApplyProperties<T>(IAllureInfrastructure allure, T target)
    {
        foreach (var property in this.Properties.OfType<IAllureProperty<T>>())
        {
            property.Apply(allure, target);
        }
    }
}