using System.Collections.Immutable;
using System.Threading.Tasks;
using Allure.Sdk.Registration;
using Xunit;
using Allure.Xunit;

namespace Allure.Xunit.v3.Tests.Samples.Generator.CustomStartupObject
{
    public class Program
    {
        public static async Task<int> Main(string[] args) =>
            await Allure.Xunit.AllureXunitEntryPoint.RunAsync(allure =>
            {
                allure.TransformConfiguration((cfg) => cfg.WithProperty(
                    c => c.GlobalLabels,
                    ImmutableDictionary<string, string>.Empty.Add("startup-object", "custom"),
                    (cfg, value) => cfg with { GlobalLabels = value }
                ));
            }, args);
    }

    public class TestClass
    {
        [Fact]
        public void TestMethod() { }
    }
}
