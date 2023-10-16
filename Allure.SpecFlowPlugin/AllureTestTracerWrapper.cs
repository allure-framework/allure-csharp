using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Allure.Net.Commons;
using CsvHelper;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;


namespace Allure.SpecFlowPlugin
{
    public class AllureTestTracerWrapper : TestTracer, ITestTracer
    {
        static readonly AllureLifecycle allure = AllureLifecycle.Instance;
        static readonly PluginConfiguration pluginConfiguration =
            PluginHelper.PluginConfiguration;
        readonly string noMatchingStepMessage =
            "No matching definition found for this step";
        readonly string noMatchingStepMessageForTest =
            "No matching definition found for the step '{0}'";

        public AllureTestTracerWrapper(
            ITraceListener traceListener,
            IStepFormatter stepFormatter,
            IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider,
            SpecFlowConfiguration specFlowConfiguration
        ) : base(
            traceListener,
            stepFormatter,
            stepDefinitionSkeletonProvider,
            specFlowConfiguration
        )
        {
        }

        void ITestTracer.TraceStep(
            StepInstance stepInstance,
            bool showAdditionalArguments
        )
        {
            this.TraceStep(stepInstance, showAdditionalArguments);
            this.StartStep(stepInstance);
        }

        void ITestTracer.TraceStepDone(
            BindingMatch match,
            object[] arguments,
            TimeSpan duration
        )
        {
            this.TraceStepDone(match, arguments, duration);
            allure.StopStep(x => x.status = Status.passed);
        }

        void ITestTracer.TraceError(Exception ex, TimeSpan duration)
        {
            this.TraceError(ex, duration);
            allure.StopStep(
                PluginHelper.WrapStatusInit(Status.failed, ex)
            );
            FailScenario(ex);
        }

        void ITestTracer.TraceStepSkipped()
        {
            this.TraceStepSkipped();
            allure.StopStep(x => x.status = Status.skipped);
        }

        void ITestTracer.TraceStepPending(BindingMatch match, object[] arguments)
        {
            this.TraceStepPending(match, arguments);
            allure.StopStep(x => x.status = Status.skipped);
        }

        void ITestTracer.TraceNoMatchingStepDefinition(
            StepInstance stepInstance,
            ProgrammingLanguage targetLanguage,
            CultureInfo bindingCulture,
            List<BindingMatch> matchesWithoutScopeCheck
        )
        {
            this.TraceNoMatchingStepDefinition(
                stepInstance,
                targetLanguage,
                bindingCulture,
                matchesWithoutScopeCheck
            );
            allure.StopStep(
                PluginHelper.WrapStatusUpdate(Status.broken, new()
                {
                    message = noMatchingStepMessage
                })
            );
            allure.UpdateTestCase(
                PluginHelper.WrapStatusInit(Status.broken, new StatusDetails
                {
                    message = string.Format(
                        noMatchingStepMessageForTest,
                        stepInstance.Text
                    )
                })
            );
        }

        private void StartStep(StepInstance stepInstance)
        {
            var stepResult = new StepResult
            {
                name = $"{stepInstance.Keyword} {stepInstance.Text}"
            };


            // parse MultilineTextArgument
            if (stepInstance.MultilineTextArgument is not null)
            {
                allure.AddAttachment(
                    "multiline argument",
                    "text/plain",
                    Encoding.ASCII.GetBytes(
                        stepInstance.MultilineTextArgument
                    ),
                    ".txt"
                );
            }

            var table = stepInstance.TableArgument;
            var isTableProcessed = table is null;

            // parse table as step params
            if (table is not null)
            {
                var header = table.Header.ToArray();
                if (pluginConfiguration.stepArguments.convertToParameters)
                {
                    var parameters = new List<Parameter>();

                    // convert 2 column table into param-value
                    if (table.Header.Count == 2)
                    {
                        var paramNameMatch = Regex.IsMatch(
                            header[0],
                            pluginConfiguration.stepArguments.paramNameRegex
                        );
                        var paramValueMatch = Regex.IsMatch(
                            header[1],
                            pluginConfiguration.stepArguments.paramValueRegex
                        );
                        if (paramNameMatch && paramValueMatch)
                        {
                            for (var i = 0; i < table.RowCount; i++)
                            {
                                parameters.Add(new()
                                {
                                    name = table.Rows[i][0],
                                    value = table.Rows[i][1]
                                });
                            }

                            isTableProcessed = true;
                        }
                    }
                    // add step params for 1 row table
                    else if (table.RowCount == 1)
                    {
                        for (var i = 0; i < table.Header.Count; i++)
                        {
                            parameters.Add(new()
                            {
                                name = header[i],
                                value = table.Rows[0][i]
                            });
                        }

                        isTableProcessed = true;
                    }

                    stepResult.parameters = parameters;
                }
            }

            allure.StartStep(stepResult);

            // add csv table for multi-row table if was not processed as
            // params already
            if (isTableProcessed)
            {
                return;
            }

            using var ms = new MemoryStream();
            using var sw = new StreamWriter(ms, System.Text.Encoding.UTF8);
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            foreach (var item in table!.Header)
            {
                csv.WriteField(item);
            }

            csv.NextRecord();
            foreach (var row in table.Rows)
            {
                foreach (var item in row.Values)
                {
                    csv.WriteField(item);
                }

                csv.NextRecord();
            }

            sw.Flush();
            allure.AddAttachment("table", "text/csv", ms.ToArray(), ".csv");
        }

        private static void FailScenario(Exception ex)
        {
            allure.UpdateTestCase(x =>
            {
                x.status = x.status != Status.none ? x.status : Status.failed;
                x.statusDetails = PluginHelper.GetStatusDetails(ex);
            });
        }
    }
}
