using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TUnit.Assertions.Core;

namespace Allure.Testing.Internal;

internal static class NarrowingFunctions
{
    public record class CriteriaMatchResult(int Position);
    public record class CriteriaMatchSuccess<T>(T Value, int Position) : CriteriaMatchResult(Position);
    public record class CriteriaMatchFailure(string? Expected, string? Actual, int Position) : CriteriaMatchResult(Position);

    public class State<T>
    {
        public List<CriteriaMatchSuccess<T>> matches = [];
        public  List<CriteriaMatchFailure> failures = [];
    }

    public static AssertionContext<TItem> MapToSingle<TCollection, TItem>(
        AssertionContext<TCollection> context
    )
        where TCollection : IReadOnlyList<TItem>

        =>
            context.Map(c => c switch
        {
            [var single] => single,
            [] => throw new InvalidOperationException("nothing was found"),
            not null => throw new InvalidOperationException($"{c.Count} were received"),
            null => throw new InvalidOperationException("the collection was null"),
        });

    public static AssertionContext<TItem> MapByIndex<TCollection, TItem>(
        AssertionContext<TCollection> context,
        string itemDescription,
        int index
    )
        where TCollection : IReadOnlyList<TItem>

        =>
            context.Map(c => c switch
            {
                null => throw new InvalidOperationException("the collection was null"),
                { Count: var count } =>
                    count > index
                        ? c[index]
                        : throw new InvalidOperationException(
                            $"the collection has only {count} {itemDescription}s"),
            });


    public static AssertionContext<TItem> MapByCriteria<TCollection, TItem>(
        AssertionContext<TCollection> context,
        Func<IAssertionSource<TItem>, IAssertion?> criteria,
        string itemDescription,
        State<TItem> state
    )
        where TCollection : IReadOnlyList<TItem>

         =>
            context.Map(GetMapper<TCollection, TItem>(criteria, itemDescription, state));

    public static Func<TCollection?, Task<TItem?>> GetMapper<TCollection, TItem>(
        Func<IAssertionSource<TItem>, IAssertion?> criteria,
        string itemDescription,
        State<TItem> state
    )
        where TCollection : IReadOnlyList<TItem>

        =>
            async (coll) => await MapToSingleItemAsync(coll, criteria, itemDescription, state);

    public static OrContinuation<T> GetThrowingOr<T>() =>
        throw new NotImplementedException(
            $"Narrowing assertions don't support Or continuations. "
                + "Switch to a non-narrowing alternative");

    public static async Task<AssertionResult> CheckAsync<T>(
        EvaluationMetadata<T> metadata
    ) =>
        metadata is { Exception.Message: var message }
            ? await Task.FromResult(AssertionResult.Failed(message))
            : await Task.FromResult(AssertionResult.Passed);

    public static string GetSingleExpectation(string itemDescription) =>
        $"a single {itemDescription}";

    public static string GetByIndexExpectation(string itemDescription, int index) =>
        $"{itemDescription} at index {index}";

    public static string GetByCriteriaExpectation<T>(string itemDescription, State<T> state) =>
        state.failures
            .Select(f => f.Expected)
            .Where(e => e is not null)
            .MaxBy(e => e!.Length) is { } elementExpectation
                ? $"exactly one {itemDescription} with {elementExpectation}"
                : $"exactly one {itemDescription} matching the provided criteria";

    static async Task<T> MapToSingleItemAsync<T>(
        IReadOnlyList<T>? sequence,
        Func<IAssertionSource<T>, IAssertion?> criteria,
        string itemDescription,
        State<T> state
    ) =>
        sequence switch
        {
            null => throw new InvalidOperationException("the collection was null"),
            _ => await ApplyCriteriaAsync(
                sequence,
                CreateCriteriaMatcher(criteria, itemDescription),
                state
            ) switch
            {
                { matches: [{ Value: var match }] } => match,

                { matches: [], failures: [] } =>
                    throw new InvalidOperationException("the collection was empty"),

                { matches: [.. { Count: var firstLength } head, { Position: var last }] } =>
                    throw new InvalidOperationException(
                        $"{itemDescription}s "
                            + $"{string.Join(", ", head.Select(m => m.Position))} and {last} matched the criteria"),

                { failures: var failure } =>
                    throw new InvalidOperationException(
                        $"no {itemDescription}s matched the criteria:{Environment.NewLine}"
                            + FormatMismatches(itemDescription, failure)
                    ),
            }
        };

