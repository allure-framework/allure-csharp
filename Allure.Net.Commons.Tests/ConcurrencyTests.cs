using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Allure.Net.Commons.Tests
{
    internal class ConcurrencyTests
    {
        InMemoryResultsWriter writer;
        AllureLifecycle lifecycle;
        int writes = 0;

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
            this.WrapInTest("test-1", () => RunThreads(
                () => this.AddStep("step-1"),
                () => this.AddStep("step-2")
            ));

            this.AssertTestWithSteps("test-1", "step-1", "step-2");
        }

        [Test]
        public async Task AsyncStepsOfTestAreIsolated()
        {
            await this.WrapInTestAsync("test-1", async () => await Task.WhenAll(
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
            var sync = new ManualResetEventSlim();

            this.WrapInTest(
                "test",
                () => this.WrapInStep(
                    "outer",
                    () => RunThreads(
                        BindEventSet(() => this.AddSteps((
                            "inner-1",
                            new object[] { "inner-1-1", "inner-1-2" }
                        )), sync),
                        BindEventWait (() => this.AddSteps((
                            "inner-2",
                            new object[] { "inner-2-1", "inner-2-2" }
                        )), sync)
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
                async () => await this.WrapInStepAsync(
                    "outer",
                    async () => await Task.WhenAll(
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

        static Action BindEventSet(Action fn, ManualResetEventSlim @event) => () =>
        {
            try
            {
                fn();
            }
            finally
            {
                @event.Set();
            }
        };

        static Action BindEventWait(Action fn, ManualResetEventSlim @event) => () =>
        {
            @event.Wait();
            fn();
        };

        async Task AddTestWithStepsAsync(string name, params object[] steps)
        {
            this.lifecycle
                .StartTestCase(new()
                {
                    name = name,
                    uuid = Guid.NewGuid().ToString(),
                    fullName = name
                });
            await Task.Delay(1);
            await this.AddStepsAsync(steps);
            this.lifecycle
                .StopTestCase()
                .WriteTestCase();
            writes++;
        }

        void WrapInTest(string testName, Action action)
        {
            this.lifecycle.StartTestCase(
                new()
                {
                    name = testName,
                    uuid = Guid.NewGuid().ToString(),
                    fullName = testName
                }
            );
            action();
            this.lifecycle
                .StopTestCase()
                .WriteTestCase();
        }

        void WrapInStep(string stepName, Action action)
        {
            this.lifecycle.StartStep(
                new() { name = stepName }
            );
            action();
            this.lifecycle
                .StopStep();
        }

        async Task WrapInStepAsync(string stepName, Func<Task> action)
        {
            this.lifecycle.StartStep(
                new() { name = stepName }
            );
            await action();
            this.lifecycle
                .StopStep();
        }

        async Task WrapInTestAsync(string testName, Func<Task> action)
        {
            this.lifecycle.StartTestCase(
                new()
                {
                    name = testName,
                    uuid = Guid.NewGuid().ToString(),
                    fullName = testName
                }
            );
            await Task.Delay(1);
            await action();
            this.lifecycle
                .StopTestCase()
                .WriteTestCase();
        }

        void AddTestWithSteps(string name, params object[] steps) =>
            this.WrapInTest(name, () => this.AddSteps(steps));

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
                new() { name = name }
            ).StopStep();
        }

        void AddStepWithSubsteps(string name, params object[] substeps)
        {
            this.lifecycle.StartStep(new() { name = name });
            this.AddSteps(substeps);
            this.lifecycle.StopStep();
        }

        async Task AddStepWithSubstepsAsync(string name, params object[] substeps)
        {
            this.lifecycle.StartStep(new() { name = name });
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
            var expectedCount = steps.Length;
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
