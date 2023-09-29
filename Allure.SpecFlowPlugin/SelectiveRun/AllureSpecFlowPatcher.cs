using HarmonyLib;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.UnitTestProvider;

namespace Allure.SpecFlowPlugin.SelectiveRun
{
    static class AllureSpecFlowPatcher
    {
        const string ALLURE_ID = "io.qameta.allure.xunit";
        static bool isTestPlanSupportInjected= false;

        internal static void EnsureTestPlanSupportInjected(
            IUnitTestRuntimeProvider unitTestRuntimeProvider
        )
        {
            if (isTestPlanSupportInjected)
            {
                return;
            }

            InjectTestPlanSupport(unitTestRuntimeProvider);
            isTestPlanSupportInjected = true;
        }

        static void InjectTestPlanSupport(
            IUnitTestRuntimeProvider unitTestRuntimeProvider
        )
        {
            var patcher = new Harmony(ALLURE_ID);
            InjectTestPlanCheckToTestRunner(patcher);
            AdjustIgnoredScenarioMessage(patcher, unitTestRuntimeProvider);
        }

        static void InjectTestPlanCheckToTestRunner(Harmony patcher) =>
            PatchRunnerFactories(
                patcher,
                (
                    from m in GetPotentialRunnerFactoryMethods()
                    where IsRunnerFactoryCandidate(m)
                    select m
                )
            );

        static void AdjustIgnoredScenarioMessage(
            Harmony patcher,
            IUnitTestRuntimeProvider unitTestRuntimeProvider
        )
        {
            var testIgnoreMethod = unitTestRuntimeProvider.GetType().GetMethod(
                nameof(IUnitTestRuntimeProvider.TestIgnore),
                new[] { typeof(string) }
            );
            if (testIgnoreMethod is not null)
            {
                patcher.Patch(
                    testIgnoreMethod,
                    prefix: new HarmonyMethod(
                        typeof(AllureSpecFlowPatcher),
                        nameof(ChangeMessageForTestPlanDisabledScenario)
                    )
                );
            }
        }

        static void PatchRunnerFactories(
            Harmony patcher,
            IEnumerable<MethodInfo> factoryCandidates
        )
        {
            foreach (var factoryCandidate in factoryCandidates)
            {
                PatchRunnerFactory(patcher, factoryCandidate);
            }
        }

        static void PatchRunnerFactory(
            Harmony patcher,
            MethodInfo factoryCandidate
        )
        {
            patcher.Patch(
                factoryCandidate,
                postfix: new HarmonyMethod(
                    typeof(AllureSpecFlowPatcher),
                    nameof(WrapTestRunnerWithTestPlanSupport)
                )
            );
        }

        static bool IsRunnerFactoryCandidate(MethodInfo method) =>
            method.ReturnType == typeof(ITestRunner)
                && method.GetParameters().All(p => p.IsOptional);

        static IEnumerable<MethodInfo> GetPotentialRunnerFactoryMethods() =>
            typeof(TestRunnerManager).GetMethods(
                BindingFlags.Static | BindingFlags.Public
            );

        static ITestRunner WrapTestRunnerWithTestPlanSupport(
            ITestRunner __result
        )
        {
            return new SelectiveRunTestRunner(__result);
        }

        static void ChangeMessageForTestPlanDisabledScenario(
            ref string __0
        )
        {
            if (!SelectiveRunTestRunner.CurrentRunner.IsCurrentScenarioSelected)
            {
                __0 = "Deselected by the testplan.";
            }
        }
    }
}
