using System;

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

    /// <summary>
    /// Creates a read-only reference whose value is projected from this
    /// reference when first accessed.
    /// </summary>
    /// <typeparam name="TResult">The projected value type.</typeparam>
    /// <param name="selector">
    /// The function used to project the assigned source value.
    /// </param>
    /// <returns>
    /// A reference that is bound when this reference is bound and lazily
    /// evaluates the projection once.
    /// </returns>
    IReadOnlyLateBoundReference<TResult> Select<TResult>(Func<T, TResult> selector);
}
