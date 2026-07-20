using Allure.Model;

namespace Allure.Abstractions;

/// <summary>
/// Provides an API tied to a specific step.
/// </summary>
public interface IAllureStepContext
{
    /// <summary>
    /// Sets the name of the step associated with this context.
    /// </summary>
    /// <param name="newName">The new name of the step.</param>
    void SetName(string newName);

    /// <summary>
    /// Adds a parameter with the specified text value to the step.
    /// </summary>
    /// <param name="parameter">A parameter to add.</param>
    /// <remarks>
    /// The value is used as-is.
    /// </remarks>
    void AddParameter(Parameter parameter);
}
