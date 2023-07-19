using System;
using System.Collections.Immutable;
using System.Linq;
using System.Text;

#nullable enable

namespace Allure.Net.Commons.Storage
{
    /// <summary>
    /// Represents allure-related contextual information required to collect
    /// the report data during a test execution. Comprises four contexts:
    /// container, fxiture, test, and step, as well as methods to query and
    /// modify them.
    /// </summary>
    /// <remarks>
    /// Instances of this class are immutable to ensure proper isolation
    /// between different tests and steps that may potentially be run
    /// cuncurrently either by a test framework or by an end user.
    /// </remarks>
    public record class AllureContext
    {
        /// <summary>
        /// Returns true if a container context is active.
        /// </summary>
        public bool HasContainer => !this.ContainerContext.IsEmpty;

        /// <summary>
        /// Returns true if a fixture context is active.
        /// </summary>
        public bool HasFixture => this.FixtureContext is not null;

        /// <summary>
        /// Returns true if a test context is active.
        /// </summary>
        public bool HasTest => this.TestContext is not null;

        /// <summary>
        /// Returns true if a step context is active.
        /// </summary>
        public bool HasStep => !this.StepContext.IsEmpty;

        /// <summary>
        /// A stack of fixture containers affecting subsequent tests.
        /// </summary>
        /// <remarks>
        /// Activating this context allows operations on the current container
        /// (including adding a fixture to or removing a fixture from the
        /// current container).
        /// </remarks>
        internal IImmutableStack<TestResultContainer> ContainerContext
        {
            get;
            private init;
        } = ImmutableStack<TestResultContainer>.Empty;

        /// <summary>
        /// A fixture that is being currently executed.
        /// </summary>
        /// <remarks>
        /// Activating this context allows operations on the current fixture
        /// result.<br/>
        /// This property differs from <see cref="CurrentFixture"/> in that
        /// instead of throwing it returns null if a fixture context isn't
        /// active.
        /// </remarks>
        internal FixtureResult? FixtureContext { get; private init; }

        /// <summary>
        /// A test that is being executed.
        /// </summary>
        /// <remarks>
        /// Activating this context allows operations on the current test
        /// result.<br/>
        /// 
        /// This property differs from <see cref="CurrentTest"/> in that
        /// instead of throwing it returns null if a test context isn't active.
        /// </remarks>
        internal TestResult? TestContext { get; private init; }

        /// <summary>
        /// A stack of nested steps that are being executed.
        /// </summary>
        /// <remarks>
        /// Activating this context allows operations on the current step.
        /// </remarks>
        internal IImmutableStack<StepResult> StepContext
        {
            get;
            private init;
        } = ImmutableStack<StepResult>.Empty;

        /// <summary>
        /// The most recently added container from the container context.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if a container
        /// context isn't active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        internal TestResultContainer CurrentContainer
        {
            get => this.ContainerContext.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "No container context is active."
                );
        }

        /// <summary>
        /// A fixture that is being executed.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if a fixture
        /// context isn't active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        internal FixtureResult CurrentFixture =>
            this.FixtureContext ?? throw new InvalidOperationException(
                "No fixture context is active."
            );

        /// <summary>
        /// A test that is being executed.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if a test context
        /// isn't active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        internal TestResult CurrentTest =>
            this.TestContext ?? throw new InvalidOperationException(
                "No test context is active."
            );

