using System;
using System.Diagnostics.CodeAnalysis;
using Allure.Abstractions;
using Allure.Model;
using Allure.Sdk.Runtime;

namespace Allure.Sdk.Internal.Runtime;

abstract class FixtureOperationContext(IAllureRuntime runtime) :
    OperationContext(runtime),
    IAllureOperationContext
{
    public bool TryReadFixtureResult<T>(
        Func<FixtureResult, T> read,
        [MaybeNullWhen(false)] out T result
    )
    {
        this.EnsureInScope();

        if (this.CurrentState.HasFixture)
        {
            result = this.Runtime.ModelApi.ReadFixtureResult(read);
            return true;
        }

        result = default;
        return false;
    }

    public void UpdateFixtureResult(Action<FixtureResult> update)
    {
        this.EnsureInScope();

        this.Runtime.ModelApi.UpdateFixtureResult(update);
    }

    protected override string ScopingErrorMessage =>
        "The fixture associated with this context has already finished.";
}
