using System;
using Allure.Net.Commons.TestPlan;
using HarmonyLib;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.Xunit;

internal static class AllureXunitPatcher
{
    private const string ALLURE_ID = "io.qameta.allure.xunit";
    private static bool _isPatched;
    private static IRunnerLogger _logger;

    private static AllureMessageSink CurrentSink
    {
        get
        {
            var sink = AllureMessageSink.CurrentSink;

            if (sink is null)
            {
                _logger.LogWarning(
                    "{0}: Unable to get current message sink.",
                    AllureXunitFacade.LOG_SOURCE
                );
            }

            return sink;
        }
    }

    internal static void PatchXunit(IRunnerLogger runnerLogger)
    {
        if (_isPatched)
        {
            return;
        }

        _logger = runnerLogger;

        var patcher = new Harmony(ALLURE_ID);
        PatchXunitTestRunnerCtors(patcher);
        _isPatched = true;
    }

    private static void PatchXunitTestRunnerCtors(Harmony patcher)
    {
        var testRunnerType = typeof(XunitTestRunner);
        var wasPatched = false;

        foreach (var ctor in testRunnerType.GetConstructors())
        {
            try
            {
                patcher.Patch(
                    ctor,
                    prefix: new HarmonyMethod(
                        typeof(AllureXunitPatcher),
                        nameof(OnTestRunnerCreating)
                    ),
                    postfix: new HarmonyMethod(
                        typeof(AllureXunitPatcher),
                        nameof(OnTestRunnerCreated)
                    )
                );

                wasPatched = true;

                _logger.LogImportantMessage(
                    "{0}: {1}'s {2} has been patched",
                    AllureXunitFacade.LOG_SOURCE,
                    testRunnerType.Name,
                    ctor.ToString()
                );
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "{0}: Unable to patch {1}'s {2}: {3}",
                    AllureXunitFacade.LOG_SOURCE,
                    testRunnerType.Name,
                    ctor.ToString(),
                    e.ToString()
                );
            }
        }

        if (!wasPatched)
        {
            _logger.LogWarning(
                "{0}: No constructors of {1} were patched. Some theories may " +
                "miss their parameters in the report",
                AllureXunitFacade.LOG_SOURCE,
                testRunnerType.Name
            );
        }
    }

    private static void OnTestRunnerCreating(ITest test, ref string skipReason)
    {
        if (AllureXunitHelper.IsOnExternalAuthority(test))
        {
            return;
        }

        if (!CurrentSink.SelectByTestPlan(test))
        {
            skipReason = AllureTestPlan.SkipReason;
        }
    }

    private static void OnTestRunnerCreated(ITest test, object[] testMethodArguments)
    {
        if (AllureXunitHelper.IsOnExternalAuthority(test))
        {
            return;
        }

        CurrentSink.OnTestArgumentsCreated(test, testMethodArguments);
    }
}