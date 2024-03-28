using System.Threading.Tasks;
using Allure.NUnit.Examples.CommonSteps;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureSuite("Tests - Async OneTime SetUp")]
    [Parallelizable(ParallelScope.All)]
    public class AllureAsyncOneTimeSetUpTests: BaseTest
    {
        [OneTimeSetUp]
        [AllureBefore]
        public async Task OneTimeSetUp()
        {
            await AsyncStepsExamples.PrepareDough();
            await AsyncStepsExamples.CookPizza();
            await AsyncStepsExamples.CookPizza();
            await AsyncStepsExamples.CookPizza();
        }

        [SetUp]
        [AllureBefore]
        public async Task SetUp()
        {
            await AsyncStepsExamples.PrepareDough();
        }

        [Test]
        [AllureName("Test1")]
        public async Task Test1()
        {
            await AsyncStepsExamples.DeliverPizza();
            await AsyncStepsExamples.Pay();
        }
        
        [Test]
        [AllureName("Test2")]
        public async Task Test2()
        {
            await AsyncStepsExamples.DeliverPizza();
        }
    }
}