using System;

namespace Allure.Sdk.Serialization;

/// <summary>
/// Serializes values of type <typeparamref name="T"/> by invoking a delegate.
/// </summary>
/// <typeparam name="T">The supported value type.</typeparam>
/// <param name="serialize">The serialization delegate.</param>
public class DelegateParameterSerializationRule<T>(Func<T, string> serialize) :
    TypedParameterSerializationRule<T>
{
    /// <summary>
    /// Gets the serialization delegate.
    /// </summary>
    public Func<T, string> Delegate => serialize;

    /// <inheritdoc/>
    protected override string Serialize(T value) => serialize(value);
}

/// <summary>
/// Creates delegate-backed parameter serialization rules.
/// </summary>
public static class DelegateParameterSerializationRule
{
    /// <summary>
    /// Creates a serialization rule backed by the specified delegate.
    /// </summary>
    public static DelegateParameterSerializationRule<T> Create<T>(Func<T, string> serialize) =>
        new(serialize);
}
