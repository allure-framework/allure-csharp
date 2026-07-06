using Allure.Testing;

namespace Allure.Xunit.v3.Tests.CustomLabels;

class GlobalLabelTests
{
    [Test]
    public async Task CheckIfGlobalLabelsConfigurationWorks(CancellationToken token)
    {
        var results = AllureSampleRunner.RunAsync(AllureSampleRegistry.GlobalLabels, new()
        {
            AllureConfiguration = new { allure = new { globalLabels = new { foo = "bar", baz = "qux" } } },
        }, token);

        await Assert.That(results).HasTestResults([
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.CustomLabels.GlobalLabels.TestClass.TestMethod1")
                .And.HasLabel((l) => l.HasName("foo").And.HasValue("bar"))
                .And.HasLabel((l) => l.HasName("baz").And.HasValue("qux"))
                ,
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.CustomLabels.GlobalLabels.TestClass.TestMethod2")
                .And.HasLabel((l) => l.HasName("foo").And.HasValue("bar"))
                .And.HasLabel((l) => l.HasName("baz").And.HasValue("qux"))
                ,
        ]);
    }

    [Test]
    public async Task CheckIfEnvironmentLabelsWork(CancellationToken token)
    {
        var results = AllureSampleRunner.RunAsync(AllureSampleRegistry.GlobalLabels, new()
        {
            EnvironmentVariables = new()
            {
                ["ALLURE_LABEL_foo"] = "bar",
                ["ALLURE_LABEL_baz"] = "qux",
            },
        }, token);

        await Assert.That(results).HasTestResults([
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.CustomLabels.GlobalLabels.TestClass.TestMethod1")
                .And.HasLabel((l) => l.HasName("foo").And.HasValue("bar"))
                .And.HasLabel((l) => l.HasName("baz").And.HasValue("qux"))
                ,
            (tr) => tr.HasName("Allure.Xunit.v3.Tests.Samples.CustomLabels.GlobalLabels.TestClass.TestMethod2")
                .And.HasLabel((l) => l.HasName("foo").And.HasValue("bar"))
                .And.HasLabel((l) => l.HasName("baz").And.HasValue("qux"))
                ,
        ]);
    }
}
