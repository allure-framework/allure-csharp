using System;
using System.Threading;

namespace Allure.Sdk.Registration;

/// <summary>
/// Stores a value that is assigned once after dependent services have been
/// constructed.
/// </summary>
/// <typeparam name="T">The referenced value type.</typeparam>
public sealed class LateBoundReference<T> : IReadOnlyLateBoundReference<T>
{
    sealed class Holder(T value)
    {
        public T Value { get; } = value;
    }

    Holder? holder = null;

    /// <inheritdoc/>
    public bool IsBound => Volatile.Read(ref this.holder) is not null;

    /// <inheritdoc/>
    public T Value => Volatile.Read(ref this.holder) is { Value: T value }
        ? value
        : throw new InvalidOperationException(
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
        if (value is null)
        {
            throw new ArgumentNullException(nameof(value));
        }

        var holder = new Holder(value);

        if (Interlocked.CompareExchange(ref this.holder, holder, null) is not null)
        {
            throw new InvalidOperationException(
                $"{typeof(T).Name} is already bound."
            );
        }
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

public static class LateBoundReference
{
    public static LateBoundReference<T> Bound<T>(T value)
    {
        LateBoundReference<T> reference = new();
        reference.Bind(value);
        return reference;
    }
}
