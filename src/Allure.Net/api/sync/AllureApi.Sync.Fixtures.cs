using System;
using Allure.Runtime;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    /// <summary>
    /// Runs an action as a setup fixture.
    /// </summary>
    public static void SetUp(string name, Action body)
    {
        if (AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.SetUp(name, [], body);
        }
        else
        {
            body();
        }
    }

    /// <summary>
    /// Runs a context-aware action as a setup fixture.
    /// </summary>
    public static void SetUp(string name, Action<IAllureSyncFixtureContext> body)
    {
        if (AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.SetUp(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    /// <summary>
    /// Runs a function as a setup fixture and returns its result.
    /// </summary>
    public static TResult SetUp<TResult>(string name, Func<TResult> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.SetUp(name, [], body)
            : body();

    /// <summary>
    /// Runs a context-aware function as a setup fixture and returns its result.
    /// </summary>
    public static TResult SetUp<TResult>(string name, Func<IAllureSyncFixtureContext, TResult> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.SetUp(name, [], body)
            : body(NullOperationContext.Instance);

    /// <summary>
    /// Runs an action as a teardown fixture.
    /// </summary>
    public static void TearDown(string name, Action body)
    {
        if (AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.TearDown(name, [], body);
        }
        else
        {
            body();
        }
    }

    /// <summary>
    /// Runs a context-aware action as a teardown fixture.
    /// </summary>
    public static void TearDown(string name, Action<IAllureSyncFixtureContext> body)
    {
        if (AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.TearDown(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    /// <summary>
    /// Runs a function as a teardown fixture and returns its result.
    /// </summary>
    public static TResult TearDown<TResult>(string name, Func<TResult> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.TearDown(name, [], body)
            : body();

    /// <summary>
    /// Runs a context-aware function as a teardown fixture and returns its result.
    /// </summary>
    public static TResult TearDown<TResult>(string name, Func<IAllureSyncFixtureContext, TResult> body) =>
        AllureRuntimeRouter.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.TearDown(name, [], body)
            : body(NullOperationContext.Instance);
}