        /// <summary>
        /// A step that is being executed.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if a step context
        /// isn't active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        internal StepResult CurrentStep =>
            this.StepContext.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "No step context is active."
                );

        /// <summary>
        /// A step container a next step should be put in.
        /// </summary>
        /// <remarks>
        /// A step container can be a fixture, a test of an another step.<br/>
        /// It throws <see cref="InvalidOperationException"/> if neither
        /// fixture, nor test, nor step context is active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        internal ExecutableItem CurrentStepContainer =>
            this.StepContext.FirstOrDefault() as ExecutableItem
                ?? this.RootStepContainer
                ?? throw new InvalidOperationException(
                    "No fixture, test, or step context is active."
                );

        protected virtual bool PrintMembers(StringBuilder stringBuilder)
        {
            var containers =
                RepresentStack(this.ContainerContext, c => c.name);
            var fixture = this.FixtureContext?.name ?? "null";
            var test = this.TestContext?.name ?? "null";
            var steps = RepresentStack(this.StepContext, s => s.name);

            stringBuilder.AppendFormat("Containers = [{0}], ", containers);
            stringBuilder.AppendFormat("Fixture = {0}, ", fixture);
            stringBuilder.AppendFormat("Test = {0}, ", test);
            stringBuilder.AppendFormat("Steps = [{0}]", steps);
            return true;
        }

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the active container
        /// context and the specified container pushed on top of it.
        /// </summary>
        /// <remarks>
        /// Can't be called if a fixture or a test context is active.
        /// </remarks>
        /// <param name="container">
        /// A container to push on top of the container context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified 
        /// (always active) container context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        internal AllureContext WithContainer(TestResultContainer container) =>
            this.ValidateContainerContextCanBeModified() with
            {
                ContainerContext = this.ContainerContext.Push(
                    container ?? throw new ArgumentNullException(
                        nameof(container)
                    )
                )
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> without the most recently
        /// added container in its container context. Requires an active
        /// container context. Deactivates a container context if it consists
        /// of one container only before the call.
        /// </summary>
        /// <remarks>
        /// Can't be called if a fixture or a test context is active.
        /// </remarks>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// (possibly inactive) container context.
        /// </returns>
        /// <exception cref="InvalidOperationException"/>
        internal AllureContext WithNoLastContainer() =>
            this with
            {
                ContainerContext = this.ValidateContainerCanBeRemoved()
                    .ContainerContext.Pop()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the active fixture
        /// context that is set to the specified fixture. Requires an active
        /// container context.
        /// </summary>
        /// <param name="fixtureResult">
        /// A new fixture context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// (always active) fixture context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        internal AllureContext WithFixtureContext(FixtureResult fixtureResult) =>
            this with
            {
                FixtureContext = this.ValidateNewFixtureContext(
                    fixtureResult ?? throw new ArgumentNullException(
                        nameof(fixtureResult)
                    )
                ),
                StepContext = this.StepContext.Clear()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with inactive fixture and
        /// step contexts.
        /// </summary>
        internal AllureContext WithNoFixtureContext() =>
            this with
            {
                FixtureContext = null,
                StepContext = this.StepContext.Clear()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the active test
        /// context that is set to the specified test result.
        /// Can't be used if a fixture context is active.
        /// </summary>
        /// <param name="testResult">
        /// A new test context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// (always active) test context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        internal AllureContext WithTestContext(TestResult testResult) =>
            this with
            {
                TestContext = this.ValidateNewTestContext(
                    testResult ?? throw new ArgumentNullException(
                        nameof(testResult)
                    )
                )
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with inactive test,
        /// fixture and step contexts.
        /// </summary>
        internal AllureContext WithNoTestContext() =>
            this with
            {
                FixtureContext = null,
                TestContext = null,
                StepContext = this.StepContext.Clear()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the active step
        /// context and the specified step result pushed on top of it.
        /// </summary>
        /// <remarks>
        /// Can't be called if neither fixture, nor test context is active.
        /// </remarks>
        /// <param name="stepResult">
        /// A new step result to push on top of the step context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// (always active) step context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        internal AllureContext WithStep(StepResult stepResult) =>
            this with
            {
                StepContext = this.StepContext.Push(
                    this.ValidateNewStep(
                        stepResult ?? throw new ArgumentNullException(
                            nameof(stepResult)
                        )
                    )
                )
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> without the most recently
        /// added step in its step context. Requires an active step context.
        /// Deactivates a step context if it consists of one step only before
        /// the call.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// (possibly inactive) step context.
        /// </returns>
        /// <exception cref="InvalidOperationException"/>
        internal AllureContext WithNoLastStep() =>
            this with
            {
                StepContext = this.HasStep
                    ? this.StepContext.Pop()
                    : throw new InvalidOperationException(
                        "Unable to deactivate a step context because it's " +
                            "already inactive."
                    )
            };

        AllureContext ValidateContainerContextCanBeModified()
        {
            if (this.FixtureContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to change a container context because a " +
                        "fixture context is active."
                );
            }

            if (this.TestContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to change a container context because a test " +
                        "context is active."
                );
            }

            return this;
        }

        AllureContext ValidateContainerCanBeRemoved()
        {
            if (!this.HasContainer)
            {
                throw new InvalidOperationException(
                    "Unable to deactivate a container context because it's " +
                        "inactive."
                );
            }

            return this.ValidateContainerContextCanBeModified();
        }

        ExecutableItem? RootStepContainer
        {
            get => this.FixtureContext as ExecutableItem ?? this.TestContext;
        }

        FixtureResult ValidateNewFixtureContext(FixtureResult fixture)
        {
            if (!this.HasContainer)
            {
                throw new InvalidOperationException(
                    "Unable to activate a fixture context " +
                        "because a container context is inactive."
                );
            }

            if (this.HasFixture)
            {
                throw new InvalidOperationException(
                    "Unable to activate a fixture context " +
                        "because another fixture context is active."
                );
            }

            return fixture;
        }

        TestResult ValidateNewTestContext(TestResult testResult)
        {
            if (this.HasFixture)
            {
                throw new InvalidOperationException(
                    "Unable to activate a test context " +
                        "because a fixture context is active."
                );
            }

            if (this.HasTest)
            {
                throw new InvalidOperationException(
                    "Unable to activate a test context " +
                        "because another test context is active."
                );
            }

            return testResult;
        }

        StepResult ValidateNewStep(StepResult stepResult)
        {
            if (!this.HasTest && !this.HasFixture)
            {
                throw new InvalidOperationException(
                    "Unable to activate a step context because neither " +
                        "test, nor fixture context is active."
                );
            }

            return stepResult;
        }

        static string RepresentStack<T>(
            IImmutableStack<T> stack,
            Func<T, string> projection
        ) => string.Join(
            " <- ",
            stack.Select(projection)
        );
    }
}
