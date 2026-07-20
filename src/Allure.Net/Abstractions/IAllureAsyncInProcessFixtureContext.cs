using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Model;

namespace Allure.Abstractions;


public interface IAllureAsyncInProcessFixtureContext : IAllureAsyncFixtureContext
{
    void UpdateFixtureResult(Action<FixtureResult> update);

    bool TryReadFixtureResult<TResult>(Func<FixtureResult, TResult> read, [MaybeNullWhen(false)] out TResult value);
}
