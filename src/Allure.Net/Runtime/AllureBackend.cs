using System;
using Allure.Abstractions;

namespace Allure.Runtime;

/// <summary>
/// Provides process-wide registration of Allure runtime routes.
/// </summary>
public static class AllureBackend
{
    readonly static AllureRuntimeRegistry registry = new();

    internal static IAllureRuntime? ResolveCurrentScope() =>
        registry.ResolveCurrentScope();

    internal static IAllureRuntime? ResolveGlobalScope() =>
        registry.ResolveGlobalScope();

    /// <summary>
    /// Installs a runtime route.
    /// The installation lasts until the returned registration is disposed.
    /// </summary>
    public static IDisposable Install(IAllureRuntimeRoute route) =>
        registry.Install(route);
}
