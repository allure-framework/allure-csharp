using System;
using Allure.Runtime;
using Allure.Abstractions;
using Allure.Internal;

namespace Allure;

public static partial class AllureApi
{
    public static void SetUp(string name, Action body)
    {
        if (AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.SetUp(name, [], body);
        }
        else
        {
            body();
        }
    }

    public static void SetUp(string name, Action<IAllureFixtureContext> body)
    {
        if (AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.SetUp(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    public static TResult SetUp<TResult>(string name, Func<TResult> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.SetUp(name, [], body)
            : body();

    public static TResult SetUp<TResult>(string name, Func<IAllureFixtureContext, TResult> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.SetUp(name, [], body)
            : body(NullOperationContext.Instance);

    public static void TearDown(string name, Action body)
    {
        if (AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.TearDown(name, [], body);
        }
        else
        {
            body();
        }
    }

    public static void TearDown(string name, Action<IAllureFixtureContext> body)
    {
        if (AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api })
        {
            api.TearDown(name, [], body);
        }
        else
        {
            body(NullOperationContext.Instance);
        }
    }

    public static TResult TearDown<TResult>(string name, Func<TResult> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.TearDown(name, [], body)
            : body();

    public static TResult TearDown<TResult>(string name, Func<IAllureFixtureContext, TResult> body) =>
        AllureFrontend.Client.ResolveCurrentScope() is { Operations.Sync: var api }
            ? api.TearDown(name, [], body)
            : body(NullOperationContext.Instance);
}
