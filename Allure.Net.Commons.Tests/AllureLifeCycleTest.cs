using NUnit.Framework;
using System;
using System.Threading.Tasks;

namespace Allure.Net.Commons.Tests
{
    [TestFixture]
    public class AllureLifeCycleTest
    {
        [Test, Description("ResultsDirectory property shouldn't be empty")]
        public void CheckResultsDirectory()
        {
            Assert.NotNull(new AllureLifecycle().ResultsDirectory);
        }

        [Test, Description("ExecutableItem.status default value should be 'none'")]
        public void ShouldSetDefaultStateAsNone()
        {
            Assert.AreEqual(Status.none, new TestResult().status);
        }

        [Test, Description("Integration Test")]
        public void IntegrationTest()
        {
            Parallel.For(0, 2, i =>
            {
                AllureLifecycle cycle = AllureLifecycle.Instance;
                var container = DataGenerator.GetTestResultContainer();
                var beforeFeature = DataGenerator.GetFixture(Fixture.BeforeFeature);
                var afterFeature = DataGenerator.GetFixture(Fixture.AfterFeature);
                var beforeScenario = DataGenerator.GetFixture(Fixture.BeforeScenario);
                var afterScenario = DataGenerator.GetFixture(Fixture.AfterScenario);
                var test = DataGenerator.GetTestResult();
                var fixtureStep = DataGenerator.GetStep();
                var step1 = DataGenerator.GetStep();
                var step2 = DataGenerator.GetStep();
                var step3 = DataGenerator.GetStep();
                var txtAttach = DataGenerator.GetAttachment(".txt");
                var txtAttachWithNoExt = DataGenerator.GetAttachment();

                cycle
                    .StartTestContainer(container)

                    .StartBeforeFixture(container.uuid, beforeFeature.uuid, beforeFeature.fixture)

                    .StartStep(fixtureStep.uuid, fixtureStep.step)
                    .StopStep(x => x.status = Status.passed)

                    .AddAttachment("text file", "text/xml", txtAttach.path)
                    .AddAttachment(txtAttach.path)
                    .UpdateFixture(beforeFeature.uuid, f => f.status = Status.passed)
                    .StopFixture(beforeFeature.uuid)

                    .StartBeforeFixture(container.uuid, beforeScenario.uuid, beforeScenario.fixture)
                    .UpdateFixture(beforeScenario.uuid, f => f.status = Status.passed)
                    .StopFixture(beforeScenario.uuid)

                    .StartTestCase(container.uuid, test)

                    .StartStep(step1.uuid, step1.step)
                    .StopStep(x => x.status = Status.passed)

                    .StartStep(step2.uuid, step2.step)
                    .AddAttachment("unknown file", "text/xml", txtAttachWithNoExt.content)
                    .StopStep(x => x.status = Status.broken)

                    .StartStep(step3.uuid, step3.step)
                    .StopStep(x => x.status = Status.skipped)

                    .AddScreenDiff(test.uuid, "expected.png", "actual.png", "diff.png")

                    .StopTestCase(x =>
                    {
                        x.status = Status.broken;
                        x.statusDetails = new StatusDetails()
                        {
                            flaky = true,
                            known = true,
                            message = "Oh my!",
                            trace = "That was really bad...",
                            muted = true
                        };
                    })

                    .StartAfterFixture(container.uuid, afterScenario.uuid, afterScenario.fixture)
                    .UpdateFixture(afterScenario.uuid, f => f.status = Status.passed)
                    .StopFixture(afterScenario.uuid)

                    .StartAfterFixture(container.uuid, afterFeature.uuid, afterFeature.fixture)
                    .StopFixture(f => f.status = Status.passed)

                    .WriteTestCase(test.uuid)
                    .StopTestContainer(container.uuid)
                    .WriteTestContainer(container.uuid);
            });
        }

        [Test, Description("Test step should be correctly added even if a " +
            "before fixture overlaps with the test")]
        public void BeforeFixtureMayOverlapsWithTest()
        {
            var writer = new InMemoryResultsWriter();
            var lifecycle = new AllureLifecycle(_ => writer);
            var container = new TestResultContainer
            {
                uuid = Guid.NewGuid().ToString()
            };
            var testResult = new TestResult
            {
                uuid = Guid.NewGuid().ToString()
            };
            var fixture = new FixtureResult { name = "fixture" };

            lifecycle.StartTestContainer(container)
                .StartTestCase(testResult)
                .StartBeforeFixture(
                    container.uuid,
                    new(),
                    out var fixtureId
                ).StopFixture(fixtureId)
                .StartStep(new(), out var stepId)
                .StopStep()
                .StopTestCase(testResult.uuid)
                .StopTestContainer(container.uuid)
                .WriteTestCase(testResult.uuid)
                .WriteTestContainer(container.uuid);

            Assert.That(writer.testContainers.Count, Is.EqualTo(1));
            Assert.That(writer.testContainers[0].uuid, Is.EqualTo(container.uuid));

            Assert.That(writer.testContainers[0].befores.Count, Is.EqualTo(1));
            Assert.That(writer.testContainers[0].befores[0].name, Is.EqualTo("fixture"));

            Assert.That(writer.testContainers[0].children.Count, Is.EqualTo(1));
            Assert.That(writer.testContainers[0].children[0], Is.EqualTo(testResult.uuid));

            Assert.That(writer.testResults.Count, Is.EqualTo(1));
            Assert.That(writer.testResults[0].uuid, Is.EqualTo(testResult.uuid));
        }
    }
}
