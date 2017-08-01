using Allure.Commons;
using CsvHelper;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin
{
    [Binding]
    public class AllureBindings
    {
        FeatureContext featureContext;
        ScenarioContext scenarioContext;
        Allure Allure => featureContext.Get<Allure>();
        public AllureBindings(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [BeforeFeature(Order = int.MinValue)]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            // Starting point.
            var Allure = new Allure();
            featureContext.Set(Allure);
            // Create feature container and fake scenario. Store in FeatureContext.
            var trc = Allure.CreateContainer();
            featureContext.Set(trc);

            Allure.Lifecycle.StartTestContainer(trc);

            // Create list of scenario containers
            featureContext.Set(new Dictionary<string, TestResultContainer>());

        }

        [BeforeScenario(Order = int.MinValue)]
        public void BeforeScenario()
        {
            // Start empty scenario container
            var scenarioContainer = Allure.CreateScenarioContainer(scenarioContext.ScenarioInfo);
            Allure.Lifecycle.StartTestContainer(scenarioContainer);

            // Save container to scenario context
            scenarioContext.Set(scenarioContainer);

            // Add scenario container to feature list
            if (featureContext.TryGetValue(out Dictionary<string, TestResultContainer> scenarioContainers))
            {
                scenarioContainers.Add(scenarioContainer.uuid, scenarioContainer);
                featureContext.Set(scenarioContainers);
            }

            // Start scenario and save into ScenarioContext
            var scenario = Allure.CreateTestResult(featureContext?.FeatureInfo, scenarioContext?.ScenarioInfo);
            scenarioContext.Set(scenario);
            Allure.Lifecycle.StartTestCase(scenarioContainer.uuid, scenario);
        }
        [BeforeStep(Order = int.MinValue)]
        public void BeforeStep()
        {
            var scenario = scenarioContext.Get<TestResult>();

            var stepInfo = scenarioContext.StepContext.StepInfo;
            var stepResult = new StepResult()
            {
                name = $"{stepInfo.StepDefinitionType} {stepInfo.Text}"
            };

            scenarioContext.StepContext.Set(stepResult);

            Allure.Lifecycle.StartStep(
                scenario.uuid, 
                Allure.GetStepId(scenarioContext), 
                stepResult);

            if (stepInfo.Table != null)
            {
                var csvFile = $"{Guid.NewGuid().ToString()}.csv";
                var a = stepInfo.Table.Rows.ToList();
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
                Allure.Lifecycle.AddAttachment("table", "text/csv", csvFile);
            }


        }

        [AfterStep(Order = int.MaxValue)]
        public void AfterStep()
        {
            if (scenarioContext.StepContext.TryGetValue(out StepResult stepResult))
            {
                var stepUuid = Allure.GetStepId(scenarioContext);

                Allure.Lifecycle
                    .UpdateStep(stepUuid, x =>
                        {
                            x.status = (scenarioContext.TestError == null) ? Status.passed : Status.failed;
                        })
                    .StopStep(stepUuid);

            }
        }

        [AfterScenario(Order = int.MaxValue)]
        public void AfterScenario()
        {
            // Stop and write scenario
            if (scenarioContext.TryGetValue(out TestResult testCase))
            {
                Allure.Lifecycle
                    .UpdateTestCase(testCase.uuid, x =>
                    {
                        x.status = (scenarioContext.TestError == null) ? Status.passed :
                        Status.failed;
                        x.statusDetails = (scenarioContext.TestError == null) ? null :
                        new StatusDetails()
                        {
                            message = scenarioContext.TestError?.Message,
                            trace = scenarioContext.TestError?.StackTrace
                        };
                    })
                    .StopTestCase(testCase.uuid);

                Allure.Lifecycle.WriteTestCase(testCase.uuid);

            }
            // Stop container
            if (featureContext.TryGetValue(out Dictionary<string, TestResultContainer> scenarioContainers))
            {
                Allure.Lifecycle.StopTestContainer(scenarioContainers.Last().Key);
            }

        }

        [AfterFeature(Order = int.MaxValue)]
        public static void AfterFeature(FeatureContext featureContext)
        {
            var Allure = featureContext.Get<Allure>();

            if (featureContext.TryGetValue(out TestResultContainer featureContainer))
            {
                // Write feature сontainer for failed BeforeFeature
                Allure.Lifecycle.WriteTestContainer(featureContainer.uuid);

                // Write feature scenarios
                if (featureContext.TryGetValue(out Dictionary<string, TestResultContainer> scenarioContainers)
                    && scenarioContainers.Count > 0)
                {
                    // Add BeforeFeature fixtures to the first scenario befores 
                    Allure.Lifecycle
                        .UpdateTestContainer(scenarioContainers.First().Key, x =>
                        {
                            x.start = featureContainer.start;
                            x.befores.AddRange(featureContainer.befores);
                        });

                    // Add AfterFeature fixtures to the last scenario afters 
                    Allure.Lifecycle
                        .UpdateTestContainer(scenarioContainers.Last().Key, x =>
                            {
                                x.afters.AddRange(featureContainer.afters);
                            })
                        .StopTestContainer(scenarioContainers.Last().Key);

                    foreach (var container in scenarioContainers)
                    {
                        Allure.Lifecycle.WriteTestContainer(container.Key);
                    }
                }

            }


        }
    }
}
