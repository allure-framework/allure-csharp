using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// Provides an API tied to a specific fixture.
/// </summary>
public interface IAllureFixtureContext
{
    /// <summary>
    /// Sets the name of the fixture associated with this context.
    /// </summary>
    /// <param name="newName">The new name of the fixture.</param>
    void SetName(string newName);

    /// <summary>
    /// Adds a parameter with the specified text value to the fixture.
    /// </summary>
    /// <param name="parameter">A parameter to add.</param>
    /// <remarks>
    /// The value is used as-is.
    /// </remarks>
    void AddParameter(Parameter parameter);
}
