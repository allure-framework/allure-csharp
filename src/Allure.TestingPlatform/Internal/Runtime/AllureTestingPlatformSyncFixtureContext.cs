using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Model;

namespace Allure.TestingPlatform.Internal.Runtime;

sealed class AllureTestingPlatformSyncFixtureContext(
    IAllureInProcessAsyncFixtureContext asyncContext
) :
    IAllureInProcessSyncFixtureContext
{
    public IAllureParameterSerializer ParameterSerializer => asyncContext.ParameterSerializer;

    public void AddParameter(Parameter parameter) =>
        asyncContext.AddParameterAsync(parameter)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public void SetName(string newName) =>
        asyncContext.SetNameAsync(newName)
            .ConfigureAwait(false)
            .GetAwaiter()
            .GetResult();

    public bool TryReadFixtureResult<TResult>(
        Func<FixtureResult, TResult> read,
        [MaybeNullWhen(false)] out TResult value
    ) =>
        asyncContext.TryReadFixtureResult(read, out value);

    public void UpdateFixtureResult(Action<FixtureResult> update) =>
        asyncContext.UpdateFixtureResult(update);
}
