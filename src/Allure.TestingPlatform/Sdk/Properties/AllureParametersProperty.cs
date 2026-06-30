using System.Collections.Generic;
using Allure.Net.Commons;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds parameters to an Allure test, step, or fixture.
/// </summary>
public sealed class AllureParametersProperty<TModel>(IEnumerable<Parameter> parameters) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the parameters to add.
    /// </summary>
    public List<Parameter> Parameters { get; } = [..parameters];

    /// <inheritdoc />
    public void Apply(LiveAllureTestingPlatformRuntime _, TModel target)
    {
        target.parameters.AddRange(this.Parameters);
    }
}
