using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;


/// <summary>
/// Provides direct access to a fixture result in the current process.
/// </summary>
public interface IAllureInProcessFixtureContext : IAllureFixtureContext
{
    /// <summary>
    /// Updates the current fixture result.
    /// </summary>
    void UpdateFixtureResult(Action<FixtureResult> update);

    /// <summary>
    /// Attempts to read a value from the current fixture result.
    /// </summary>
    bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, [MaybeNullWhen(false)] out TResult value);
}
