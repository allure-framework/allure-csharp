using System;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    public static class StepsExamples
    {
        [AllureStep]
        public static void Step1()
        {
            Console.WriteLine("1");
        }


        [AllureStep("Step 3 - with explicit name")]
        public static void Step3()
        {
            Console.WriteLine("3");
        }

        [AllureStep("Step with params #{0} and #{1}")]
        public static void StepWithParams(object firstParam, object lastParam)
        {
            Console.WriteLine(firstParam);
            Console.WriteLine(lastParam);
        }
    }

    [AllureSuite("Tests - Steps")]
    public class AllureStepTest : BaseTest
    {
        [Test]
        [AllureName("Simple test with steps")]
        public void SimpleStepTest()
        {
            StepsExamples.Step1();
            AllureApi.Step("Step2", () => { Console.WriteLine("Step 2"); });
            StepsExamples.Step3();
        }

        [Test]
        [AllureName("Complex test with steps")]
        public void WrappedStepTest()
        {
            AllureApi.Step("RootStep", () =>
            {
                StepsExamples.Step1();

                AllureApi.Step("Step2", () =>
                {
                    Console.WriteLine("2");
                    AllureApi.Step("Step in Step 2", () => { Console.WriteLine("Step in step 2"); });
                });

                StepsExamples.Step3();
            });
        }

       

        [Test]
        [AllureName("Test with parametrized steps")]
        public void SimpleStepTest2()
        {
            StepsExamples.StepWithParams("0", "1");
            StepsExamples.StepWithParams(1, 2);
            StepsExamples.StepWithParams(new[] { 1, 3, 5}, "array");
        }
    }
}