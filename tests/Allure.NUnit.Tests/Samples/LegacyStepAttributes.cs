using System.Threading.Tasks;
using Allure.NUnit.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.LegacyStepAttributes
{
    [AllureNUnit]
    public class TestsClass
    {
        [Test]
        public async Task TestMethod()
        {
            this.Void();
            this.Return();
            await this.Async();
            await this.AsyncReturn();
            this.Named();
            this.Parameters(1, "baz");
            this.SkippedParameter(2);
            this.RenamedParameter(3);
        }

        [AllureStep]
        void Void() { }

        [AllureStep]
        int Return() => 1;

        [AllureStep]
        async Task Async()
        {
            await Task.Delay(1);
        }

        [AllureStep]
        async Task<int> AsyncReturn()
        {
            await Task.Delay(1);
            return 1;
        }

        [AllureStep("Renamed")]
        void Named() { }

        [AllureStep]
        void Parameters(int foo, string bar) { }

        [AllureStep]
        void SkippedParameter([Skip] int foo) { }

        [AllureStep]
        void RenamedParameter([Name("Bar")] int foo) { }
    }
}
