using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace Allure.Commons.Test
{
    public class AllureLifeCycleTest
    {
        private readonly ITestOutputHelper output;
        AllureLifeсycle cycle = new AllureLifeсycle();

        public AllureLifeCycleTest(ITestOutputHelper output)
        {
            this.output = output;
        }

        [Fact(DisplayName = "ExecutableItem.status default value should be 'none'")]
        public void ShouldSetDefaultStateAsNone()
        {
            Assert.Equal(Status.none, new TestResult().status);
        }

        [Fact(DisplayName = "Integration Test")]
        public void IntegrationTest()
        {
            Parallel.For(0, 2, i =>
            {
                var container = DataGenerator.GetTestResultContainer();
                var beforeFeature = DataGenerator.GetFixture(Fixture.BeforeFeature);
                var afterFeature = DataGenerator.GetFixture(Fixture.AfterFeature);
                var beforeScenario = DataGenerator.GetFixture(Fixture.BeforeScenario);
                var afterScenario = DataGenerator.GetFixture(Fixture.AfterScenario);
                var test = DataGenerator.GetTestResult();
                var step1 = DataGenerator.GetStep();
                var step2 = DataGenerator.GetStep();
                var step3 = DataGenerator.GetStep();
                var txtAttach = DataGenerator.GetAttachment(".txt");
                var txtAttachWithNoExt = DataGenerator.GetAttachment();

                cycle
                    .StartTestContainer(container)

                    .StartBeforeFixture(container.uuid, beforeFeature.uuid, beforeFeature.fixture)
                    .UpdateFixture(beforeFeature.uuid, f => f.status = Status.passed)
                    .AddAttachment("text file", "text/xml", txtAttach.path)
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

                    .AddScreenDiff("expected.png", "actual.png", "diff.png")

                    .StopTestCase(x => {
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
