using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.NUnit.Tests.Steps;

class StepTests
{
    [Test]
    public async Task StepAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.StepAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.StepsMatching([
                step => step.HasName("Void")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Return")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Async")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("AsyncReturn")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Renamed")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Parameters")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([
                        p => p.HasName("plain")
                            .And.HasValue("1")
                            .And.HasNoMode(),
                        p => p.HasName("Bar")
                            .And.HasValue("3")
                            .And.HasMode(AllureParameterMode.Default),
                        p => p.HasName("masked")
                            .And.HasValue("4")
                            .And.HasMode(AllureParameterMode.Masked),
                        p => p.HasName("hidden")
                            .And.HasValue("5")
                            .And.HasMode(AllureParameterMode.Hidden),
                    ])
                    .And.HasStepsMatching([]),
            ]);
    }

    [Test]
    public async Task LegacyStepAttributeWorks()
    {
        var results = await AllureSampleRunner.RunAsync2(AllureSampleRegistry.LegacyStepAttributes);

        await Assert.That(results).HasSingleTestResult()
            .With.StepsMatching([
                step => step.HasName("Void")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Return")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Async")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("AsyncReturn")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Renamed")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("Parameters")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([
                        p => p.HasName("foo").And.HasValue("1"),
                        p => p.HasName("bar").And.HasValue("\"baz\""),
                    ])
                    .And.HasStepsMatching([]),
                step => step.HasName("SkippedParameter")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([])
                    .And.HasStepsMatching([]),
                step => step.HasName("RenamedParameter")
                    .And.HasStatus(AllureStatus.Passed)
                    .And.HasParametersMatching([
                        p => p.HasName("Bar"),
                    ])
                    .And.HasStepsMatching([]),
            ]);
    }
}
