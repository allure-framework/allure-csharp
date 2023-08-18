using Allure.Net.Commons;
using TechTalk.SpecFlow;

namespace Allure.SpecFlowPlugin
{
    [Binding]
    public static class AllureBindings
    {
        static readonly AllureLifecycle allure = AllureLifecycle.Instance;

        [BeforeFeature(Order = int.MinValue)]
        public static void FirstBeforeFeature(FeatureContext featureContext) =>
            // Capturing the context allows us to access the container later in
            // AfterFeature hooks (it's executed by SpecFlow in a different
            // execution context).
            PluginHelper.CaptureAllureContext(
                featureContext,
                () => allure.StartTestContainer(new()
                {
                    uuid = PluginHelper.GetFeatureContainerId(
                        featureContext.FeatureInfo
                    )
                })
            );

        [AfterFeature(Order = int.MaxValue)]
        public static void LastAfterFeature() =>
            allure
                .StopTestContainer()
                .WriteTestContainer();

        [BeforeScenario(Order = int.MinValue)]
        public static void FirstBeforeScenario() =>
            PluginHelper.StartTestContainer();

        [BeforeScenario(Order = int.MaxValue)]
        public static void LastBeforeScenario(
            FeatureContext featureContext,
            ScenarioContext scenarioContext
        ) =>
            PluginHelper.StartTestCase(
                featureContext.FeatureInfo,
                scenarioContext.ScenarioInfo
            );

        [AfterScenario(Order = int.MinValue)]
        public static void FirstAfterScenario() => allure.StopTestCase();

        [AfterScenario(Order = int.MaxValue)]
        public static void LastAfterScenario(
            ScenarioContext scenarioContext
        ) =>
            allure.UpdateTestCase(
                PluginHelper.TestStatusResolver(scenarioContext)
            ).WriteTestCase()
                .StopTestContainer()
                .WriteTestContainer();
    }
}