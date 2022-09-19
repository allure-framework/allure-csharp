using System;
using System.Threading.Tasks;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples;

public static class AsyncStepsExamples
{
    [AllureStep("Prepare dough")]
    public static async Task PrepareDough()
    {
        await AsyncMethod($"Step {nameof(PrepareDough)}");
    }

    [AllureStep("Cook pizza")]
    public static async Task CookPizza()
    {
        await AsyncMethod($"Step {nameof(CookPizza)}");
    }
    private static async Task AsyncMethod(string message)
    {
        await Task.Delay(3);
        Console.WriteLine($"{message}");
    }
}

[AllureSuite("Tests - Async Steps")]
public class AllureAsyncStepTest : BaseTest
{
    [Test]
    [AllureName("Simple test with async steps")]
    public async Task SimpleStepTest()
    {
        await AsyncStepsExamples.PrepareDough();
        await AsyncStepsExamples.CookPizza();
    }
}