using System;
using System.Collections.Generic;
using System.Text;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.TestPlan;
using Allure.ReqnrollPlugin.Configuration;
using Allure.ReqnrollPlugin.Functions;
using Reqnroll;
using Reqnroll.Bindings;

namespace Allure.ReqnrollPlugin.State;

static class AllureReqnrollStateFacade
{
    const string LABELS_AND_LINKS_CACHE_KEY =
        "LABELS_AND_LINKS";

    const string ASSERT_EXC_NUNIT =
        "NUnit.Framework.AssertionException";
    const string ASSERT_EXC_NUNIT_MULTIPLE =
        "NUnit.Framework.MultipleAssertException";
    const string ASSERT_EXC_XUNIT_NEW = // From v2.4.2 and onward.
        "Xunit.Sdk.IAssertionException";
    const string ASSERT_EXC_XUNIT_OLD = // Prior to v2.4.2
        "Xunit.Sdk.XunitException";
    const string ASSERT_EXC_MSTEST =
        "Microsoft.VisualStudio.TestTools.UnitTesting.AssertFailedException";

    const string STEP_PENDING_MESSAGE = "The step is not implemented.";
    const string STEP_UNKNOWN_MESSAGE =
        "No matching definition found for this step.";
    const string BAD_HOOK_FIXTURE_NAME_FORMAT = "Invalid {0} hook";
    const string FAILED_FEATURE_SCENARIO_NAME_FORMAT =
        "{0} of '{1}' has failed";
    const string FAILED_FEATURE_SCENARIO_DESC_FORMAT =
        "This test result serves as a placeholder to report an exception " +
        "in {0} hook of '{1}'.";

    static AllureLifecycle Lifecycle { get => AllureLifecycle.Instance; }

    static Dictionary<ExecutableItem, StringBuilder> OutputCache { get; } = new();

    static AllureReqnrollConfiguration Configuration
    {
        get => AllureReqnrollConfiguration.CurrentConfig;
    }

    internal static void Initialize()
    {
        Lifecycle.AllureConfiguration.FailExceptions ??= new()
        {
            ASSERT_EXC_NUNIT,
            ASSERT_EXC_NUNIT_MULTIPLE,
            ASSERT_EXC_XUNIT_NEW,
            ASSERT_EXC_XUNIT_OLD,
            ASSERT_EXC_MSTEST
        };
    }

    internal static void StartContainer() =>
        Lifecycle
            .StartTestContainer(
                MappingFunctions.CreateContainer()
            );

    internal static void StartBeforeFixture(IHookBinding binding) =>
        Lifecycle.StartBeforeFixture(
            MappingFunctions.ToFixtureResult(binding)
        );

    internal static void StartAfterFixture(IHookBinding binding) =>
        Lifecycle.StartAfterFixture(
            MappingFunctions.ToFixtureResult(binding)
        );

    internal static void StopFixture()
    {
        AttachOutputCacheAsAttachment();
        ExtendedApi.PassFixture();
    }

    internal static void FixSnapshottedFixtureStatus(
        AllureContext stateSnapshot,
        Exception error
    ) =>
        Lifecycle.RunInContext(
            stateSnapshot,
            () => ExtendedApi.ResolveFixture(error)
        );

    internal static void CreateFailedFixturePlaceHolder(
        HookType hookType,
        Exception error
    )
    {
        var start = ResolveStartFixtureFunction(hookType);
        start(
            string.Format(BAD_HOOK_FIXTURE_NAME_FORMAT, hookType)
        );
        ExtendedApi.ResolveFixture(error);
    }

    internal static void ReportFeatureHookError(
        ITestRunnerManager runnerManager,
        HookType hookType,
        FeatureInfo featureInfo,
        Exception error
    ) =>
        Lifecycle
            .StartTestCase(
                MappingFunctions.CreateTestResult(
                    runnerManager.TestAssembly,
                    featureInfo,
                    string.Format(
                        FAILED_FEATURE_SCENARIO_NAME_FORMAT,
                        hookType,
                        featureInfo.Title
                    ),
                    string.Format(
                        FAILED_FEATURE_SCENARIO_DESC_FORMAT,
                        hookType,
                        featureInfo.Title
                    )
                )
            )
            .StopTestCase(tc =>
            {
                tc.status = ModelFunctions.ResolveErrorStatus(
                    Lifecycle.AllureConfiguration.FailExceptions,
                    error
                );
                tc.statusDetails = ModelFunctions.ToStatusDetails(error);
            })
            .WriteTestCase();

    static Action<string> ResolveStartFixtureFunction(HookType hookType) =>
        hookType switch
        {
            HookType.BeforeScenario or HookType.BeforeFeature =>
                ExtendedApi.StartBeforeFixture,
            _ => ExtendedApi.StartAfterFixture
        };

    internal static void ScheduleScenario(
        ITestRunnerManager runnerManager,
        FeatureInfo featureInfo,
        ScenarioContext scenarioContext
    ) =>
        Lifecycle
            .StartTestContainer(
                MappingFunctions.CreateContainer()
            )
            .ScheduleTestCase(
                MappingFunctions.CreateTestResult(
                    runnerManager.TestAssembly,
                    featureInfo,
                    scenarioContext.ScenarioInfo,
                    ResolveScenarioMetadata(featureInfo, scenarioContext)
                )
            );

