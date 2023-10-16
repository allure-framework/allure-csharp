using System.Threading.Tasks;
using Allure.NUnit.Examples.CommonSteps;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples;

[AllureSuite("Tests - Async Steps")]
public class AllureAsyncStepTest : BaseTest
{
    private bool _isStep1Finished;

    [Test]
    [AllureName("Simple test with async steps")]
    public async Task CookPizzaStepByStepTest()
    {
        await AsyncStepsExamples.PrepareDough();
        await AsyncStepsExamples.CookPizza();
    }

    [Test]
    public async Task AwaitingStepsTaskCastTest()
    {
        await Step1();
        Assert.That(_isStep1Finished, Is.True);
    }

    [AllureStep("Step1")]
    public async Task Step1()
    {
        _isStep1Finished = false;
        await Task.Delay(500);
        _isStep1Finished = true;
    }
}
