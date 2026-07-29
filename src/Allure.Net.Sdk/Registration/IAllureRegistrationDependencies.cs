using Allure.Abstractions;
using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Provides services available while an Allure runtime is being constructed.
/// </summary>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
public interface IAllureRegistrationDependencies<TConfiguration>
    where TConfiguration : AllureConfiguration
{
    /// <summary>
    /// Gets the resolved runtime configuration.
    /// </summary>
    TConfiguration Configuration { get; }

    /// <summary>
    /// Gets the parameter serializer.
    /// </summary>
    IAllureParameterSerializer ParameterSerializer { get; }

    /// <summary>
    /// Gets a reference that is bound after runtime construction completes.
    /// </summary>
    IReadOnlyLateBoundReference<IAllureRuntime<TConfiguration>> RuntimeReference { get; }
}
