using Allure.Commons;
using System;
using System.Collections.Generic;
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

        public AllureBindings(FeatureContext featureContext, ScenarioContext scenarioContext)
        {
            this.featureContext = featureContext;
            this.scenarioContext = scenarioContext;
        }

        [BeforeFeature(Order = int.MinValue)]
        public static void BeforeFeature(FeatureContext featureContext)
        {
            // Starting point. Create feature container and fake scenario. Store in FeatureContext.
            var trc = new TestResultContainer()
            {
                uuid = Allure.Uuid,
                name = featureContext.FeatureInfo.Title
            };
            featureContext.Set(trc);
            Allure.Lifecycle.StartTestContainer(trc);

            // Create list of scenario containers
            featureContext.Set(new List<TestResultContainer>());

            //var scenario = Allure.GetScenario(featureContext.FeatureInfo, Allure.Uuid);
            //featureContext.Set(scenario);
        }

        [BeforeScenario(Order = int.MinValue)]
        public void BeforeScenario()
        {
            // Start empty scenario container
            var scenarioContainer = Allure.CreateContainer();
            Allure.Lifecycle.StartTestContainer(scenarioContainer);

            // Save container to scenario context
            scenarioContext.Set(scenarioContainer);

            //// Get container from FeatureContext
            //if (featureContext.TryGetValue(out TestResultContainer featureContainer))
            //{
            //    // Copy BeforeFeature fixtures to scenario container
            //    Allure.Lifecycle.UpdateTestContainer(scenarioContainer.uuid, x =>
            //    {
            //        x.start = featureContainer.start;
            //        x.befores = featureContainer.befores;
            //    });
            //}

            // Add scenario container to feature list
            if (featureContext.TryGetValue(out List<TestResultContainer> scenarioContainers))
            {
                scenarioContainers.Add(scenarioContainer);
                featureContext.Set(scenarioContainers);
            }

            // Start scenario and save into ScenarioContext
            var scenario = Allure.GetScenario(featureContext?.FeatureInfo, scenarioContext?.ScenarioInfo);
            scenarioContext.Set(scenario);
            Allure.Lifecycle.StartTestCase(scenarioContainer.uuid, scenario);
        }

        [AfterScenario(Order = int.MaxValue)]
        public void AfterScenario()
        {
            // Write scenario
            if (scenarioContext.TryGetValue(out TestResult testCase))
            {
                Allure.Lifecycle.WriteTestCase(testCase.uuid);
                scenarioContext.Remove(testCase.GetType().FullName);
            }
            // Stop container
            if (featureContext.TryGetValue(out List<TestResultContainer> scenarioContainers))
            {
                Allure.Lifecycle.StopTestContainer(scenarioContainers.Last().uuid);
            }

        }

        [AfterFeature(Order = int.MaxValue)]
        public static void AfterFeature(FeatureContext featureContext)
        {
            if (featureContext.TryGetValue(out TestResultContainer featureContainer))
            {
                // Write featureContainer
                Allure.Lifecycle.WriteTestContainer(featureContainer.uuid);

                // Write feature scenarios
                if (featureContext.TryGetValue(out List<TestResultContainer> scenarioContainers) && scenarioContainers.Count > 0)
                {
                    // Add BeforeFeature fixtures to the first scenario befores 
                    Allure.Lifecycle
                        .UpdateTestContainer(scenarioContainers.First().uuid, x =>
                        {
                            x.start = featureContainer.start;
                            x.befores.AddRange(featureContainer.befores);
                        });

                    // Add AfterFeature fixtures to the last scenario afters 
                    Allure.Lifecycle
                        .UpdateTestContainer(scenarioContainers.Last().uuid, x =>
                            {
                                x.afters.AddRange(featureContainer.afters);
                            })
                        .StopTestContainer(scenarioContainers.Last().uuid);

                    foreach (var container in scenarioContainers)
                    {
                        Allure.Lifecycle.WriteTestContainer(container.uuid);
                    }
                }
                
            }


        }
    }
}
