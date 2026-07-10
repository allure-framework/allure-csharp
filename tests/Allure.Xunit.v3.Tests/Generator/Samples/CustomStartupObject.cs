using System.Threading.Tasks;
using Allure.TestingPlatform.Sdk.Registration;
using Xunit;
using Allure.Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Generator.CustomStartupObject
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
            await Allure.Xunit.AllureXunitEntryPoint.RunAsync(allure =>
            {
                allure.UseConfiguration(serviceProvider =>
                {
                    var configuration = AllureRegistrationDefaults.ReadAllureConfiguration(serviceProvider);
                    configuration.GlobalLabels["startup-object"] = "custom";
                    return configuration;
                });
            }, args);
    }

    public class TestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
