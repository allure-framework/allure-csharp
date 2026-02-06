using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.Net.Commons.Attributes;
using NUnit.Framework;

namespace Allure.NUnit.Tests.Samples.StepAttributes
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
            this.Parameters(1, 2, 3, 4, 5);
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
        void Parameters(
            int plain,
            [AllureParameter(Ignore = true)] int ignored,
            [AllureParameter(Name = "Bar")] int renamed,
            [AllureParameter(Mode = ParameterMode.Masked)] int masked,
            [AllureParameter(Mode = ParameterMode.Hidden)] int hidden
        ) { }
    }
}
