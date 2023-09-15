using System;

using HarmonyLib;
using Xunit;
using Xunit.Abstractions;
using Xunit.Sdk;

namespace Allure.XUnit
{
    static class AllureXunitPatcher
    {
        const string ALLURE_ID = "io.qameta.allure.xunit";
        static bool isPatched = false;
        static IRunnerLogger logger;

        public static void PatchXunit(IRunnerLogger runnerLogger)
        {
            if (isPatched)
            {
                logger.LogMessage(
                    "Patching is skipped: Xunit is already patched"
                );
                return;
            }

            logger = runnerLogger;
            var patcher = new Harmony(ALLURE_ID);
            PatchXunitTestRunnerCtors(patcher);
            isPatched = true;
        }

        static void PatchXunitTestRunnerCtors(Harmony patcher)
        {
            var testRunnerType = typeof(XunitTestRunner);
            var wasPatched = false;
            foreach (var ctor in testRunnerType.GetConstructors())
            {
                try
                {
                    patcher.Patch(
                        ctor,
                        postfix: new HarmonyMethod(
                            typeof(AllureXunitPatcher),
                            nameof(AllureXunitPatcher.OnTestRunnerCreated)
                        )
                    );
                    wasPatched = true;
                    logger.LogImportantMessage(
                        "{0}'s {1} has been patched",
                        testRunnerType.Name,
                        ctor.ToString()
                    );
                }
                catch (Exception e)
                {
                    logger.LogError(
                        "Unable to patch {0}'s {1}: {2}",
                        testRunnerType.Name,
                        ctor.ToString(),
                        e.ToString()
                    );
                }
            }

            if (!wasPatched)
            {
                logger.LogWarning(
                    "No constructors of {0} were pathched. Some theories may " +
                        "miss their parameters in the report",
                    testRunnerType.Name
                );
            }
        }

        static void OnTestRunnerCreated(
            ITest test,
            object[] testMethodArguments
        )
        {
            if (AllureMessageSink.CurrentSink is null)
            {
                logger.LogWarning(
                    "Unable to get current message sink from the test " +
                        "runner's hook of the {0} test",
                    test.DisplayName
                );
            }
            else
            {
                AllureMessageSink.CurrentSink.OnTestArgumentsCreated(
                    test,
                    testMethodArguments
                );
            }
        }
    }
}