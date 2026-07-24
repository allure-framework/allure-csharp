using System;
using System.Threading.Tasks;

namespace Allure.Sdk.Runtime;

/// <summary>
/// Defines how the current execution state of an Allure runtime is accessed,
/// updated, and temporarily replaced while an operation is performed.
/// </summary>
public interface IAllureRuntimeContext
{
    /// <summary>
    /// Gets the Allure runtime associated with this context.
    /// </summary>
    IAllureRuntime Runtime { get; }

    /// <summary>
    /// Gets the current execution state for the current execution flow.
    /// </summary>
    AllureExecutionState CurrentState { get; }

    /// <summary>
    /// Replaces the current execution state with a new state produced from it.
    /// </summary>
    /// <param name="transition">
    /// A function that produces the new execution state from the current one.
    /// </param>
    void Update(Func<AllureExecutionState, AllureExecutionState> transition);

    /// <summary>
    /// Runs the specified action with the provided execution state as the
    /// current state and returns the state produced by the action.
    /// </summary>
    /// <param name="state">
    /// The execution state to make current while the action is running.
    /// </param>
    /// <param name="action">
    /// The action to run. The argument passed to the action is the runtime
    /// associated with this context.
    /// </param>
    /// <returns>
    /// The execution state that is current when the action completes.
    /// </returns>
    /// <remarks>
    /// The execution state that was current before the call is restored after
    /// the action completes, including when the action throws an exception.
    /// </remarks>
    AllureExecutionState RunWithState(
        AllureExecutionState state,
        Action<IAllureRuntime> action
    );

    /// <summary>
    /// Runs the specified asynchronous action with the provided execution
    /// state as the current state.
    /// </summary>
    /// <param name="state">
    /// The execution state to make current while the action is running.
    /// </param>
    /// <param name="asyncAction">
    /// The asynchronous action to run. The argument passed to the action is
    /// the runtime associated with this context.
    /// </param>
    /// <returns>
    /// A task that represents the asynchronous operation.
    /// </returns>
    /// <remarks>
    /// The execution state that was current before the call is restored after
    /// the returned task completes, including when the action throws an
    /// exception. Unlike
    /// <see cref="RunWithState(AllureExecutionState, Action{IAllureRuntime})"/>,
    /// this method does not return the new state with the changes made within
    /// the asynchronous action. Use
    /// <see cref="GetWithStateAsync{T}(AllureExecutionState, Func{IAllureRuntime, Task{T}})"/>
    /// to capture the state from the asynchronous flow explicitly.
    /// </remarks>
    Task RunWithStateAsync(
        AllureExecutionState state,
        Func<IAllureRuntime, Task> asyncAction
    );

    /// <summary>
    /// Invokes the specified function with the provided execution state as the
    /// current state and returns the function's result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of value returned by the function.
    /// </typeparam>
    /// <param name="state">
    /// The execution state to make current while the function is running.
    /// </param>
    /// <param name="function">
    /// The function to invoke. The argument passed to the function is the
    /// runtime associated with this context.
    /// </param>
    /// <returns>
    /// The value returned by <paramref name="function"/>.
    /// </returns>
    /// <remarks>
    /// The execution state that was current before the call is restored after
    /// the function completes, including when the function throws an
    /// exception.
    /// </remarks>
    T GetWithState<T>(
        AllureExecutionState state,
        Func<IAllureRuntime, T> function
    );

    /// <summary>
    /// Invokes the specified asynchronous function with the provided execution
    /// state as the current state and returns the function's result.
    /// </summary>
    /// <typeparam name="T">
    /// The type of value returned by the function.
    /// </typeparam>
    /// <param name="state">
    /// The execution state to make current while the function is running.
    /// </param>
    /// <param name="asyncFunction">
    /// The asynchronous function to invoke. The argument passed to the
    /// function is the runtime associated with this context.
    /// </param>
    /// <returns>
    /// A task whose result is the value returned by
    /// <paramref name="asyncFunction"/>.
    /// </returns>
    /// <remarks>
    /// The execution state that was current before the call is restored after
    /// the returned task completes, including when the function throws an
    /// exception.
    /// </remarks>
    Task<T> GetWithStateAsync<T>(
        AllureExecutionState state,
        Func<IAllureRuntime, Task<T>> asyncFunction
    );
}
