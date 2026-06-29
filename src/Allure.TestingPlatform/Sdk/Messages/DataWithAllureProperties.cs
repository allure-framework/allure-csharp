using System.Collections.Generic;
using System.Linq;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Runtime;
using Allure.TestingPlatform.Sdk.Runtime.Correlation;

namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class DataWithAllureProperties(
    string displayName,
    string description,
    CorrelationUid correlationUid
) :
    DataWithCorrelationUid(displayName, description, correlationUid)
{
    public List<IAllureProperty> Properties { get; init; } = [];

    public void ApplyProperties<T>(ReadyAllureTestingPlatformRuntime allureState, T target)
    {
        foreach (var property in this.Properties.OfType<IAllureProperty<T>>())
        {
            property.Apply(allureState, target);
        }
    }
}