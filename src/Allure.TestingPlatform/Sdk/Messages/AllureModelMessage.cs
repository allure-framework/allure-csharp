using System.Collections.Generic;
using System.Linq;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Correlation;
using Allure.TestingPlatform.Sdk.Properties;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Messages;

/// <summary>
/// Base class for messages that apply properties to an Allure model object.
/// </summary>
public abstract class AllureModelMessage(
    string displayName,
    string description,
    CorrelationUid correlationUid
) :
    AllureCorrelatedMessage(displayName, description, correlationUid)
{
    /// <summary>
    /// Gets the properties applied by the message.
    /// </summary>
    public List<IAllureProperty> Properties { get; init; } = [];

    /// <summary>
    /// Applies all properties that support the specified model type.
    /// </summary>
    public void ApplyProperties<T>(
        IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> allureRuntime,
        T target
    )
    {
        foreach (var property in this.Properties.OfType<IAllureProperty<T>>())
        {
            property.Apply(allureRuntime, target);
        }
    }
}
