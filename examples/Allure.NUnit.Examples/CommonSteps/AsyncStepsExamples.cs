using System;
using System.Threading.Tasks;
using Allure.NUnit.Attributes;

namespace Allure.NUnit.Examples.CommonSteps;

public static class AsyncStepsExamples
{
    private static readonly Random Random = new();
    
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

    [AllureStep("Deliver")]
    public static async Task DeliverPizza()
    {
        await AsyncMethod($"Step {nameof(DeliverPizza)}");
    }
    
    [AllureStep("Pay")]
    public static async Task Pay()
    {
        await AsyncMethod($"Step {nameof(Pay)}");
    }
    private static async Task AsyncMethod(string message)
    {
        var delay = Random.Next(50, 200);
        await Task.Delay(delay);
        Console.WriteLine($"{message}");
    }
}