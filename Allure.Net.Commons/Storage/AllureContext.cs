using System;
using System.Collections.Immutable;
using System.Linq;

#nullable enable

namespace Allure.Net.Commons.Storage
{
    /// <summary>
    /// Represents information related to a particular test at all stages of
    /// its life cycle.
    /// </summary>
    /// 
    /// <remarks>
    /// Instances of this class are immutable to ensure proper isolation
    /// between different tests and steps that may potentially be run
    /// cuncurrently either by a test framework or by an end user.<br/>
    /// 
    /// Methods in this class don't mutate allure model.
    /// </remarks>
    public record class AllureContext
    {
        /// <summary>
        /// A stack of fixture containers affecting subsequent tests.
        /// </summary>
        /// <remarks>
        /// Setting up this context allows operations on the current container
        /// (including adding a fixture to or removing a fixture from the
        /// current container).
        /// </remarks>
        public IImmutableStack<TestResultContainer> ContainerContext
        {
            get;
            private init;
        } = ImmutableStack<TestResultContainer>.Empty;

        /// <summary>
        /// A fixture that is being currently executed.
        /// </summary>
        /// <remarks>
        /// Setting up this context allows operations on the current fixture
        /// result.<br/>
        /// This property differs from <see cref="CurrentFixture"/> in that it
        /// returns null if no fixture context exists instead of throwing.
        /// </remarks>
        public FixtureResult? FixtureContext { get; private init; }

        /// <summary>
        /// A test that is being executed.
        /// </summary>
        /// <remarks>
        /// Setting up this context allows operations on the current test
        /// result.<br/>
        /// 
        /// This property differs from <see cref="CurrentTest"/> in that it
        /// returns null if no test context exists instead of throwing.
        /// </remarks>
        public TestResult? TestContext { get; private init; }

        /// <summary>
        /// A stack of nested steps that are being executed.
        /// </summary>
        /// <remarks>
        /// Setting up this context allows operations on the current step.
        /// </remarks>
        public IImmutableStack<StepResult> StepContext
        {
            get;
            private init;
        } = ImmutableStack<StepResult>.Empty;

        /// <summary>
        /// The most recently added container from the container context.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if there is no
        /// container context.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public TestResultContainer CurrentContainer
        {
            get => this.ContainerContext.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "No container context has been set up."
                );
        }

        /// <summary>
        /// A fixture that is being executed.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if there is no
        /// fixture context.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public FixtureResult CurrentFixture
        {
            get => this.FixtureContext ?? throw new InvalidOperationException(
                "No fixture context has been set up."
            );
        }

        /// <summary>
        /// A test that is being executed.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if there is no
        /// test context.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public TestResult CurrentTest
        {
            get => this.TestContext ?? throw new InvalidOperationException(
                "No test context has been set up."
            );
        }

        /// <summary>
        /// A step that is being executed.
        /// </summary>
        /// <remarks>
        /// It throws <see cref="InvalidOperationException"/> if there is no
        /// step context.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public StepResult CurrentStep
        {
            get => this.StepContext.FirstOrDefault()
                ?? throw new InvalidOperationException(
                    "No step context has been set up."
                );
        }

