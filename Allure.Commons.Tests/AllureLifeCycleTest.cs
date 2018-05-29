﻿using NUnit.Framework;
using System.Threading.Tasks;

namespace Allure.Commons.Tests
{
    [TestFixture]
    public class AllureLifeCycleTest
    {
        [Test, Description("ResultsDirectory property shouldn't be empty")]
        public void CheckResultsDirectory()
        {
            Assert.NotNull(AllureLifecycle.Instance.ResultsDirectory);
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
    }
}
