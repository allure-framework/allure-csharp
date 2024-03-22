using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Reqnroll;
using Reqnroll.UnitTestProvider;
using Xunit;

namespace Allure.ReqnrollPlugin.Tests.Samples;
public enum TestOutcome
{
    passed,
    failed,
    broken
}

[Binding]
public class BindingDefinitions
{
    readonly ScenarioContext scenarioContext;
    readonly IUnitTestRuntimeProvider runnerRuntimeApi;

    public BindingDefinitions(ScenarioContext scenarioContext, IUnitTestRuntimeProvider runnerRuntimeApi)
    {
        this.scenarioContext = scenarioContext;
        this.runnerRuntimeApi = runnerRuntimeApi;
    }

    [BeforeTestRun]
    public static void BeforeTestRun() =>
        AllureLifecycle.Instance.CleanupResultDirectory();

    [StepDefinition(@"Step is '(.*)'")]
    public static async Task StepResultIs(TestOutcome outcome)
    {
        switch (outcome)
        {
            case TestOutcome.failed:
                Assert.Fail("Assertion failed");
                break;
            case TestOutcome.broken:
                throw new Exception("This test was failed");
        }
        await Task.CompletedTask;
    }

    [StepDefinition("Step with attachment")]
    public static void StepWithAttach() =>
        AllureApi.AddAttachment(
            "Text attachment",
            "text/plain",
            "Attachment content"u8.ToArray()
        );

    [StepDefinition("Step with table")]
    public static void StepWithTable(Table _) { }

    [StepDefinition("Step with params: (.*)")]
    public static void StepWithArgs(int _, string __) { }

    [BeforeFeature("beforefeaturepassed", Order = 1)]
    public static void PassedBeforeFeature() { }

    [AfterFeature("afterfeaturepassed", Order = 1)]
    public static void PassedAfterFeature() { }

    [BeforeFeature("beforefeaturefailed")]
    public static void FailedBeforeFeature() =>
        throw new Exception("BeforeFeature failed");

    [AfterFeature("afterfeaturefailed")]
    public static void FailedAfterFeature() =>
        throw new Exception("AfterFeature failed");

    [BeforeScenario("beforescenario")]
    [AfterScenario("afterscenario")]
    [BeforeStep("beforestep")]
    [AfterStep("afterstep")]
    public void HandleIt() =>
        this.Handle(scenarioContext.ScenarioInfo.Tags);

    void Handle(string[] tags)
    {
        if (tags != null && tags.Contains("attachment"))
        {
            var content = "text file"u8.ToArray();
            AllureApi.AddAttachment("attachment-1", "text/plain", content, ".txt");
            AllureApi.AddAttachment("attachment-2", "text/plain", content, ".txt");
        }

        if (tags != null)
        {
            if (ShouldFail(tags))
            {
                Assert.Fail("The hook has failed");
            }
            else if (ShouldBreak(tags))
            {
                throw new Exception("The hook is broken");
            }
            else if (tags.Contains("runtimeignore"))
            {
                this.runnerRuntimeApi.TestIgnore("Ignored because of '@runtimeignore'");
            }
        }
    }

    static bool ShouldFail(IEnumerable<string> tags) =>
        tags.Any(t => t.EndsWith("failed"));

    static bool ShouldBreak(IEnumerable<string> tags) =>
        tags.Any(t => t.EndsWith("broken"));
}
