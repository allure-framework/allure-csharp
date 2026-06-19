using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Allure.TestingPlatform.Functions;

static class CollectionAlgorithms
{
    public static IEnumerable<TResult> MergeSortedByItem1<TFirst, TSecond, TResult>(
        IEnumerable<(int key, TFirst value)> first,
        IEnumerable<(int key, TSecond value)> second
    )
        where TFirst : TResult
        where TSecond : TResult
    {
        using var e1 = first.GetEnumerator();
        using var e2 = second.GetEnumerator();

        var has1 = e1.MoveNext();
        var has2 = e2.MoveNext();

        while (has1 && has2)
        {
            if (e1.Current.key < e2.Current.key)
            {
                yield return e1.Current.value;
                has1 = e1.MoveNext();
            }
            else
            {
                yield return e2.Current.value;
                has2 = e2.MoveNext();
            }
        }

        while (has1)
        {
            yield return e1.Current.value;
            has1 = e1.MoveNext();
        }

        while (has2)
        {
            yield return e2.Current.value;
            has2 = e2.MoveNext();
        }
    }

    public static V RemoveAndGet<K, V>(IDictionary<K, V> dictionary, K key, V fallback)
    {
        if (dictionary.TryGetValue(key, out var value))
        {
            dictionary.Remove(key);
            return value;
        }
        return fallback;
    }

    public static bool TryRemoveAndGet<K, V>(
        IDictionary<K, V> dictionary,
        K key,
        [MaybeNullWhen(false)] out V value
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