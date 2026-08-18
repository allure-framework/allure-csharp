using System.Collections.Generic;

namespace Allure.TestingPlatform.Internal.Functions;

static class SortedSequences
{
    public static IEnumerable<TResult> MergeByKey<TFirst, TSecond, TResult>(
        IEnumerable<(int key, TFirst value)> first,
        IEnumerable<(int key, TSecond value)> second
    )
        where TFirst : TResult
        where TSecond : TResult
    {
        using var firstEnumerator = first.GetEnumerator();
        using var secondEnumerator = second.GetEnumerator();

        var hasFirst = firstEnumerator.MoveNext();
        var hasSecond = secondEnumerator.MoveNext();

        while (hasFirst && hasSecond)
        {
            if (firstEnumerator.Current.key < secondEnumerator.Current.key)
            {
                yield return firstEnumerator.Current.value;
                hasFirst = firstEnumerator.MoveNext();
            }
            else
            {
                yield return secondEnumerator.Current.value;
                hasSecond = secondEnumerator.MoveNext();
            }
        }

        while (hasFirst)
        {
            yield return firstEnumerator.Current.value;
            hasFirst = firstEnumerator.MoveNext();
        }

        while (hasSecond)
        {
            yield return secondEnumerator.Current.value;
            hasSecond = secondEnumerator.MoveNext();
        }
    }
}
