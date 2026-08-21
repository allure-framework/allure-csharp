using System.Threading.Tasks;
using Allure;
using Allure.Model;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Steps.StepApi
{
    public class AttributeSteps
    {
        [Fact]
        public async Task TestMethod()
        {
            Void();
            _ = Return();
            await Async();
            _ = await AsyncReturn();
            Named();
            Parameters(1, 2, 3, 4, 5);
        }

        [AllureStep]
        static void Void() { }

        [AllureStep]
        static int Return() => 1;

        [AllureStep]
        static async Task Async()
        {
            await Task.Yield();
        }

        [AllureStep]
        static async Task<int> AsyncReturn()
        {
            await Task.Yield();
            return 1;
        }

        [AllureStep("Renamed")]
        static void Named() { }

        [AllureStep]
        static void Parameters(
            int plain,
            [AllureParameter(Ignore = true)] int ignored,
            [AllureParameter(Name = "renamed")] int renamed,
            [AllureParameter(Mode = ParameterMode.Masked)] int masked,
            [AllureParameter(Mode = ParameterMode.Hidden)] int hidden
        ) { }
    }
}
