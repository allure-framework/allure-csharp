using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Reflection;
using Allure.Net.Commons;
using HarmonyLib;
using Xunit.Runner.Common;

namespace Allure.Xunit;

internal static class AllureXunitPatcher
{
    const string ALLURE_ID = "io.qameta.allure.xunit.v3";

    static bool isPatched;
    static IRunnerLogger? logger;

    static readonly ConcurrentDictionary<string, AllureContext> contextsByTestUniqueId = new();

    internal static void Enable(IRunnerLogger runnerLogger)
    {
        if (isPatched)
        {
            return;
        }

        logger = runnerLogger;

        try
        {
            var patcher = new Harmony(ALLURE_ID);
            var runnerBaseType = ResolveXunitType("Xunit.v3.XunitTestRunnerBase`2");

            if (runnerBaseType is null)
            {
                logger.LogWarning("[allure] Unable to locate Xunit.v3.XunitTestRunnerBase`2 for patching");
                return;
            }

            var invokeTest = runnerBaseType
                .GetMethods(BindingFlags.Instance | BindingFlags.NonPublic)
                .FirstOrDefault(static m =>
                    string.Equals(m.Name, "InvokeTest", StringComparison.Ordinal)
                    && m.GetParameters().Length == 2
                );

            if (invokeTest is null)
            {
                logger.LogWarning("[allure] Unable to locate Xunit.v3.XunitTestRunnerBase`2.InvokeTest for patching");
                return;
            }

            patcher.Patch(
                invokeTest,
                prefix: new HarmonyMethod(typeof(AllureXunitPatcher), nameof(BeforeInvokeTest)),
                postfix: new HarmonyMethod(typeof(AllureXunitPatcher), nameof(AfterInvokeTest))
            );

            isPatched = true;
            logger.LogRaw("[allure] xunit v3 invoker patch enabled");
        }
        catch (Exception e)
        {
            logger.LogWarning("[allure] Failed to patch xunit v3 invoker: {0}", e.ToString());
        }
    }

    internal static void RegisterContext(string testUniqueId, AllureContext context)
    {
        if (string.IsNullOrEmpty(testUniqueId) || context is null)
        {
            return;
        }

        contextsByTestUniqueId[testUniqueId] = context;
    }

    internal static void UnregisterContext(string testUniqueId)
    {
        if (string.IsNullOrEmpty(testUniqueId))
        {
            return;
        }

        contextsByTestUniqueId.TryRemove(testUniqueId, out _);
    }

    static void BeforeInvokeTest(object ctxt)
    {
        var testUniqueId = ResolveTestUniqueId(ctxt);
        if (testUniqueId is null)
        {
            return;
        }

        if (contextsByTestUniqueId.TryGetValue(testUniqueId, out var context))
        {
            AllureLifecycle.Instance.RestoreContext(context);
        }
    }

    static void AfterInvokeTest(object ctxt)
    {
        var testUniqueId = ResolveTestUniqueId(ctxt);
        if (testUniqueId is null)
        {
            return;
        }

        if (contextsByTestUniqueId.TryGetValue(testUniqueId, out _))
        {
            contextsByTestUniqueId[testUniqueId] = AllureLifecycle.Instance.Context;
        }
    }

    static string? ResolveTestUniqueId(object ctxt)
    {
        var test = ctxt
            .GetType()
            .GetProperty("Test", BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic)
            ?.GetValue(ctxt);

        if (test is null)
        {
            return null;
        }

        var testType = test.GetType();

        var publicUniqueId = testType
            .GetProperty("UniqueID", BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(test) as string;
        if (!string.IsNullOrEmpty(publicUniqueId))
        {
            return publicUniqueId;
        }

        var publicTestUniqueId = testType
            .GetProperty("TestUniqueID", BindingFlags.Instance | BindingFlags.Public)
            ?.GetValue(test) as string;
        if (!string.IsNullOrEmpty(publicTestUniqueId))
        {
            return publicTestUniqueId;
        }

        var hiddenUniqueId = testType
            .GetProperties(BindingFlags.Instance | BindingFlags.NonPublic)
            .FirstOrDefault(static p => p.Name.EndsWith("UniqueID", StringComparison.Ordinal))
            ?.GetValue(test) as string;

        return hiddenUniqueId;
    }

    static Type? ResolveXunitType(string typeName)
    {
        foreach (var assembly in AppDomain.CurrentDomain.GetAssemblies())
        {
            var type = assembly.GetType(typeName, throwOnError: false, ignoreCase: false);
            if (type is not null)
            {
                return type;
            }
        }

        return null;
    }
}