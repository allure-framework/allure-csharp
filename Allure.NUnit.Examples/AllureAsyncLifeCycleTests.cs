using System;
using System.Text;
using System.Threading.Tasks;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Allure.Core;
using NUnit.Framework;

namespace Allure.NUnit.Examples
{
    [AllureNUnit]
    public class AllureAsyncLifeCycleTests
    {
        [Test]
        public async Task AsyncTest_With_AllureStepAttribute()
        {
            Console.WriteLine("> Сalling async method with [AllureStep] ...");
            await StepWithAttribute();
            Console.WriteLine("  Done!");
        }

        [AllureStep("Calling StepWithAttribute")]
        private async Task StepWithAttribute()
        {
            // switching thread on async
            Console.WriteLine($"  > Delay...");
            await Task.Delay(1000);
            Console.WriteLine($"    Done!");

            // use internally allure steps storage on different thread
            Console.WriteLine($"  > AddAttachment...");
            AllureApi.AddAttachment("attachment-name", "text/plain", Encoding.UTF8.GetBytes("attachment-value"));
            Console.WriteLine($"    Done!");
        }

        [Test]
        public async Task AsyncTest_With_WrapInStep()
        {
            Console.WriteLine("> StepLevel1...");
            await AllureLifecycle.Instance.WrapInStepAsync(
                async () => await StepLevel1()
            );
            Console.WriteLine("  Done!");
        }

        private async Task StepLevel1()
        {
            Console.WriteLine("  > StepLevel2...");
            await AllureLifecycle.Instance.WrapInStepAsync(
                async () => await StepLevel2()
            );
            Console.WriteLine("    Done!");
        }

        private async Task StepLevel2()
        {
            // switching thread on async
            Console.WriteLine($"    > Sleep...");
            await Task.Delay(1000);
            Console.WriteLine($"      Done!");

            // use internally allure steps storage on different thread
            Console.WriteLine($"    > AddAttachment...");
            AllureApi.AddAttachment("attachment-name", "text/plain", Encoding.UTF8.GetBytes("attachment-value"));
            Console.WriteLine($"      Done!");
        }
    }
}