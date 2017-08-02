using Allure.Commons;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace Allure.SpecFlowPlugin
{
    class AllureBindingInvoker : BindingInvoker
    {
        static AllureLifecycle allure = AllureLifecycle.Instance;

        public AllureBindingInvoker(RuntimeConfiguration runtimeConfiguration, IErrorProvider errorProvider) : base(runtimeConfiguration, errorProvider)
        {
        }
        public override object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration)
        {
            var hook = binding as HookBinding;

            // process hook
            if (hook != null)
            {
                string containerId = AllureHelper.FeatureContainerId(contextManager.FeatureContext);
                try
                {
                    string fixtureId = AllureHelper.NewId();

                    switch (hook.HookType)
                    {
                        case HookType.BeforeFeature:
                            if (hook.HookOrder != int.MinValue)
                                allure.StartBeforeFixture(
                                    containerId,
                                    fixtureId,
                                    AllureHelper.GetFixtureResult(hook)
                                );
                            else
                            {
                                allure.StartTestContainer(new TestResultContainer()
                                {
                                    uuid = containerId
                                });

                                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                            }
                            break;

                        case HookType.BeforeScenario:
                            if (hook.HookOrder != int.MinValue && hook.HookOrder != int.MaxValue)
                                allure.StartBeforeFixture(
                                    containerId,
                                    fixtureId,
                                    AllureHelper.GetFixtureResult(hook)
                                );
                            else
                                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);

                            break;


                        case HookType.AfterFeature:
                            if (hook.HookOrder != int.MaxValue)
                                allure.StartAfterFixture(
                                    containerId,
                                    fixtureId,
                                    AllureHelper.GetFixtureResult(hook)
                                );
                            else
                            {
                                allure.WriteTestContainer(containerId);
                                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                            }
                            break;

                        case HookType.AfterScenario:
                            if (hook.HookOrder != int.MinValue && hook.HookOrder != int.MaxValue)
                                allure.StartAfterFixture(
                                    containerId,
                                    fixtureId,
                                    AllureHelper.GetFixtureResult(hook)
                                );
                            else
                                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);

                            break;

                        case HookType.BeforeStep:
                            allure.StartBeforeFixture(
                                containerId,
                                fixtureId,
                                AllureHelper.GetFixtureResult(hook)
                            );
                            break;

                        case HookType.AfterStep:
                            allure.StartAfterFixture(
                                containerId,
                                fixtureId,
                                AllureHelper.GetFixtureResult(hook)
                        );
                            break;

                        case HookType.BeforeTestRun:
                        case HookType.AfterTestRun:
                        case HookType.BeforeScenarioBlock:
                        case HookType.AfterScenarioBlock:
                        default:
                            return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                    }

                    var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                    allure.StopFixture(x => x.status = Status.passed);
                    return result;
                }

                catch (Exception ex)
                {
                    allure.StopFixture(x => x.status = Status.broken);

                    // create fake scenario
                    if (hook.HookType == HookType.BeforeScenario || hook.HookType == HookType.BeforeFeature)
                    {
                        allure.StartTestCase(
                            containerId,
                            AllureHelper.GetTestResult(
                                contextManager.FeatureContext?.FeatureInfo,
                                contextManager.ScenarioContext?.ScenarioInfo));
                    }

                    allure.UpdateTestCase(
                        AllureHelper.ScenarioId(contextManager.ScenarioContext?.ScenarioInfo),
                        x =>
                            {
                                x.status = Status.broken;
                                x.statusDetails = new StatusDetails()
                                {
                                    message = ex.Message,
                                    trace = ex.StackTrace
                                };
                            });

                    if (hook.HookType == HookType.AfterScenario)
                    {
                        AllureLifecycle.Instance
                            .WriteTestCase(AllureHelper.ScenarioId(contextManager.ScenarioContext?.ScenarioInfo));
                    }

                    throw;
                }
            }

            // process step
            else
            {
                try
                {
                    var step = binding as StepDefinitionBinding;
                    var stepId = AllureHelper.NewId();
                    allure.StartStep(stepId,
                        new StepResult()
                        {
                            name = $"{contextManager.StepContext.StepInfo.StepDefinitionType} {contextManager.StepContext.StepInfo.Text}"
                        });

                    if (contextManager.StepContext.StepInfo.Table != null)
                    {
                        var csvFile = $"{Guid.NewGuid().ToString()}.csv";
                        var a = contextManager.StepContext.StepInfo.Table.Rows.ToList();
                        using (var csv = new CsvWriter(File.CreateText(csvFile)))
                        {
                            foreach (var item in contextManager.StepContext.StepInfo.Table.Header)
                            {
                                csv.WriteField(item);
                            }
                            csv.NextRecord();
                            foreach (var row in contextManager.StepContext.StepInfo.Table.Rows)
                            {
                                foreach (var item in row.Values)
                                {
                                    csv.WriteField(item);
                                }
                                csv.NextRecord();
                            }
                        }
                        allure.AddAttachment("table", "text/csv", csvFile);
                    }

                    var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);

                    allure.StopStep(x => x.status = Status.passed);

                    return result;
                }
                catch (Exception ex)
                {
                    AllureLifecycle.Instance
                        .StopStep(x => x.status = Status.failed)
                        .UpdateTestCase(
                        AllureHelper.ScenarioId(contextManager.ScenarioContext?.ScenarioInfo),
                            x =>
                            {
                                x.status = Status.failed;
                                x.statusDetails = new StatusDetails()
                                {
                                    message = ex.Message,
                                    trace = ex.StackTrace
                                };
                            });

                    throw;
                }
            }
        }

    }
}