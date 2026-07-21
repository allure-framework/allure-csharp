using System;
using Allure.Abstractions;

namespace Allure.Runtime;

public static class AllureBackend
{
    readonly static AllureRuntimeRegistry registry = new();

    internal static IAllureRuntime? ResolveCurrentScope() =>
        registry.ResolveCurrentScope();

    internal static IAllureRuntime? ResolveGlobalScope() =>
        registry.ResolveGlobalScope();

    public static IDisposable Install(IAllureRuntimeRoute route) =>
        registry.Install(route);
}