        /// <summary>
        /// A step container a next step should be put in.
        /// </summary>
        /// <remarks>
        /// A step container can be a fixture, a test of an another step.<br/>
        /// It throws <see cref="InvalidOperationException"/> if neither
        /// fixture nor test nor step context has been set up.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public ExecutableItem CurrentStepContainer
        {
            get => this.StepContext.FirstOrDefault() as ExecutableItem
                ?? this.RootStepContainer
                ?? throw new InvalidOperationException(
                    "No fixture, test, or step context has been set up."
                );
        }

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the specified
        /// container pushed into the container context.
        /// </summary>
        /// <param name="container">
        /// A container to push into the container context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified 
        /// container context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        public AllureContext WithContainer(TestResultContainer container) =>
            this with
            {
                ContainerContext = this.ContainerContext.Push(
                    container ?? throw new ArgumentNullException(
                        nameof(container)
                    )
                )
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> without the most recently
        /// added container in its container context. Requires a nonempty
        /// container context.
        /// </summary>
        /// <remarks>
        /// Can't be called if a fixture or a test context exists.
        /// </remarks>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// container context.
        /// </returns>
        /// <exception cref="InvalidOperationException"/>
        public AllureContext WithNoLastContainer() =>
            this with
            {
                ContainerContext = this.ValidateContainerCanBeRemoved()
                    .ContainerContext.Pop()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the specified
        /// fixture result as the fixture context. Requires at least one
        /// container in the container context.
        /// </summary>
        /// <param name="fixtureResult">
        /// A new fixture context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// fixture context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public AllureContext WithFixtureContext(FixtureResult fixtureResult) =>
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
        /// Creates a new <see cref="AllureContext"/> with no fixture and step
        /// contexts.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> without a fixture or
        /// a step contexts.
        /// </returns>
        public AllureContext WithNoFixtureContext() =>
            this with
            {
                FixtureContext = null,
                StepContext = this.StepContext.Clear()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the specified
        /// test result as the test context. Can't be used if a fixture context
        /// exists.
        /// </summary>
        /// <param name="testResult">
        /// A new test context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// test context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public AllureContext WithTestContext(TestResult testResult) =>
            this with
            {
                TestContext = this.ValidateNewTestContext(
                    testResult ?? throw new ArgumentNullException(
                        nameof(testResult)
                    )
                )
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with no test, fixture
        /// and step contexts.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> without a test,
        /// a fixture and a step contexts.
        /// </returns>
        public AllureContext WithNoTestContext() =>
            this with
            {
                FixtureContext = null,
                TestContext = null,
                StepContext = this.StepContext.Clear()
            };

        /// <summary>
        /// Creates a new <see cref="AllureContext"/> with the specified
        /// step result pushed into the step context. Requires either a test or
        /// a fixture context to exists.
        /// </summary>
        /// <param name="stepResult">
        /// A new step result to push into the step context.
        /// </param>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// step context.
        /// </returns>
        /// <exception cref="ArgumentNullException"/>
        /// <exception cref="InvalidOperationException"/>
        public AllureContext WithStep(StepResult stepResult) =>
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
        /// added step in its step context. Requires a nonempty step context.
        /// </summary>
        /// <returns>
        /// A new instance of <see cref="AllureContext"/> with the modified
        /// step context.
        /// </returns>
        /// <exception cref="InvalidOperationException"/>
        public AllureContext WithNoLastStep() =>
            this with
            {
                StepContext = this.StepContext.IsEmpty
                    ? throw new InvalidOperationException(
                        "Unable to exclude the latest step from the context " +
                            "because no step context has been set up."
                    ) : this.StepContext.Pop()
            };

        AllureContext ValidateContainerCanBeRemoved()
        {
            if (this.ContainerContext.IsEmpty)
            {
                throw new InvalidOperationException(
                    "Unable to exclude the latest container from the " +
                        "context because no container context has been set up."
                );
            }

            if (this.FixtureContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to exclude the latest container from the " +
                        "context because a fixture context exists."
                );
            }

            if (this.TestContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to exclude the latest container from the " +
                        "context because a test context exists."
                );
            }

            return this;
        }

        ExecutableItem? RootStepContainer
        {
            get => this.FixtureContext as ExecutableItem ?? this.TestContext;
        }

        FixtureResult ValidateNewFixtureContext(FixtureResult fixture)
        {
            if (this.ContainerContext.IsEmpty)
            {
                throw new InvalidOperationException(
                    "Unable to set up the fixture context " +
                        "because there is no container context."
                );
            }

            if (this.FixtureContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to set up the fixture context " +
                        "because another fixture context already exists."
                );
            }

            return fixture;
        }

        TestResult ValidateNewTestContext(TestResult testResult)
        {
            if (this.FixtureContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to set up the test context " +
                        "because a fixture context is currently active."
                );
            }

            if (this.TestContext is not null)
            {
                throw new InvalidOperationException(
                    "Unable to set up the test context " +
                        "because another test context already exists."
                );
            }

            return testResult;
        }

        StepResult ValidateNewStep(StepResult stepResult)
        {
            if (this.RootStepContainer is null)
            {
                throw new InvalidOperationException(
                    "Unable to set up the step context because no test or" +
                        "fixture context exists."
                );
            }

            return stepResult;
        }
    }
}
