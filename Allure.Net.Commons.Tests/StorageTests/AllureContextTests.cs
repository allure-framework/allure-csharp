using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;
using NUnit.Framework;

namespace Allure.Net.Commons.Tests.StorageTests
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

            Assert.That(
                () => ctx.CurrentContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No container context has been set up."
                )
            );
            Assert.That(
                () => ctx.CurrentFixture,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No fixture context has been set up."
                )
            );
            Assert.That(
                () => ctx.CurrentTest,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No test context has been set up."
                )
            );
            Assert.That(
                () => ctx.CurrentStep,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No step context has been set up."
                )
            );
            Assert.That(
                () => ctx.CurrentStepContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "No fixture, test, or step context has been set up."
                )
            );
        }

        [Test]
        public void TestContextOnly()
        {
            var test = new TestResult();

            var ctx = new AllureContext().WithTestContext(test);

            Assert.That(ctx.TestContext, Is.SameAs(test));
            Assert.That(ctx.CurrentTest, Is.SameAs(test));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(test));
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
                    "Unable to exclude the latest container from the " +
                        "context because a test context exists."
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
            Assert.That(ctx.CurrentContainer, Is.SameAs(container2));
        }

        [Test]
        public void CanNotRemoveContainerIfNoneExist()
        {
            var ctx = new AllureContext();

            Assert.That(
                ctx.WithNoLastContainer,
                Throws.InvalidOperationException.With.Message.EqualTo(
                    "Unable to exclude the latest container from the " +
                        "context because no container context has been set up."
                )
            );
        }

        [Test]
        public void LatestContainerCanBeRemoved()
        {
            var ctx = new AllureContext()
                .WithContainer(new())
                .WithNoLastContainer();

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
                        "Unable to set up the fixture context because there " +
                            "is no container context."
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
                    "Unable to exclude the latest container from the " +
                        "context because a fixture context exists."
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
                        "Unable to set up the fixture context " +
                            "because another fixture context already exists."
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
                        "Unable to set up the test context " +
                            "because another test context already exists."
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
                    "Unable to set up the test context " +
                        "because a fixture context is currently active."
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
                    "Unable to set up the step context because no test or" +
                        "fixture context exists."
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
                    "Unable to exclude the latest step from the context " +
                        "because no step context has been set up."
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

            Assert.That(ctx.StepContext, Is.EqualTo(new[] { step }));
            Assert.That(ctx.CurrentStepContainer, Is.SameAs(step));
        }

        [Test]
        public void StepCanBeAddedIfTestExists()
        {
            var step = new StepResult();
            var ctx = new AllureContext()
                .WithTestContext(new())
                .WithStep(step);

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

            Assert.That(ctx.StepContext, Is.Empty);
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

            Assert.That(ctx.StepContext, Is.Empty);
        }

        [Test]
        public void RemovingTestClearsStepContext()
        {
            var ctx = new AllureContext()
                .WithTestContext(new())
                .WithStep(new())
                .WithNoTestContext();

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

            Assert.That(ctx.StepContext, Is.Empty);
        }
    }
}
