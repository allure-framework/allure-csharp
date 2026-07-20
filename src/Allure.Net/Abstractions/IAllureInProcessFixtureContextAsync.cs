using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;


public interface IAllureInProcessFixtureContextAsync : IAllureFixtureContextAsync
{
    void UpdateFixtureResult(Action<FixtureResult> update);

    bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, [MaybeNullWhen(false)] out TResult value);
}