    static async Task<State<T>> ApplyCriteriaAsync<T>(
        IEnumerable<T> sequence,
        Func<T, int, Task<AssertionResult>> criteriaMatcher,
        State<T> state
    )
    {
        int i = 0;
        foreach (var element in sequence)
        {
            var result = await criteriaMatcher(element, i++);
            if (result.IsPassed)
            {
                state.matches.Add(new(element, i));
            }
            else
            {
                var (expected, actual) = ExtractExpectedAndActual(result.Message, 0);
                state.failures.Add(new(expected, actual, i));
            }
        }
        return state;
    }

    static Func<T, int, Task<AssertionResult>> CreateCriteriaMatcher<T>(
        Func<IAssertionSource<T>, IAssertion?> criteria,
        string itemDescription
    ) =>
        async (item, i) =>
        {
            var (result, _) = await AssertionFunctions.ExecuteInlineAssertionAsync(
                item,
                $"{itemDescription}s[{i}]",
                criteria);
            return result;
        };

    static string FormatMismatches(string itemDescription, IEnumerable<CriteriaMatchFailure> failures) =>
        string.Join(
            Environment.NewLine,
            failures.Select(e => FormatMismatchLine(itemDescription, e)));

    public static (string? expected, string? actual) ExtractExpectedAndActual(string message, int ident)
    {
        var lines = message
            .Split(Environment.NewLine, StringSplitOptions.RemoveEmptyEntries)
            .SelectMany(l => l.Split('\n'))
            .Select(l => l.Trim())
            .Where(l => l.Length > 0)
            .ToArray();

        var expectedIdentPrefix = new string(' ', ident * 2);
        var expected = lines.TakeWhile(l => !l.StartsWith("but ")).ToList() switch
        {
            [] => null,
            [var single] => single,
            [var first, var second, .. var rest]
                when IsJsonPropertyDefinedAssertion(first) && second.StartsWith("and ")
                    => $"{first} {TryMakeBeing(second[4..])}{IdentBlock(expectedIdentPrefix, rest)}",
            [var first, .. var rest]
                => $"{first}{IdentBlock(expectedIdentPrefix, rest)}",
        } switch
        {
            { } notNull when notNull.StartsWith("Expected ") => notNull[9..],
            var otherwise => otherwise,
        };

        var actualLines = lines
            .SkipWhile(l => !l.StartsWith("but "))
            .TakeWhile(l => !l.StartsWith("at Assert.That("))
            .ToList();

        var actualIdentPrefix = new string(' ', ident * 2 + 4);
        var actual = actualLines switch
        {
            [] => message,
            [var first, .. var rest] =>
                $"{first[4..]}{IdentBlock(actualIdentPrefix, rest)}"
        };

        return (expected, actual);

        static string IdentBlock(string prefix, IEnumerable<string> lines) =>
            string.Join("", lines.Select(l => $"{Environment.NewLine}{prefix}{l}"));

        static bool IsJsonPropertyDefinedAssertion(string message)
        {
            return message.StartsWith("Expected \"") && message.EndsWith('"');
        }
    }

    static string FormatMismatchLine(string itemDescription, CriteriaMatchFailure mismatch) =>
        $"  - {itemDescription} {mismatch.Position}: {mismatch.Actual}";

    static string TryMakeBeing(string v) => v.StartsWith("to be ") ? $"being{v[5..]}" : v;
}
