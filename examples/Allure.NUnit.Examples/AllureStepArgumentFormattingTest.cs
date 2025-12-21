using System;
using Allure.Net.Commons;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples;

[AllureSuite("Tests - Steps")]
public class AllureStepArgumentFormattingTest : BaseTest
{
    [AllureStep("Step with params #{0}")]
    private static void StepWithCustomClassParams(CustomClass firstParam)
    {
        Console.WriteLine(firstParam);
    }

    [OneTimeSetUp]
    public static void OneTimeSetUp()
    {
        AllureLifecycle.Instance.AddTypeFormatter(new CustomClassFormatter());
    }

    [Test]
    [AllureName("Test with custom formatting for step argument")]
    public void StepWithCustomParamTest()
    {
        StepWithCustomClassParams(new CustomClass { I = 10, S = "string" });
    }

    private class CustomClass
    {
        public int I { get; init; }
        public string S { get; init; }
    }

    private class CustomClassFormatter : TypeFormatter<CustomClass>
    {
        public override string Format(CustomClass value) => $"___{value.S} + {value.I}___";
    }
}