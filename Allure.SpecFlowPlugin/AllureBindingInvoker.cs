using Allure.Commons;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

        public AllureBindingInvoker(SpecFlowConfiguration specFlowConfiguration, IErrorProvider errorProvider) : base(
            specFlowConfiguration, errorProvider)
        {
        }

        public override object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments,
            ITestTracer testTracer, out TimeSpan duration)
        {
            var hook = binding as HookBinding;

            // process hook
            if (hook != null)
            {
                var featureContainerId = PluginHelper.GetFeatureContainerId(contextManager.FeatureContext?.FeatureInfo);

                switch (hook.HookType)
                {
                    case HookType.BeforeFeature:
                        if (hook.HookOrder == int.MinValue)
                        {
                            // starting point
                            var featureContainer = new TestResultContainer
                            {
                                uuid = PluginHelper.GetFeatureContainerId(contextManager.FeatureContext?.FeatureInfo)
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
                                var result = base.InvokeBinding(binding, contextManager, arguments, testTracer,
                                    out duration);
                                allure.StopFixture(x => x.status = Status.passed);
                                return result;
                            }
                            catch (Exception ex)
                            {
                                allure.StopFixture(x => x.status = Status.broken);

                                // if BeforeFeature is failed execution is stopped. We need to create, update, stop and write everything here.

                                // create fake scenario container
                                var scenarioContainer =
                                    PluginHelper.StartTestContainer(contextManager.FeatureContext, null);

                                // start fake scenario
                                var scenario = PluginHelper.StartTestCase(scenarioContainer.uuid,
                                    contextManager.FeatureContext, null);

                                // update, stop and write
                                allure
                                    .StopTestCase(x =>
                                    {
                                        x.status = Status.broken;
                                        x.statusDetails = PluginHelper.GetStatusDetails(ex);

                                    })
                                    .WriteTestCase(scenario.uuid)
                                    .StopTestContainer(scenarioContainer.uuid)
                                    .WriteTestContainer(scenarioContainer.uuid)
                                    .StopTestContainer(featureContainerId)
                                    .WriteTestContainer(featureContainerId);

                                throw;
                            }

                    case HookType.BeforeStep:
                    case HookType.AfterStep:
                        {
                            var scenario = PluginHelper.GetCurrentTestCase(contextManager.ScenarioContext);

                            try
                            {
                                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                            }
                            catch (Exception ex)
                            {
                                allure
                                    .UpdateTestCase(scenario.uuid,
                                        x =>
                                        {
                                            x.status = Status.broken;
                                            x.statusDetails = PluginHelper.GetStatusDetails(ex);
                                        });
                                throw;
                            }
                        }

                    case HookType.BeforeScenario:
                    case HookType.AfterScenario:
                        if (hook.HookOrder == int.MinValue || hook.HookOrder == int.MaxValue)
                            return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                        else
                        {
                            var scenarioContainer = PluginHelper.GetCurrentTestConainer(contextManager.ScenarioContext);

                            try
                            {
                                this.StartFixture(hook, scenarioContainer.uuid);
                                var result = base.InvokeBinding(binding, contextManager, arguments, testTracer,
                                    out duration);
                                allure.StopFixture(x => x.status = Status.passed);
                                return result;
                            }
                            catch (Exception ex)
                            {
                                var status = (ex.GetType().Name.Contains(PluginHelper.IGNORE_EXCEPTION)) ?
                                        Status.skipped : Status.broken;

                                allure.StopFixture(x => x.status = status);

                                // get or add new scenario
                                var scenario = PluginHelper.GetCurrentTestCase(contextManager.ScenarioContext) ??
                                               PluginHelper.StartTestCase(scenarioContainer.uuid,
                                                   contextManager.FeatureContext, contextManager.ScenarioContext);

                                allure.UpdateTestCase(scenario.uuid,
                                    x =>
                                    {
                                        x.status = status;
                                        x.statusDetails = PluginHelper.GetStatusDetails(ex);
                                    });
                                throw;
                            }
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
                        {
                            try
                            {
                                StartFixture(hook, featureContainerId);
                                var result = base.InvokeBinding(binding, contextManager, arguments, testTracer,
                                    out duration);
                                allure.StopFixture(x => x.status = Status.passed);
                                return result;
                            }
                            catch (Exception ex)
                            {
                                var scenario = contextManager.FeatureContext.Get<HashSet<TestResult>>().Last();
                                allure
                                    .StopFixture(x => x.status = Status.broken)
                                    .UpdateTestCase(scenario.uuid,
                                        x =>
                                        {
                                            x.status = Status.broken;
                                            x.statusDetails = PluginHelper.GetStatusDetails(ex);
                                        });

                                WriteScenarios(contextManager);

                                allure
                                    .StopTestContainer(featureContainerId)
                                    .WriteTestContainer(featureContainerId);

                                throw;
                            }
                        }

                    case HookType.BeforeScenarioBlock:
                    case HookType.AfterScenarioBlock:
                    case HookType.BeforeTestRun:
                    case HookType.AfterTestRun:
                    default:
                        return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
                }
            }
            else
            {
                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
            }
        }

        private void StartFixture(HookBinding hook, string containerId)
        {
            if (hook.HookType.ToString().StartsWith("Before"))
                allure.StartBeforeFixture(containerId, PluginHelper.NewId(), PluginHelper.GetFixtureResult(hook));
            else
                allure.StartAfterFixture(containerId, PluginHelper.NewId(), PluginHelper.GetFixtureResult(hook));
        }

        private static void StartStep(StepInfo stepInfo, string containerId)
        {
            var stepResult = new StepResult
            {
                name = $"{stepInfo.StepDefinitionType} {stepInfo.Text}"
            };

            allure.StartStep(containerId, PluginHelper.NewId(), stepResult);

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