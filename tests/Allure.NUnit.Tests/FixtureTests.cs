using System.Text.Json.Nodes;
using Allure.Testing;

namespace Allure.NUnit.Tests;

class FixtureTests
{
    record class ContainerExpectation(
        string Name,
        List<StepTests.StepExpectations> Befores,
        List<StepTests.StepExpectations> Afters
    )
    {
        public bool Check(JsonObject fixture)
            => (string)fixture["name"] == this.Name
                && StepTests.StepExpectations.CheckAll(
                    this.Befores,
                    fixture["befores"].AsArray())
                && StepTests.StepExpectations.CheckAll(
                    this.Afters,
                    fixture["afters"].AsArray());
    }

    [Test]
    [Skip("Can't emit OneTime-fixture container: need sdk hook")]
    public async Task FixtureAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.FixtureAttributes);

        var testResults = results.TestResults.Cast<JsonObject>().ToArray();
        var containers = results.Containers.Cast<JsonObject>().ToArray();

        await Assert.That(containers).Count().IsEqualTo(2);
        await Assert.That(testResults).Count().IsEqualTo(1);
        var uuid = (string)testResults[0]["uuid"];
        await Assert.That(containers).Any(
            (c) => new ContainerExpectation(
                "Allure.NUnit.Tests.Samples.LegacyFixtureAttributes.TestsClass",
                [new StepTests.StepExpectations("OneTimeSetUp", "passed", [], [])],
                [new StepTests.StepExpectations("Bar", "passed", [], [])]
            ).Check(c)
        );
        await Assert.That(containers).Any(
            (c) => new ContainerExpectation(
                "Allure.NUnit.Tests.Samples.LegacyFixtureAttributes.TestsClass.TestMethod",
                [new StepTests.StepExpectations("Foo", "passed", [], [])],
                [new StepTests.StepExpectations("TearDown", "passed", [], [])]
            ).Check(c)
        );
    }

    [Test]
    public async Task LegacyFixtureAttributesWork()
    {
        var results = await AllureSampleRunner.RunAsync(AllureSampleRegistry.LegacyFixtureAttributes);

        var testResults = results.TestResults.Cast<JsonObject>().ToArray();
        var containers = results.Containers.Cast<JsonObject>().ToArray();

        await Assert.That(containers).Count().IsEqualTo(2);
        await Assert.That(testResults).Count().IsEqualTo(1);
        var uuid = (string)testResults[0]["uuid"];
        await Assert.That(containers).Any(
            (c) => new ContainerExpectation(
                "Allure.NUnit.Tests.Samples.LegacyFixtureAttributes.TestsClass",
                [new StepTests.StepExpectations("OneTimeSetUp", "passed", [], [])],
                [new StepTests.StepExpectations("Bar", "passed", [], [])]
            ).Check(c)
        );
        await Assert.That(containers).Any(
            (c) => new ContainerExpectation(
                "Allure.NUnit.Tests.Samples.LegacyFixtureAttributes.TestsClass.TestMethod",
                [new StepTests.StepExpectations("Foo", "passed", [], [])],
                [new StepTests.StepExpectations("TearDown", "passed", [], [])]
            ).Check(c)
        );
    }
}
