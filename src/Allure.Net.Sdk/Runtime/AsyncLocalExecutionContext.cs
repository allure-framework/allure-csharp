using System.Threading;
using Allure.Sdk.Registration;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Provides an Allure runtime context whose current execution state is stored
/// in an <see cref="AsyncLocal{T}"/> field. This allows concurrently
/// executing tests and steps to maintain independent execution states, provided
/// that the <see cref="ExecutionContext"/> is propagated correctly.
/// </summary>
/// <param name="reference">
/// A reference to the Allure runtime associated with this context.
/// </param>
public sealed class AsyncLocalExecutionContext(
    IReadOnlyLateBoundReference<IAllureRuntimeBase> reference
) : AllureExecutionContext(reference)
{
    readonly AsyncLocal<AllureExecutionState> currentState = new() { Value = new() };

    /// <inheritdoc/>
    public override AllureExecutionState CurrentState
    {
        get => this.currentState.Value;
        protected set => this.currentState.Value = value;
    }
}
