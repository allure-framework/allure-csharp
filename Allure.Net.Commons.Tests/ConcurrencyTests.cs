using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Allure.Net.Commons.Tests
{
    internal class ConcurrencyTests
    {
        InMemoryResultsWriter writer;
        AllureLifecycle lifecycle;

        [SetUp]
        public void SetUp()
        {
            this.writer = new InMemoryResultsWriter();
            this.lifecycle = new AllureLifecycle(_ => this.writer);
        }

        [Test]
        public void ParallelTestsAreIsolated()
        {
            RunThreads(
                () => this.AddTestWithSteps("test-1", "step-1-1", "step-1-2"),
                () => this.AddTestWithSteps("test-2", "step-2-1", "step-2-2")
            );

            this.AssertTestWithSteps("test-1", "step-1-1", "step-1-2");
            this.AssertTestWithSteps("test-2", "step-2-1", "step-2-2");
        }

        [Test]
        public async Task AsyncTestsAreIsolated()
        {
            await Task.WhenAll(
                this.AddTestWithStepsAsync("test-1", "step-1-1", "step-1-2"),
                this.AddTestWithStepsAsync("test-2", "step-2-1", "step-2-2"),
                this.AddTestWithStepsAsync("test-3", "step-3-1", "step-3-2")
            );

            this.AssertTestWithSteps("test-1", "step-1-1", "step-1-2");
            this.AssertTestWithSteps("test-2", "step-2-1", "step-2-2");
            this.AssertTestWithSteps("test-3", "step-3-1", "step-3-2");
        }

        [Test]
        public void ParallelStepsOfTestAreIsolated()
        {
            this.WrapInTest("test-1", _ => RunThreads(
                () => this.AddStep("step-1"),
                () => this.AddStep("step-2")
            ));

            this.AssertTestWithSteps("test-1", "step-1", "step-2");
        }

        [Test]
        public async Task AsyncStepsOfTestAreIsolated()
        {
            await this.WrapInTestAsync("test-1", async _ => await Task.WhenAll(
                this.AddStepsAsync("step-1"),
                this.AddStepsAsync("step-2"),
                this.AddStepsAsync("step-3")
            ));

            this.AssertTestWithSteps("test-1", "step-1", "step-2", "step-3");
        }

        [Test]
        public void ContextCapturedBySubThreads()
        {
            /*
             * test               | Parent thread
             *   - outer          | Parent thread
             *     - inner-1      | Child thread 1
             *       - inner-1-1  | Child thread 1
             *       - inner-1-2  | Child thread 1
             *     - inner-2      | Child thread 2
             *       - inner-2-1  | Child thread 2
             *       - inner-2-2  | Child thread 2
             */
            this.WrapInTest(
                "test",
                _ => this.WrapInStep(
                    "outer",
                    _ => RunThreads(
                        () => this.AddSteps(
                            ("inner-1", new object[] { "inner-1-1", "inner-1-2" })
                        ),
                        () => this.AddSteps(
                            ("inner-2", new object[] { "inner-2-1", "inner-2-2" })
                        )
                    )
                )
            );

            this.AssertTestWithSteps(
                "test",
                ("outer", new object[]
                {
                    ("inner-1", new object[] { "inner-1-1", "inner-1-2" }),
                    ("inner-2", new object[] { "inner-2-1", "inner-2-2" })
                })
            );
        }

        [Test]
        public async Task ContextCapturedBySubTasks()
        {
            /*
             * test               | Parent task
             *   - outer          | Parent task
             *     - inner-1      | Child task 1
             *       - inner-1-1  | Child task 1
             *       - inner-1-2  | Child task 1
             *     - inner-2      | Child task 2
             *       - inner-2-1  | Child task 2
             *       - inner-2-2  | Child task 2
             */
            await this.WrapInTestAsync(
                "test",
                async _ => await this.WrapInStepAsync(
                    "outer",
                    async _ => await Task.WhenAll(
                        this.AddStepsAsync(
                            ("inner-1", new object[] { "inner-1-1", "inner-1-2" })
                        ),
                        this.AddStepsAsync(
                            ("inner-2", new object[] { "inner-2-1", "inner-2-2" })
                        )
                    )
                )
            );

            this.AssertTestWithSteps(
                "test",
                ("outer", new object[]
                {
                    ("inner-1", new object[] { "inner-1-1", "inner-1-2" }),
                    ("inner-2", new object[] { "inner-2-1", "inner-2-2" })
                })
            );
        }

        async Task AddTestWithStepsAsync(string name, params object[] steps)
        {
            var uuid = Guid.NewGuid().ToString();
            this.lifecycle
                .StartTestCase(new() { name = name, uuid = uuid });
            await Task.Delay(1);
            await this.AddStepsAsync(steps);
            this.lifecycle
                .StopTestCase(uuid)
                .WriteTestCase(uuid);
        }

        void WrapInTest(string testName, Action<string> action)
        {
            var uuid = Guid.NewGuid().ToString();
            this.lifecycle.StartTestCase(
                new() { name = testName, uuid = uuid }
            );
            action(uuid);
            this.lifecycle
                .StopTestCase(uuid)
                .WriteTestCase(uuid);
        }

        void WrapInStep(string stepName, Action<string> action)
        {
            this.lifecycle.StartStep(
                new() { name = stepName },
                out var stepId
            );
            action(stepId);
            this.lifecycle
                .StopStep();
        }

        async Task WrapInStepAsync(string stepName, Func<string, Task> action)
        {
            this.lifecycle.StartStep(
                new() { name = stepName },
                out var stepId
            );
            await action(stepId);
            this.lifecycle
                .StopStep();
        }

        async Task WrapInTestAsync(string testName, Func<string, Task> action)
        {
            var uuid = Guid.NewGuid().ToString();
            this.lifecycle.StartTestCase(
                new() { name = testName, uuid = uuid }
            );
            await Task.Delay(1);
            await action(uuid);
            this.lifecycle
                .StopTestCase(uuid)
                .WriteTestCase(uuid);
        }

        void AddTestWithSteps(string name, params object[] steps) =>
            this.WrapInTest(name, _ => this.AddSteps(steps));

        async Task AddStepsAsync(params object[] steps)
        {
            foreach (var step in steps)
            {
                if (step is string simpleStepName)
                {
                    this.AddStep(simpleStepName);
                }
                else if (step is (string complexStepName, object[] substeps))
                {
                    await this.AddStepWithSubstepsAsync(complexStepName, substeps);
                }

                await Task.Delay(1);
            }
        }

        void AddSteps(params object[] steps)
        {
            foreach (var step in steps)
            {
                if (step is string simpleStepName)
                {
                    this.AddStep(simpleStepName);
                }
                else if (step is (string complexStepName, object[] substeps))
                {
                    this.AddStepWithSubsteps(complexStepName, substeps);
                }
            }
        }

        void AddStep(string name)
        {
            this.lifecycle.StartStep(
                new() { name = name },
                out var _
            ).StopStep();
        }

        void AddStepWithSubsteps(string name, params object[] substeps)
        {
            this.lifecycle.StartStep(new() { name = name }, out var _);
            this.AddSteps(substeps);
            this.lifecycle.StopStep();
        }

        async Task AddStepWithSubstepsAsync(string name, params object[] substeps)
        {
            this.lifecycle.StartStep(new() { name = name }, out var _);
            await this.AddStepsAsync(substeps);
            this.lifecycle.StopStep();
        }

        void AssertTestWithSteps(string testName, params object[] steps)
        {
            Assert.That(
                this.writer.testResults.Select(tr => tr.name),
                Contains.Item(testName)
            );
            var test = this.writer.testResults.Single(tr => tr.name == testName);
            this.AssertSteps(test.steps, steps);
        }

        void AssertSteps(List<StepResult> actualSteps, params object[] steps)
        {
            var expectedCount = steps.Count();
            Assert.That(actualSteps.Count, Is.EqualTo(expectedCount));
            for (var i = 0; i < expectedCount; i++)
            {
                var actualStep = actualSteps[i];
                var step = steps.ElementAt(i);
                if (!(step is (string expectedStepName, object[] substeps)))
                {
                    expectedStepName = (string)step;
                    substeps = Array.Empty<object>();
                }
                Assert.That(actualStep.name, Is.EqualTo(expectedStepName));
                this.AssertSteps(actualStep.steps, substeps);
            }
        }

        static void RunThreads(params Action[] jobs)
        {
            var errors = new List<Exception>();
            var threads = jobs.Select(
                j => new Thread(
                    WrapThreadCallbackError(j, errors)
                )
            ).ToList();
            foreach(var thread in threads)
            {
                thread.Start();
            }
            foreach (var thread in threads)
            {
                thread.Join();
            }

            Assert.That(errors, Is.Empty);
        }

        static ThreadStart WrapThreadCallbackError(
            Action callback,
            List<Exception> errors
        ) => () =>
        {
            try
            {
                callback();
            }
            catch (Exception ex)
            {
                errors.Add(ex);
            }
        };
    }
}
