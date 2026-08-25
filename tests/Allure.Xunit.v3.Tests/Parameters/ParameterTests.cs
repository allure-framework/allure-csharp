using Allure.Testing;
using Allure.Testing.Assertions.Model;

namespace Allure.Xunit.v3.Tests.Parameters;

class ParameterTests
{
    [Test]
    public async Task CheckParameterAttributesOnTheoryParametersWork(CancellationToken token)
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.ParameterAttributesOnTheoryParameters, token);

        await Assert.That(results).HasSingleTestResult()
            .With.ParametersMatching([
                p => p.HasName("name1")
                    .And.HasValue("\"value-1\"")
                    .And.HasNoMode()
                    .And.HasExcluded(false),
                p => p.HasName("name2")
                    .And.HasValue("\"value-2\"")
                    .And.HasMode(AllureParameterMode.Masked)
                    .And.HasExcluded(false),
                p => p.HasName("name3")
                    .And.HasValue("\"value-3\"")
                    .And.HasMode(AllureParameterMode.Hidden)
                    .And.HasExcluded(false),
                p => p.HasName("name4")
                    .And.HasValue("\"value-4\"")
                    .And.HasNoMode()
                    .And.HasExcluded(true),
                p => p.HasName("name5")
                    .And.HasValue("\"value-5\"")
                    .And.HasMode(AllureParameterMode.Masked)
                    .And.HasExcluded(true),
            ]);
    }
}
