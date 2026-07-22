using System;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure;

public static partial class AllureInProcessApi
{
    /// <summary>
    /// Runs a context-aware action as a setup fixture.
    /// </summary>
    public static void SetUp(string name, Action<IAllureInProcessFixtureContext> body)
    {
        if (ResolveOperations() is { Sync: var api })
        {
            api.SetUp(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    /// <summary>
    /// Runs a context-aware function as a setup fixture and returns its result.
    /// </summary>
    public static TResult SetUp<TResult>(string name, Func<IAllureInProcessFixtureContext, TResult> body) =>
        ResolveOperations() is { Sync: var api }
            ? api.SetUp(name, [], body)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs a context-aware action as a teardown fixture.
    /// </summary>
    public static void TearDown(string name, Action<IAllureInProcessFixtureContext> body)
    {
        if (ResolveOperations() is { Sync: var api })
        {
            api.TearDown(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    /// <summary>
    /// Runs a context-aware function as a teardown fixture and returns its result.
    /// </summary>
    public static TResult TearDown<TResult>(string name, Func<IAllureInProcessFixtureContext, TResult> body) =>
        ResolveOperations() is { Sync: var api }
            ? api.TearDown(name, [], body)
            : body(NullOperationContext.Instance);
}
