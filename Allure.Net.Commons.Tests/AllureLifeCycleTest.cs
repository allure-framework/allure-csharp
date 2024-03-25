using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests
{
    [TestFixture]
    public class AllureLifeCycleTest
    {
        [Test, Description("ResultsDirectory property shouldn't be empty")]
        public void CheckResultsDirectory()
        {
            Assert.That(new AllureLifecycle().ResultsDirectory, Is.Not.Null);
        }

        [Test, Description("ExecutableItem.status default value should be 'none'")]
        public void ShouldSetDefaultStateAsNone()
        {
            Assert.That(new TestResult().status, Is.EqualTo(Status.none));
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

                    .StartBeforeFixture(beforeFeature.fixture)

                    .StartStep(fixtureStep.step)
                    .StopStep(x => x.status = Status.passed);

                AllureApi.AddAttachment("text file", "text/xml", txtAttach.path);
                AllureApi.AddAttachment(txtAttach.path);

                cycle
                    .UpdateFixture(f => f.status = Status.passed)
                    .StopFixture()

                    .StartBeforeFixture(beforeScenario.fixture)
                    .UpdateFixture(f => f.status = Status.passed)
                    .StopFixture()

                    .StartTestCase(test)

                    .StartStep(step1.step)
                    .StopStep(x => x.status = Status.passed)

                    .StartStep(step2.step);

                AllureApi.AddAttachment("unknown file", "text/xml", txtAttachWithNoExt.content);

                cycle
                    .StopStep(x => x.status = Status.broken)

                    .StartStep(step3.step)
                    .StopStep(x => x.status = Status.skipped);

                AllureApi.AddScreenDiff("expected.png", "actual.png", "diff.png");

                cycle
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

                    .StartAfterFixture(afterScenario.fixture)
                    .UpdateFixture(f => f.status = Status.passed)
                    .StopFixture()

                    .StartAfterFixture(afterFeature.fixture)
                    .StopFixture(f => f.status = Status.passed)

                    .WriteTestCase()
                    .StopTestContainer()
                    .WriteTestContainer();
            });
        }

        [Test, Description("A test step should be correctly added even if a " +
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
                .StartBeforeFixture(fixture)
                .StopFixture()
                .StartStep(new())
                .StopStep()
                .StopTestCase()
                .StopTestContainer()
                .WriteTestCase()
                .WriteTestContainer();

            Assert.That(writer.testContainers.Count, Is.EqualTo(1));
            Assert.That(writer.testContainers[0].uuid, Is.EqualTo(container.uuid));

            Assert.That(writer.testContainers[0].befores.Count, Is.EqualTo(1));
            Assert.That(writer.testContainers[0].befores[0].name, Is.EqualTo("fixture"));

            Assert.That(writer.testContainers[0].children.Count, Is.EqualTo(1));
            Assert.That(writer.testContainers[0].children[0], Is.EqualTo(testResult.uuid));

            Assert.That(writer.testResults.Count, Is.EqualTo(1));
            Assert.That(writer.testResults[0].uuid, Is.EqualTo(testResult.uuid));
        }

        [Test]
        public async Task ContextCapturingTest()
        {
            var writer = new InMemoryResultsWriter();
            var lifecycle = new AllureLifecycle(_ => writer);
            AllureContext context = null, modifiedContext = null;
            await Task.Factory.StartNew(() =>
            {
                lifecycle.StartTestCase(new()
                {
                    uuid = Guid.NewGuid().ToString()
                });
                context = lifecycle.Context;
            });
            modifiedContext = lifecycle.RunInContext(context, () =>
            {
                lifecycle.StopTestCase();
                lifecycle.WriteTestCase();
            });

            Assert.That(writer.testResults, Is.Not.Empty);
            Assert.That(modifiedContext.HasTest, Is.False);
        }

        [Test]
        public async Task ContextCapturingHasNoEffectIfContextIsNull()
        {
            var writer = new InMemoryResultsWriter();
            var lifecycle = new AllureLifecycle(_ => writer);
            await Task.Factory.StartNew(() =>
            {
                lifecycle.StartTestCase(new()
                {
                    uuid = Guid.NewGuid().ToString()
                });
            });

            Assert.That(() => lifecycle.RunInContext(null, () =>
            {
                lifecycle.StopTestCase();
            }), Throws.InvalidOperationException);
        }

        [Test]
        public void HistoryIdIsSetAfterStop()
        {
            var writer = new InMemoryResultsWriter();
            var lifecycle = new AllureLifecycle(_ => writer);
            lifecycle.StartTestCase(new()
            {
                uuid = "uuid",
                fullName = "full-name",
                parameters = new List<Parameter>
                {
                    new(){ name = "name", value = "value" }
                }
            });

            lifecycle.StopTestCase();

            Assert.That(lifecycle.Context.CurrentTest.historyId, Is.Not.Null);
        }

        [Test]
        public void HistoryIdNotOverwrittenAfterStop()
        {
            var writer = new InMemoryResultsWriter();
            var lifecycle = new AllureLifecycle(_ => writer);
            lifecycle.StartTestCase(new()
            {
                uuid = "uuid",
                fullName = "full-name",
                historyId = "history-id"
            });

            lifecycle.StopTestCase();

            Assert.That(lifecycle.Context.CurrentTest.historyId, Is.EqualTo("history-id"));
        }
    }
}
