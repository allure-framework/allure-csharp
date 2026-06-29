using System.Collections.Generic;
using System.Linq;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

public abstract class AllureModelMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid
) :
    AllureCorrelatedMessage(displayName, description, correlationUid)
{
    public List<IAllureProperty> Properties { get; init; } = [];

    public void ApplyProperties<T>(LiveAllureTestingPlatformRuntime allureState, T target)
    {
        foreach (var property in this.Properties.OfType<IAllureProperty<T>>())
        {
            property.Apply(allureState, target);
        }
    }
}