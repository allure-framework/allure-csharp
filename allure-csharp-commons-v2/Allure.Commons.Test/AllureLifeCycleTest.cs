using System;
using Xunit;

namespace Allure.Commons.Test
{
    public class AllureLifeCycleTest
    {
        AllureLifeCycle cycle = new AllureLifeCycle();

        public AllureLifeCycleTest()
        {

        }

        [Fact]
        public void IntegrationTest()
        {
            var container = new TestResultContainer()
            {
                uuid = Guid.NewGuid().ToString("N")
            };

            var tr = new TestResult()
            {
                uuid = Guid.NewGuid().ToString("N"),
                description = "This is Description",
                fullName = "This is Full Name",
                name = "Test1",
            };
            //tr.testCaseId = tr.uuid;
            var tr2 = new TestResult()
            {
                uuid = Guid.NewGuid().ToString("N"),
                description = "This is Description",
                fullName = "This is Full Name",
                name = "Test2",
            };

            cycle
                .StartTestContainer(container)

                .ScheduleTestCase(container.uuid, tr)
                .StartTestCase(tr.uuid)
                
                .StartStep(Guid.NewGuid().ToString("N"), new StepResult() { name = "step1", status = Status.passed })
                
                .StartStep(Guid.NewGuid().ToString("N"), new StepResult() { name = "ste21", status = Status.failed })
                .StopStep()
                .StopStep()

                //.UpdateTestCase(t => t.status = Status.failed)
                .StopTestCase(tr.uuid)
                .WriteTestCase(tr.uuid)

                .StopTestContainer(container.uuid)
                .WriteTestContainer(container.uuid);
        }
    }
}
