using Allure.Commons;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow;
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
            TestResult scenario = null;
            contextManager.FeatureContext?.TryGetValue(out scenario);

            // process hook
            if (hook != null)
            {
                var featureContainerId = AllureHelper.GetFeatureContainerId(contextManager.FeatureContext?.FeatureInfo);
                TestResultContainer scenarioContainer = null;
                contextManager.FeatureContext?.TryGetValue(out scenarioContainer);

                switch (hook.HookType)
                {
                    case HookType.BeforeFeature:
                        if (hook.HookOrder == int.MinValue)
                        {
                            // starting point
                            var featureContainer = new TestResultContainer()
                            {
                                uuid = AllureHelper.GetFeatureContainerId(contextManager.FeatureContext?.FeatureInfo)
                            };
                            allure.StartTestContainer(featureContainer);

                            contextManager.FeatureContext.Set(new HashSet<TestResultContainer>());
                            contextManager.FeatureContext.Set(new HashSet<TestResult>());

                            return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                        }
                        else
                            try
                            {
                                this.StartFixture(hook, featureContainerId);
                                var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                                allure.StopFixture(x => x.status = Status.passed);
                                return result;
                            }
                            catch (Exception ex)
                            {
                                allure.StopFixture(x => x.status = Status.broken);

                                // if BeforeFeature is failed execution is stopped. We need to create, update, stop and write everything here.

                                // create fake scenario container
                                scenarioContainer = new TestResultContainer()
                                {
                                    uuid = AllureHelper.NewId()
                                };
                                allure.StartTestContainer(featureContainerId, scenarioContainer);

                                // create fake scenario
                                scenario = AllureHelper.GetTestResult(contextManager.FeatureContext?.FeatureInfo, contextManager.ScenarioContext?.ScenarioInfo);
                                allure.StartTestCase(scenarioContainer.uuid, scenario);

                                // update, stop and write
                                allure
                                    .StopTestCase(x =>
                                    {
                                        x.status = Status.broken;
                                        x.statusDetails = new StatusDetails()
                                        {
                                            message = ex.Message,
                                            trace = ex.StackTrace
                                        };
                                    })
                                    .WriteTestCase(scenario.uuid)
                                    .StopTestContainer(scenarioContainer.uuid)
                                    .WriteTestContainer(scenarioContainer.uuid)
                                    .StopTestContainer(featureContainerId)
                                    .WriteTestContainer(featureContainerId);

                                throw;
                            }

                    case HookType.BeforeScenario:
                    case HookType.BeforeStep:
                    case HookType.AfterStep:
                    case HookType.AfterScenario:
                    case HookType.BeforeScenarioBlock:
                    case HookType.AfterScenarioBlock:
                        if (hook.HookOrder == int.MinValue || hook.HookOrder == int.MaxValue)
                            return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                        else
                            try
                            {
                                this.StartFixture(hook, scenarioContainer.uuid);
                                var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                                allure.StopFixture(x => x.status = Status.passed);
                                return result;
                            }
                            catch (Exception ex)
                            {
                                allure
                                    .StopFixture(x => x.status = Status.broken)
                                    .UpdateTestCase(scenario.uuid,
                                        x =>
                                        {
                                            x.status = Status.broken;
                                            x.statusDetails = new StatusDetails()
                                            {
                                                message = ex.Message,
                                                trace = ex.StackTrace
                                            };
                                        });
                                throw;
                            }

                    case HookType.AfterFeature:
                        if (hook.HookOrder == int.MaxValue)
                        // finish point
                        {
                            WriteScenarios(contextManager);
                            allure
                                   .StopTestContainer(featureContainerId)
                                   .WriteTestContainer(featureContainerId);

                            return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                        }
                        else
                            try
                            {
                                StartFixture(hook, featureContainerId);
                                var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                                allure.StopFixture(x => x.status = Status.passed);
                                return result;
                            }
                            catch (Exception ex)
                            {
                                allure
                                    .StopFixture(x => x.status = Status.broken)
                                    .UpdateTestCase(scenario.uuid,
                                        x =>
                                        {
                                            x.status = Status.broken;
                                            x.statusDetails = new StatusDetails()
                                            {
                                                message = ex.Message,
                                                trace = ex.StackTrace
                                            };
                                        });

                                WriteScenarios(contextManager);

                                allure
                                    .StopTestContainer(featureContainerId)
                                    .WriteTestContainer(featureContainerId);

                                throw;
                            }

                    case HookType.BeforeTestRun:
                    case HookType.AfterTestRun:
                    default:
                        return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                }
            }

            // process step
            else
            {
                try
                {
                    var step = binding as StepDefinitionBinding;
                    StartStep(contextManager.StepContext.StepInfo, scenario.uuid);
                    var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                    allure.StopStep(x => x.status = Status.passed);
                    return result;
                }
                catch (Exception ex)
                {
                    allure
                        .StopStep(x => x.status = Status.failed)
                        .UpdateTestCase(scenario.uuid,
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

        private void StartFixture(HookBinding hook, string containerId)
        {
            if (hook.HookType.ToString().StartsWith("Before"))
                allure.StartBeforeFixture(containerId, AllureHelper.NewId(), AllureHelper.GetFixtureResult(hook));
            else
                allure.StartAfterFixture(containerId, AllureHelper.NewId(), AllureHelper.GetFixtureResult(hook));
        }
        private static void StartStep(StepInfo stepInfo, string containerId)
        {
            var stepResult = new StepResult()
            {
                name = $"{stepInfo.StepDefinitionType} {stepInfo.Text}"
            };

            allure.StartStep(containerId, AllureHelper.NewId(), stepResult);

            if (stepInfo.Table != null)
            {
                var csvFile = $"{Guid.NewGuid().ToString()}.csv";
                using (var csv = new CsvWriter(File.CreateText(csvFile)))
                {
                    foreach (var item in stepInfo.Table.Header)
                    {
                        csv.WriteField(item);
                    }
                    csv.NextRecord();
                    foreach (var row in stepInfo.Table.Rows)
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
        }
        private static void WriteScenarios(IContextManager contextManager)
        {
            foreach (var s in contextManager.FeatureContext.Get<HashSet<TestResult>>())
            {
                allure.WriteTestCase(s.uuid);
            }

            foreach (var c in contextManager.FeatureContext.Get<HashSet<TestResultContainer>>())
            {
                allure
                    .StopTestContainer(c.uuid)
                    .WriteTestContainer(c.uuid);
            }
        }
    }
}