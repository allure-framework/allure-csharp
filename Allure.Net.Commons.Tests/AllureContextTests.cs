using NUnit.Framework;

namespace Allure.Net.Commons.Tests
{
    class AllureContextTests
    {
        [Test]
        public void TestEmptyContext()
        {
            var ctx = new AllureContext();

            Assert.That(ctx.ContainerContext, Is.Empty);
            Assert.That(ctx.FixtureContext, Is.Null);
            Assert.That(ctx.TestContext, Is.Null);
            Assert.That(ctx.StepContext, Is.Empty);
            Assert.That(ctx.HasContainer, Is.False);
            Assert.That(ctx.ContainerContextDepth, Is.Zero);
            Assert.That(ctx.HasFixture, Is.False);
            Assert.That(ctx.HasTest, Is.False);
            Assert.That(ctx.HasStep, Is.False);
            Assert.That(ctx.StepContextDepth, Is.Zero);

            Assert.That(
                () => ctx.CurrentContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No container context is active."
                )
            );
            Assert.That(
                () => ctx.CurrentFixture,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No fixture context is active."
                )
            );
            Assert.That(
                () => ctx.CurrentTest,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No test context is active."
                )
            );
            Assert.That(
                () => ctx.CurrentStep,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No step context is active."
                )
            );
            Assert.That(
                () => ctx.CurrentStepContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No fixture, test, or step context is active."
                )
            );
        }

        [Test]
        public void TestContextOnly()
        {
            var test = new TestResult();

            var ctx = new AllureContext().WithTestContext(test);

            Assert.That(ctx.HasTest, Is.True);
            Assert.That(ctx.TestContext, Is.SameAs(test));
            Assert.That(ctx.CurrentTest, Is.SameAs(test));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(test));
        }

        [Test]
        public void CanNotAddContainerIfTestIsSet()
        {
            var ctx = new AllureContext()
                .WithTestContext(new());

            Assert.That(
                () => ctx.WithContainer(new()),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to change the container context because the " +
                        "test context is active."
                )
            );
        }

        [Test]
        public void CanNotAddContainerIfFixtureIsSet()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(new());

            Assert.That(
                () => ctx.WithContainer(new()),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to change the container context because the " +
                        "fixture context is active."
                )
            );
        }

        [Test]
        public void CanNotRemoveContainerIfTestIsSet()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithTestContext(new());

            Assert.That(
                ctx.WithNoLastContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to change the container context because the " +
                        "test context is active."
                )
            );
        }

        [Test]
        public void TestContextCanBeRemoved()
        {
            var test = new TestResult();

            var ctx = new AllureContext()
                .WithTestContext(test)
                .WithNoTestContext();

            Assert.That(ctx.HasTest, Is.False);
            Assert.That(ctx.TestContext, Is.Null);
            Assert.That(
                () => ctx.CurrentStepContainer,
                Throws.InvalidOperationException
            );
        }

        [Test]
        public void ContainerCanNotBeNull()
        {
            var ctx = new AllureContext();

            Assert.That(
                () => ctx.WithContainer(null),
                Throws.ArgumentNullException
            );
        }

        [Test]
        public void OneContainerInContainerContext()
        {
            var container = new TestResultContainer();

            var ctx = new AllureContext().WithContainer(container);

            Assert.That(ctx.HasContainer, Is.True);
            Assert.That(ctx.ContainerContextDepth, Is.EqualTo(1));
            Assert.That(ctx.ContainerContext, Is.EqualTo(new[] { container }));
            Assert.That(ctx.CurrentContainer, Is.SameAs(container));
        }

        [Test]
        public void SecondContainerIsPushedInFront()
        {
            var container1 = new TestResultContainer();
            var container2 = new TestResultContainer();

            var ctx = new AllureContext()
                .WithContainer(container1)
                .WithContainer(container2);

            Assert.That(
                ctx.ContainerContext,
                Is.EqualTo(new[] { container2, container1 })
            );
            Assert.That(ctx.ContainerContextDepth, Is.EqualTo(2));
            Assert.That(ctx.CurrentContainer, Is.SameAs(container2));
        }

        [Test]
        public void CanNotRemoveContainerIfNoneExist()
        {
            var ctx = new AllureContext();

            Assert.That(
                ctx.WithNoLastContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to deactivate the container context because " +
                        "it is not active."
                )
            );
        }

        [Test]
        public void LatestContainerCanBeRemoved()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithNoLastContainer();

            Assert.That(ctx.HasContainer, Is.False);
            Assert.That(ctx.ContainerContextDepth, Is.Zero);
            Assert.That(ctx.ContainerContext, Is.Empty);
        }

        [Test]
        public void IfContainerIsRemovedThePreviousOneBecomesActive()
        {
            var container = new TestResultContainer();
            var ctx = new AllureContext()
                .WithContainer(container)
                .WithContainer(new())
                .WithNoLastContainer();

            Assert.That(ctx.ContainerContext, Is.EqualTo(new[] { container }));
            Assert.That(ctx.CurrentContainer, Is.SameAs(container));
            Assert.That(ctx.ContainerContextDepth, Is.EqualTo(1));
        }

        [Test]
        public void FixtureContextRequiresContainer()
        {
            var fixture = new FixtureResult();

            var ctx = new AllureContext();

            Assert.That(
                () => ctx.WithFixtureContext(new()),
                Throws.InvalidOperationException
                    .With.Message.EqualTo(
                        "Unable to activate the fixture context because " +
                            "the container context is not active."
                    )
            );
        }

        [Test]
        public void FixtureCanNotBeNull()
        {
            var ctx = new AllureContext().WithContainer(new());

            Assert.That(
                () => ctx.WithFixtureContext(null),
                Throws.ArgumentNullException
            );
        }

        [Test]
        public void FixtureContextIsSet()
        {
            var fixture = new FixtureResult();

            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(fixture);

            Assert.That(ctx.HasFixture, Is.True);
            Assert.That(ctx.FixtureContext, Is.SameAs(fixture));
            Assert.That(ctx.CurrentFixture, Is.SameAs(fixture));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(fixture));
        }

        [Test]
        public void CanNotRemoveContainerIfFixtureIsSet()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(new());

            Assert.That(
                ctx.WithNoLastContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to change the container context because the " +
                        "fixture context is active."
                )
            );
        }

        [Test]
        public void FixturesCanNotBeNested()
        {
            var fixture = new FixtureResult();

            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(fixture);

            Assert.That(
                () => ctx.WithFixtureContext(new()),
                Throws.InvalidOperationException
                    .With.Message.EqualTo(
                        "Unable to activate the fixture context because " +
                            "it's already active."
                    )
            );
        }

        [Test]
        public void TestCanNotBeNull()
        {
            var ctx = new AllureContext();

            Assert.That(
                () => ctx.WithTestContext(null),
                Throws.ArgumentNullException
            );
        }

        [Test]
        public void TestsCanNotBeNested()
        {
            var test = new TestResult();

            var ctx = new AllureContext().WithTestContext(test);

            Assert.That(
                () => ctx.WithTestContext(new()),
                Throws.InvalidOperationException
                    .With.Message.EqualTo(
                        "Unable to activate the test context because " +
                            "it is already active."
                    )
            );
        }

        [Test]
        public void CanNotSetTestContextIfFixtureContextIsActive()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(new());

            Assert.That(
                () => ctx.WithTestContext(new()),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to activate the test context because the " +
                        "fixture context is active."
                )
            );
        }

        [Test]
        public void ClearingTestContextClearsFixtureContext()
        {
            var test = new TestResult();

            var ctx = new AllureContext()
                .WithContainer(new())
                .WithTestContext(test)
                .WithFixtureContext(new())
                .WithNoTestContext();

            Assert.That(ctx.HasFixture, Is.False);
            Assert.That(ctx.FixtureContext, Is.Null);
            Assert.That(
                () => ctx.CurrentStepContainer,
                Throws.InvalidOperationException
            );
        }

        [Test]
        public void SettingFixtureContextAfterTestAffectsStepContainer()
        {
            var fixture = new FixtureResult();

            var ctx = new AllureContext()
                .WithContainer(new())
                .WithTestContext(new())
                .WithFixtureContext(fixture);

            Assert.That(ctx.CurrentStepContainer, Is.SameAs(fixture));
        }

        [Test]
        public void FixtureContextCanBeCleared()
        {
            var fixture = new FixtureResult();

            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(fixture)
                .WithNoFixtureContext();

            Assert.That(ctx.HasFixture, Is.False);
            Assert.That(ctx.FixtureContext, Is.Null);
        }

        [Test]
        public void StepCanNotBeNull()
        {
            var ctx = new AllureContext().WithTestContext(new());

            Assert.That(
                () => ctx.WithStep(null),
                Throws.ArgumentNullException
            );
        }

        [Test]
        public void StepCanNotBeAddedIfNoTestOrFixtureExists()
        {
            var ctx = new AllureContext();

            Assert.That(
                () => ctx.WithStep(new()),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to activate the step context because neither " +
                        "test, nor fixture context is active."
                )
            );
        }

        [Test]
        public void StepCanNotBeRemovedIfNoStepExists()
        {
            var ctx = new AllureContext();

            Assert.That(
                () => ctx.WithNoLastStep(),
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to deactivate the step context because it " +
                        "isn't active."
                )
            );
        }

        [Test]
        public void StepCanBeAddedIfFixtureExists()
        {
            var step = new StepResult();
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(new())
                .WithStep(step);

            Assert.That(ctx.HasStep, Is.True);
            Assert.That(ctx.StepContext, Is.EqualTo(new[] { step }));
            Assert.That(ctx.StepContextDepth, Is.EqualTo(1));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(step));
        }

        [Test]
        public void StepCanBeAddedIfTestExists()
        {
            var step = new StepResult();
            var ctx = new AllureContext()
                .WithTestContext(new())
                .WithStep(step);

            Assert.That(ctx.HasStep, Is.True);
            Assert.That(ctx.StepContext, Is.EqualTo(new[] { step }));
            Assert.That(ctx.CurrentStep, Is.SameAs(step));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(step));
        }

        [Test]
        public void TwoStepsCanBeAdded()
        {
            var step1 = new StepResult();
            var step2 = new StepResult();
            var ctx = new AllureContext()
                .WithTestContext(new())
                .WithStep(step1)
                .WithStep(step2);

            Assert.That(ctx.StepContext, Is.EqualTo(new[] { step2, step1 }));
            Assert.That(ctx.StepContextDepth, Is.EqualTo(2));
            Assert.That(ctx.CurrentStep, Is.SameAs(step2));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(step2));
        }

        [Test]
        public void RemovingStepRestoresPreviousStepAsStepContainer()
        {
            var step1 = new StepResult();
            var step2 = new StepResult();
            var ctx = new AllureContext()
                .WithTestContext(new())
                .WithStep(step1)
                .WithStep(step2)
                .WithNoLastStep();

            Assert.That(ctx.StepContext, Is.EqualTo(new[] { step1 }));
            Assert.That(ctx.CurrentStep, Is.SameAs(step1));
            Assert.That(ctx.StepContextDepth, Is.EqualTo(1));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(step1));
        }

        [Test]
        public void RemovingTheOnlyStepRestoresTestAsStepContainer()
        {
            var test = new TestResult();
            var ctx = new AllureContext()
                .WithTestContext(test)
                .WithStep(new())
                .WithNoLastStep();

            Assert.That(ctx.HasStep, Is.False);
            Assert.That(ctx.StepContext, Is.Empty);
            Assert.That(ctx.StepContextDepth, Is.Zero);
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(test));
        }

        [Test]
        public void RemovingTheOnlyStepRestoresFixtureAsStepContainer()
        {
            var fixture = new FixtureResult();
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(fixture)
                .WithStep(new())
                .WithNoLastStep();

            Assert.That(ctx.StepContext, Is.Empty);
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(fixture));
        }

        [Test]
        public void RemovingFixtureClearsStepContext()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithFixtureContext(new())
                .WithStep(new())
                .WithNoFixtureContext();

            Assert.That(ctx.HasStep, Is.False);
            Assert.That(ctx.StepContext, Is.Empty);
        }

        [Test]
        public void RemovingTestClearsStepContext()
        {
            var ctx = new AllureContext()
                .WithTestContext(new())
                .WithStep(new())
                .WithNoTestContext();

            Assert.That(ctx.HasStep, Is.False);
            Assert.That(ctx.StepContext, Is.Empty);
        }

        [Test]
        public void FixtureAfterTestClearsStepContext()
        {
            // It is typical for some tear down fixtures to overlap with a
            // test. Once such a fixture is started, all steps left after the
            // test should be removed from the context.
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithTestContext(new())
                .WithStep(new())
                .WithFixtureContext(new());

            Assert.That(ctx.HasStep, Is.False);
            Assert.That(ctx.StepContext, Is.Empty);
        }

        [Test]
        public void ContextToString()
        {
            Assert.That(
                new AllureContext().ToString(),
                Is.EqualTo("AllureContext { Containers = [], Fixture = null, Test = null, Steps = [] }")
            );
            Assert.That(
                new AllureContext()
                    .WithContainer(new() { name = "c" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [c], Fixture = null, Test = null, Steps = [] }")
            );
            Assert.That(
                new AllureContext()
                    .WithContainer(new() { name = "c1" })
                    .WithContainer(new() { name = "c2" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [c2 <- c1], Fixture = null, Test = null, Steps = [] }")
            );
            Assert.That(
                new AllureContext()
                    .WithTestContext(new() { name = "t" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [], Fixture = null, Test = t, Steps = [] }")
            );
            Assert.That(
                new AllureContext()
                    .WithContainer(new() { name = "c" })
                    .WithFixtureContext(new() { name = "f" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [c], Fixture = f, Test = null, Steps = [] }")
            );
            Assert.That(
                new AllureContext()
                    .WithTestContext(new() { name = "t" })
                    .WithStep(new() { name = "s" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [], Fixture = null, Test = t, Steps = [s] }")
            );
            Assert.That(
                new AllureContext()
                    .WithTestContext(new() { name = "t" })
                    .WithStep(new() { name = "s1" })
                    .WithStep(new() { name = "s2" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [], Fixture = null, Test = t, Steps = [s2 <- s1] }")
            );
            Assert.That(
                new AllureContext()
                    .WithContainer(new() { uuid = "c" })
                    .WithTestContext(new() { uuid = "t" })
                    .ToString(),
                Is.EqualTo("AllureContext { Containers = [c], Fixture = null, Test = t, Steps = [] }")
            );
        }
    }
}
