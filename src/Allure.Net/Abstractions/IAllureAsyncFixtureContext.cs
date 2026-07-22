using System.Threading;
using System.Threading.Tasks;
using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// Provides an async API tied to a specific fixture.
/// </summary>
public interface IAllureAsyncFixtureContext : IAllureOperationContext
{
    /// <summary>
    /// Sets the name of the fixture associated with this context.
    /// </summary>
    /// <param name="newName">The new name of the fixture.</param>
    /// <param name="cancellationToken">A canellation token</param>
    Task SetNameAsync(string newName, CancellationToken cancellationToken);

    /// <summary>
    /// Adds a parameter with the specified text value to the fixture.
    /// </summary>
    /// <param name="parameter">A parameter to add.</param>
    /// <param name="cancellationToken">A canellation token</param>
    /// <remarks>
    /// The value is used as-is.
    /// </remarks>
    Task AddParameterAsync(Parameter parameter, CancellationToken cancellationToken);
}
