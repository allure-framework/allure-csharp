using System;

namespace Allure.Sdk.Registration;

/// <summary>
/// Stores a value that is assigned once after dependent services have been
/// constructed.
/// </summary>
/// <typeparam name="T">The referenced value type.</typeparam>
public sealed class LateBoundReference<T> : IReadOnlyLateBoundReference<T>
{
    T? value = default;

    /// <inheritdoc/>
    public bool IsBound => this.value is not null;

    /// <inheritdoc/>
    public T Value => this.value ?? throw new InvalidOperationException(
        $"{typeof(T).Name} has not been bound yet."
    );

    /// <summary>
    /// Assigns the referenced value.
    /// </summary>
    /// <param name="value">The value to assign.</param>
    /// <exception cref="ArgumentNullException">
    /// <paramref name="value"/> is <see langword="null"/>.
    /// </exception>
    /// <exception cref="InvalidOperationException">
    /// The reference has already been assigned.
    /// </exception>
    public void Bind(T value)
    {
        if (this.IsBound)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} is already bound."
            );
        }

        this.value = value ?? throw new ArgumentNullException(nameof(value));
    }

    /// <inheritdoc/>
    public IReadOnlyLateBoundReference<TResult> Select<TResult>(Func<T, TResult> selector) =>
        new LazyProjectedLateBoundReference<T, TResult>(this, selector);

    private sealed class LazyProjectedLateBoundReference<TSource, TResult>(
        IReadOnlyLateBoundReference<TSource> source,
        Func<TSource, TResult> selector
    ) :
        IReadOnlyLateBoundReference<TResult>
    {
        readonly Lazy<TResult> result = new(() => selector(source.Value));

        public bool IsBound => source.IsBound;

        public TResult Value => result.Value;

        public IReadOnlyLateBoundReference<TNextResult> Select<TNextResult>(
            Func<TResult, TNextResult> nextSelector
        ) =>
            new LazyProjectedLateBoundReference<TResult, TNextResult>(this, nextSelector);
    }
}
