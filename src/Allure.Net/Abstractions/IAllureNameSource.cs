namespace Allure.Abstractions;

/// <summary>
/// Provides an optional Allure operation name format.
/// </summary>
public interface IAllureNameSource
{
    /// <summary>
    /// Gets the name format.
    /// </summary>
    string? Name { get; }
}