    internal static void StartTestCase() =>
        Lifecycle.StartTestCase();

    internal static void StartStep(StepInfo stepInfo)
    {
        var stepResult = MappingFunctions.ToStepResult(stepInfo);
        var (attachmentsData, parameters) =
            MappingFunctions.ResolveStepAttachmentsAndParameters(
                Configuration.GherkinPatterns.StepArguments,
                stepInfo
            );
        stepResult.parameters = parameters;
        Lifecycle.StartStep(stepResult);
        foreach (var (title, mediaType, content, extension) in attachmentsData)
        {
            AllureApi.AddAttachment(title, mediaType, content, extension);
        }
    }

    internal static void StopStep(
        IScenarioContext scenarioContext,
        IScenarioStepContext stepContext
    )
    {
        AttachOutputCacheAsAttachment();

        var reqnrollStatus = ResolveStepStatus(
            scenarioContext.ScenarioExecutionStatus,
            stepContext.Status
        );
        var error = scenarioContext.TestError;
        switch (reqnrollStatus)
        {
            case ScenarioExecutionStatus.OK:
                ExtendedApi.PassStep();
                break;
            case ScenarioExecutionStatus.StepDefinitionPending:
                ExtendedApi.SkipStep(
                    s => s.statusDetails.message = STEP_PENDING_MESSAGE
                );
                break;
            case ScenarioExecutionStatus.Skipped:
                ExtendedApi.SkipStep();
                break;
            case ScenarioExecutionStatus.UndefinedStep:
                ExtendedApi.BreakStep(
                    s => s.statusDetails.message = STEP_UNKNOWN_MESSAGE
                );
                break;
            case ScenarioExecutionStatus.BindingError:
                ExtendedApi.BreakStep(s =>
                {
                    s.statusDetails.message = error!.Message;
                });
                break;
            case ScenarioExecutionStatus.TestError:
                ExtendedApi.ResolveStep(error!);
                break;
        }
    }

    internal static void StopTestCase()
    {
        AttachOutputCacheAsAttachment();
        Lifecycle.StopTestCase();
    }

    internal static void StopContainer()
    {
        if (OutputCache.Count > 0)
        {
            Console.WriteLine("Warning: Some output was not attached to a test case, step or fixture.");
        }
        OutputCache.Clear(); // Reset cache on a per-container basisis
        Lifecycle.StopTestContainer();
    }
    internal static void EmitScenarioFiles(
        IScenarioContext scenarioContext
    )
    {
        var (status, statusDetails) = MappingFunctions.ResolveTestStatus(
            Lifecycle.AllureConfiguration.FailExceptions,
            scenarioContext
        );
        Lifecycle
            .UpdateTestCase(t =>
            {
                t.status = status;
                t.statusDetails = statusDetails;
            })
            .WriteTestCase()
            .WriteTestContainer();
    }

    internal static void EmitFeatureFiles() =>
        Lifecycle.WriteTestContainer();

    internal static void AddOutput(string text)
    {
        if (!(Lifecycle.Context.HasTest || Lifecycle.Context.HasFixture || Lifecycle.Context.HasStep))
            return;
        Lifecycle.UpdateExecutableItem(tr =>
        {
            if (OutputCache.ContainsKey(tr))
            {
                OutputCache[tr].Append(text);
                return;
            }

            OutputCache.Add(tr, new StringBuilder(text));
        });
    }

    internal static string? GetAllureId(
        FeatureInfo featureInfo,
        ScenarioContext scenarioContext
    ) =>
        AllureTestPlan.GetAllureId(
            ResolveScenarioMetadata(featureInfo, scenarioContext).labels
        );

    static void AttachOutputCacheAsAttachment()
    {
        var output = "";
        Lifecycle.UpdateExecutableItem(tr =>
        {
            if (OutputCache.ContainsKey(tr))
            {
                output = OutputCache[tr].ToString();
                OutputCache.Remove(tr);
            }
        });

        if (!string.IsNullOrWhiteSpace(output))
        {
            AllureApi.AddAttachment("TestOutput", "text/plain", Encoding.UTF8.GetBytes(output), ".log");
        }
    }
    static (List<Label> labels, List<Link> links) ResolveScenarioMetadata(
        FeatureInfo featureInfo,
        ScenarioContext scenarioContext
    )
    {
        var cacheHit = scenarioContext.TryGetValue(
            LABELS_AND_LINKS_CACHE_KEY,
            out (List<Label>, List<Link>) items
        );
        if (!cacheHit)
        {
            items = MappingFunctions.GetLabelsAndLinks(
                Configuration.GherkinPatterns,
                featureInfo,
                scenarioContext.ScenarioInfo
            );
            scenarioContext.Set(items, LABELS_AND_LINKS_CACHE_KEY);
        }
        return items;
    }

    static ScenarioExecutionStatus ResolveStepStatus(
        ScenarioExecutionStatus scenarioStatus,
        ScenarioExecutionStatus stepStatus
    ) =>
        stepStatus is ScenarioExecutionStatus.OK
            ? scenarioStatus
            : stepStatus;
}
