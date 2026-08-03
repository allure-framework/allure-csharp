using Allure.Sdk.Configuration;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Registration;

/// <summary>
/// Represents a prepared, single-use Allure runtime registration.
/// </summary>
/// <remarks>
/// A plan is produced after configuration has been resolved and registration
/// hooks have run, but before the runtime has been constructed or its endpoint
/// has been installed.
/// </remarks>
/// <typeparam name="TConfiguration">The runtime configuration type.</typeparam>
/// <typeparam name="TRuntime">The runtime type.</typeparam>
public interface IAllureRuntimeRegistrationPlan<TConfiguration, TRuntime>
    where TConfiguration : AllureConfiguration
    where TRuntime : IAllureRuntime<TConfiguration>
{
    /// <summary>
    /// Gets the resolved configuration that will be used to construct the
    /// runtime.
    /// </summary>
    TConfiguration Configuration { get; }

    /// <summary>
    /// A reference to a runtime registration that becomes available after <see cref="Build"/>
    /// is called.
    /// </summary>
    IReadOnlyLateBoundReference<IAllureRuntimeRegistration<TRuntime>> RegistrationReference { get; }

    /// <summary>
    /// Constructs the runtime and installs its configured endpoint.
    /// </summary>
    /// <returns>
    /// A registration that provides the constructed runtime and removes its
    /// in-process endpoint when disposed.
    /// </returns>
    /// <exception cref="System.InvalidOperationException">
    /// The runtime has already been built from this plan.
    /// </exception>
    IAllureRuntimeRegistration<TRuntime> Build();
}
