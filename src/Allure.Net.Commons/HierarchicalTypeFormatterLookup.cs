using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

#nullable enable

namespace Allure.Net.Commons;

/// <summary>
/// Wraps a dictionary of explicitly registered type formatters and resolves
/// a formatter for a given runtime type by walking its type hierarchy when
/// no formatter is registered for the exact type.
/// </summary>
/// <remarks>
/// Resolution order:
/// <list type="number">
/// <item>The exact type.</item>
/// <item>The type's generic type definition, if it is a closed generic type.</item>
/// <item>Each interface the type implements, in the order returned by <see cref="Type.GetInterfaces" />.</item>
/// <item>The generic type definition of each such interface.</item>
/// <item>Each base class, from the closest to the furthest.</item>
/// <item>The generic type definition of each such base class.</item>
/// </list>
/// Resolved results are cached per runtime type. The cache is invalidated
/// whenever a new formatter is registered.
/// </remarks>
internal sealed class HierarchicalTypeFormatterLookup : IReadOnlyDictionary<Type, ITypeFormatter>
{
    private readonly Dictionary<Type, ITypeFormatter> registered;
    private readonly ConcurrentDictionary<Type, ITypeFormatter?> resolveCache = new();

    public HierarchicalTypeFormatterLookup(Dictionary<Type, ITypeFormatter> registered)
    {
        this.registered = registered;
    }

    /// <summary>
    /// Drops all cached resolution results. Must be called whenever the
    /// underlying registered-formatters dictionary is mutated.
    /// </summary>
    public void Invalidate() => resolveCache.Clear();

    public bool TryGetValue(Type key, out ITypeFormatter value)
    {
        var resolved = resolveCache.GetOrAdd(key, Resolve);
        value = resolved!;
        return resolved is not null;
    }

    private ITypeFormatter? Resolve(Type type)
    {
        if (registered.TryGetValue(type, out var exact))
        {
            return exact;
        }

        if (type.IsGenericType &&
            registered.TryGetValue(type.GetGenericTypeDefinition(), out var genericDefinition))
        {
            return genericDefinition;
        }

        var interfaces = type.GetInterfaces();

        foreach (var i in interfaces)
        {
            if (registered.TryGetValue(i, out var iface))
            {
                return iface;
            }
        }

        foreach (var i in interfaces)
        {
            if (i.IsGenericType &&
                registered.TryGetValue(i.GetGenericTypeDefinition(), out var ifaceGenericDefinition))
            {
                return ifaceGenericDefinition;
            }
        }

        for (var b = type.BaseType; b is not null; b = b.BaseType)
        {
            if (registered.TryGetValue(b, out var baseExact))
            {
                return baseExact;
            }
        }

        for (var b = type.BaseType; b is not null; b = b.BaseType)
        {
            if (b.IsGenericType &&
                registered.TryGetValue(b.GetGenericTypeDefinition(), out var baseGenericDefinition))
            {
                return baseGenericDefinition;
            }
        }

        return null;
    }

    /// <summary>
    /// The explicitly registered types. Unlike <see cref="TryGetValue" />,
    /// this doesn't include every type that could be resolved through them
    /// (e.g., subclasses or implementors).
    /// </summary>
    public IEnumerable<Type> Keys => registered.Keys;

    /// <summary>
    /// The formatters registered for the types in <see cref="Keys" />, in
    /// the same order.
    /// </summary>
    public IEnumerable<ITypeFormatter> Values => registered.Values;

    /// <summary>
    /// The number of explicitly registered types. See <see cref="Keys" />.
    /// </summary>
    public int Count => registered.Count;

    public ITypeFormatter this[Type key] =>
        TryGetValue(key, out var value)
            ? value
            : throw new KeyNotFoundException($"No type formatter is registered for {key}.");

    public bool ContainsKey(Type key) => TryGetValue(key, out _);

    public IEnumerator<KeyValuePair<Type, ITypeFormatter>> GetEnumerator() =>
        registered.GetEnumerator();

    IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();
}
