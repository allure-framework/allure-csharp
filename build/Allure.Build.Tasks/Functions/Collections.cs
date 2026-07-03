using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Allure.Build.Tasks.Functions;

public static class Collections
{
    public static IEnumerable<R> MapNotEmpty<T, R>(IEnumerable<T> sequence, Func<IEnumerable<T>, R> map) =>
        sequence.ToImmutableArray() is { IsEmpty: false } array
            ? [map(array)]
            : [];

    public static IEnumerable<R> MapNotEmpty<R>(string value, Func<string, R> map) =>
        string.IsNullOrEmpty(value)
            ? []
            : [map(value)];
}