namespace Allure.Sdk.Registration;

/// <summary>
/// Exposes a value that is assigned after dependent services are constructed.
/// </summary>
/// <typeparam name="T">The referenced value type.</typeparam>
public interface IReadOnlyLateBoundReference<out T>
{
    /// <summary>
    /// Gets a value indicating whether the reference has been assigned.
    /// </summary>
    bool IsBound { get; }

    /// <summary>
    /// Gets the assigned value.
    /// </summary>
    /// <exception cref="System.InvalidOperationException">
    /// The reference has not yet been assigned.
    /// </exception>
    T Value { get; }
}
