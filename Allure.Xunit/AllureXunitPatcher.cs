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
                _logger.LogWarning("Unable to get current message sink.");
            }

            return sink;
        }
    }

    internal static void PatchXunit(IRunnerLogger runnerLogger)
    {
        if (_isPatched)
        {
            _logger.LogMessage(
                "Patching is skipped: Xunit is already patched"
            );
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
                    "{0}'s {1} has been patched",
                    testRunnerType.Name,
                    ctor.ToString()
                );
            }
            catch (Exception e)
            {
                _logger.LogWarning(
                    "Unable to patch {0}'s {1}: {2}",
                    testRunnerType.Name,
                    ctor.ToString(),
                    e.ToString()
                );
            }
        }

        if (!wasPatched)
        {
            _logger.LogWarning(
                "No constructors of {0} were patched. Some theories may " +
                "miss their parameters in the report",
                testRunnerType.Name
            );
        }
    }

    private static void OnTestRunnerCreating(ITest test, ref string skipReason)
    {
        if (!CurrentSink.SelectByTestPlan(test))
        {
            skipReason = AllureTestPlan.SkipReason;
        }
    }

    private static void OnTestRunnerCreated(ITest test, object[] testMethodArguments) =>
        CurrentSink.OnTestArgumentsCreated(test, testMethodArguments);
}