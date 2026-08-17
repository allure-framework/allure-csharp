using System.Collections.Generic;
using Allure.Model;
using Allure.TestingPlatform.Configuration;
using Allure.TestingPlatform.Sdk.Runtime;

namespace Allure.TestingPlatform.Sdk.Properties;

/// <summary>
/// Adds parameters to an Allure test, step, or fixture.
/// </summary>
/// <typeparam name="TModel">The type of model object to update.</typeparam>
/// <param name="parameters">The parameters to add.</param>
public sealed class AllureParametersProperty<TModel>(IEnumerable<Parameter> parameters) :
    IAllureProperty<TModel>

    where TModel : ExecutableItem
{
    /// <summary>
    /// Gets the parameters to add.
    /// </summary>
    public List<Parameter> Parameters { get; } = [..parameters];

    /// <inheritdoc />
    public void Apply(IAllureTestingPlatformRuntime<AllureTestingPlatformConfiguration> _, TModel target)
    {
        target.Parameters.AddRange(this.Parameters);
    }
}
