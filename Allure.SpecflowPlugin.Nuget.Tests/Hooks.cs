using Allure.Commons;
using TechTalk.SpecFlow;

namespace SpecFlowAllureNuget.Tests
{
    [Binding]
    public class Hooks
    {
        [BeforeTestRun]
        public static void BeforeTestRun()
        {
            AllureLifecycle.Instance.CleanupResultDirectory();
        }
    }
}
