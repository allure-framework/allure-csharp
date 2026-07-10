using System.Threading.Tasks;
using Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Generator.RunnerHelper
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
            await Allure.Xunit.AllureXunitRunner.RunAsync(
                SelfRegisteredExtensions.AddSelfRegisteredExtensions,
                args
            );
    }

    public class TestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
