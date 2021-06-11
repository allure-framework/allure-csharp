using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Allure.Commons;
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
        private static readonly AllureLifecycle allure = AllureLifecycle.Instance;
        private static readonly PluginConfiguration pluginConfiguration = PluginHelper.PluginConfiguration;
        private readonly string noMatchingStepMessage = "No matching step definition found for the step";

        public AllureTestTracerWrapper(ITraceListener traceListener, IStepFormatter stepFormatter,
            IStepDefinitionSkeletonProvider stepDefinitionSkeletonProvider, SpecFlowConfiguration specFlowConfiguration)
            : base(traceListener, stepFormatter, stepDefinitionSkeletonProvider, specFlowConfiguration)
        {
        }

        void ITestTracer.TraceStep(StepInstance stepInstance, bool showAdditionalArguments)
        {
            TraceStep(stepInstance, showAdditionalArguments);
            StartStep(stepInstance);
        }

        void ITestTracer.TraceStepDone(BindingMatch match, object[] arguments, TimeSpan duration)
        {
            TraceStepDone(match, arguments, duration);
            allure.StopStep(x => x.status = Status.passed);
        }

        void ITestTracer.TraceError(Exception ex, TimeSpan duration)
        {
            TraceError(ex, duration);
            allure.StopStep(x => x.status = Status.failed);
            FailScenario(ex);
        }

        void ITestTracer.TraceStepSkipped()
        {
            TraceStepSkipped();
            allure.StopStep(x => x.status = Status.skipped);
        }

        void ITestTracer.TraceStepPending(BindingMatch match, object[] arguments)
        {
            TraceStepPending(match, arguments);
            allure.StopStep(x => x.status = Status.skipped);
        }

        void ITestTracer.TraceNoMatchingStepDefinition(StepInstance stepInstance, ProgrammingLanguage targetLanguage,
            CultureInfo bindingCulture, List<BindingMatch> matchesWithoutScopeCheck)
        {
            TraceNoMatchingStepDefinition(stepInstance, targetLanguage, bindingCulture, matchesWithoutScopeCheck);
            allure.StopStep(x => x.status = Status.broken);
            allure.UpdateTestCase(x =>
            {
                x.status = Status.broken;
                x.statusDetails = new StatusDetails {message = noMatchingStepMessage};
            });
        }

        private void StartStep(StepInstance stepInstance)
        {
            var stepResult = new StepResult
            {
                name = $"{stepInstance.Keyword} {stepInstance.Text}"
            };


            // parse MultilineTextArgument
            if (stepInstance.MultilineTextArgument != null)
                allure.AddAttachment(
                    "multiline argument",
                    "text/plain",
                    Encoding.ASCII.GetBytes(stepInstance.MultilineTextArgument),
                    ".txt");

            var table = stepInstance.TableArgument;
            var isTableProcessed = table == null;

            // parse table as step params
            if (table != null)
            {
                var header = table.Header.ToArray();
                if (pluginConfiguration.stepArguments.convertToParameters)
                {
                    var parameters = new List<Parameter>();

                    // convert 2 column table into param-value
                    if (table.Header.Count == 2)
                    {
                        var paramNameMatch = Regex.IsMatch(header[0], pluginConfiguration.stepArguments.paramNameRegex);
                        var paramValueMatch =
                            Regex.IsMatch(header[1], pluginConfiguration.stepArguments.paramValueRegex);
                        if (paramNameMatch && paramValueMatch)
                        {
                            for (var i = 0; i < table.RowCount; i++)
                                parameters.Add(new Parameter {name = table.Rows[i][0], value = table.Rows[i][1]});

                            isTableProcessed = true;
                        }
                    }
                    // add step params for 1 row table
                    else if (table.RowCount == 1)
                    {
                        for (var i = 0; i < table.Header.Count; i++)
                            parameters.Add(new Parameter {name = header[i], value = table.Rows[0][i]});
                        isTableProcessed = true;
                    }

                    stepResult.parameters = parameters;
                }
            }

            allure.StartStep(PluginHelper.NewId(), stepResult);

            // add csv table for multi-row table if was not processed as params already
            if (isTableProcessed) return;
            using var sw = new StringWriter();
            using var csv = new CsvWriter(sw, CultureInfo.InvariantCulture);
            foreach (var item in table.Header) csv.WriteField(item);
            csv.NextRecord();
            foreach (var row in table.Rows)
            {
                foreach (var item in row.Values) csv.WriteField(item);
                csv.NextRecord();
            }

            allure.AddAttachment("table", "text/csv",
                Encoding.ASCII.GetBytes(sw.ToString()), ".csv");
        }

        private static void FailScenario(Exception ex)
        {
            allure.UpdateTestCase(
                x =>
                {
                    x.status = x.status != Status.none ? x.status : Status.failed;
                    x.statusDetails = PluginHelper.GetStatusDetails(ex);
                });
        }
    }
}