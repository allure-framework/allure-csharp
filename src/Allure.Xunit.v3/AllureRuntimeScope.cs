using System;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;

namespace Allure.Xunit;

public sealed class AllureRuntimeScope : IDisposable
{
    readonly string testUniqueId;
    readonly AllureContext previousContext;
    readonly bool isActive;

    AllureRuntimeScope(string testUniqueId, AllureContext previousContext, bool isActive)
    {
        this.testUniqueId = testUniqueId;
        this.previousContext = previousContext;
        this.isActive = isActive;
    }

    public static void ActivateContextForDispose() =>
        AllureMessageSink.ActivateContextForDispose();

    public static AllureRuntimeScope Begin()
    {
        var lifecycle = AllureLifecycle.Instance;
        var previousContext = lifecycle.Context;

        if (TryResolveCurrentTestUniqueId(out var testUniqueId)
            && !string.IsNullOrWhiteSpace(testUniqueId)
            && AllureMessageSink.TryGetStoredContext(testUniqueId, out var contextById))
        {
            lifecycle.RestoreContext(contextById);
            return new(testUniqueId, previousContext, true);
        }

        if (AllureMessageSink.TryGetSingleStoredContext(out var singleTestUniqueId, out var singleContext))
        {
            lifecycle.RestoreContext(singleContext);
            return new(singleTestUniqueId, previousContext, true);
        }

        return new(string.Empty, previousContext, false);
    }

    public void Dispose()
    {
        if (!isActive)
        {
            return;
        }

        var lifecycle = AllureLifecycle.Instance;
        AllureMessageSink.SaveStoredContext(testUniqueId, lifecycle.Context);
        lifecycle.RestoreContext(previousContext);
    }

    static bool TryResolveCurrentTestUniqueId(out string? testUniqueId)
    {
        testUniqueId = TryResolveFromXunitTestContext();
        if (!string.IsNullOrWhiteSpace(testUniqueId))
        {
            return true;
        }

        return false;
    }

    static string? TryResolveFromXunitTestContext()
    {
        var testContextType =
            Type.GetType("Xunit.TestContext, xunit.v3.core", throwOnError: false)
            ?? AppDomain.CurrentDomain
                .GetAssemblies()
                .Select(static a => a.GetType("Xunit.TestContext", throwOnError: false, ignoreCase: false))
                .FirstOrDefault(static t => t is not null);

        if (testContextType is null)
        {
            return null;
        }

        var current = testContextType
            .GetProperty("Current", BindingFlags.Public | BindingFlags.Static)
            ?.GetValue(null);

        if (current is null)
        {
            return null;
        }

        return TryReadUniqueId(current)
            ?? TryReadUniqueIdFromProperty(current, "Test")
            ?? TryReadUniqueIdFromProperty(current, "TestCase")
            ?? TryReadUniqueIdFromProperty(current, "TestMethod")
            ?? TryReadUniqueIdFromProperty(current, "TestDetails");
    }

    static string? TryReadUniqueIdFromProperty(object source, string propertyName)
    {
        var nested = source
            .GetType()
            .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
            ?.GetValue(source);

        return nested is null ? null : TryReadUniqueId(nested);
    }

    static string? TryReadUniqueId(object source)
    {
        foreach (var propertyName in new[] { "UniqueID", "UniqueId", "TestUniqueID", "TestUniqueId" })
        {
            var value = source
                .GetType()
                .GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance)
                ?.GetValue(source) as string;

            if (!string.IsNullOrWhiteSpace(value))
            {
                return value;
            }
        }

        return null;
    }
}
