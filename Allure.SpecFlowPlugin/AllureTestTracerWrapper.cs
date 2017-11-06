using Allure.Commons;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.BindingSkeletons;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.Tracing;

namespace Allure.SpecFlowPlugin
{
    public class AllureTestTracerWrapper : TestTracer, ITestTracer
    {
        static AllureLifecycle allure = AllureLifecycle.Instance;

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

        void ITestTracer.TraceError(Exception ex)
        {
            TraceError(ex);
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
            allure.StopStep(x => x.status = Status.skipped);
            allure.UpdateTestCase(x => x.status = Status.skipped);
        }

        private static void StartStep(StepInstance stepInstance)
        {
            var stepResult = new StepResult
            {
                name = $"{stepInstance.Keyword} {stepInstance.Text}"
            };

            var table = stepInstance.TableArgument;

            // add step params for 1 row table
            if (table != null && table.RowCount == 1)
            {
                var paramNames = table.Header.ToArray();
                var parameters = new List<Parameter>();
                for (int i = 0; i < table.Header.Count; i++)
                {
                    parameters.Add(new Parameter {name = paramNames[i], value = table.Rows[0][i]});
                }
                stepResult.parameters = parameters;
            }

            allure.StartStep(AllureHelper.NewId(), stepResult);

            // add csv table for multi-row table
            if (table != null && table.RowCount != 1)
            {
                var csvFile = $"{Guid.NewGuid().ToString()}.csv";
                using (var csv = new CsvWriter(File.CreateText(csvFile)))
                {
                    foreach (var item in table.Header)
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
                }
                allure.AddAttachment("table", "text/csv", csvFile);
            }
        }

        private static void FailScenario(Exception ex)
        {
            allure.UpdateTestCase(
                x =>
                {
                    x.status = (x.status != Status.none) ? x.status : Status.failed;
                    x.statusDetails = new StatusDetails
                    {
                        message = ex.Message,
                        trace = ex.StackTrace
                    };
                });
        }
    }
}