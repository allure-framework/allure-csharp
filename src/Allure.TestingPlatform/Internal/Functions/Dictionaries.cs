using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Allure.TestingPlatform.Internal.Functions;

static class Dictionaries
{
    public static TValue RemoveAndGet<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        TKey key,
        TValue fallback
    )
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            dictionary.Remove(key);
            return value;
        }

        return fallback;
    }

    public static bool TryRemoveAndGet<TKey, TValue>(
        IDictionary<TKey, TValue> dictionary,
        TKey key,
        [MaybeNullWhen(false)] out TValue value
    )
    {
        if (dictionary.TryGetValue(key, out value))
        {
            dictionary.Remove(key);
            return true;
        }

        return false;
    }
}
