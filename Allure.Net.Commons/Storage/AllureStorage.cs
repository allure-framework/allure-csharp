using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

#nullable enable

namespace Allure.Net.Commons.Storage
{
    internal class AllureStorage
    {
        readonly ConcurrentDictionary<string, object> storage = new();
        readonly AsyncLocal<AllureContext> context = new();

        public AllureContext CurrentContext
        {
            get => this.context.Value ??= new();
            set => this.context.Value = value;
        }

        public AllureStorage()
        {
            this.CurrentContext = new();
        }

        public TestResultContainer? CurrentTestContainerOrNull
        {
            get => this.CurrentContext.ContainerContext.FirstOrDefault();
        }

        public TestResultContainer CurrentTestContainer
        {
            get => this.CurrentContext.CurrentContainer;
        }

        public FixtureResult CurrentFixture
        {
            get => this.CurrentContext.CurrentFixture;
        }
        
        public TestResult CurrentTest
        {
            get => this.CurrentContext.CurrentTest;
        }
        
        public ExecutableItem CurrentStepContainer
        {
            get => this.CurrentContext.CurrentStepContainer;
        }
        
        public StepResult CurrentStep
        {
            get => this.CurrentContext.CurrentStep;
        }

        public T Get<T>(string uuid)
        {
            return (T) storage[uuid];
        }

        public T Put<T>(string uuid, T item) where T: notnull
        {
            return (T) storage.GetOrAdd(uuid, item);
        }

        public T Remove<T>(string uuid)
        {
            storage.TryRemove(uuid, out var value);
            return (T) value;
        }

        public void PutTestContainer(TestResultContainer container) =>
            this.PutAndUpdateContext(
                container.uuid,
                container,
                c => c.WithContainer(container)
            );

        public TestResultContainer RemoveTestContainer() =>
            this.RemoveAndUpdateContext<TestResultContainer>(
                this.CurrentTestContainer.uuid,
                c => c.WithNoLastContainer()
            );

        public TestResultContainer RemoveTestContainer(string uuid) =>
            this.RemoveAndUpdateContext<TestResultContainer>(
                uuid,
                c => ContextWithNoContainer(c, uuid)
            );

        public void PutFixture(FixtureResult fixture) =>
            this.UpdateContext(c => c.WithFixtureContext(fixture));

        public void PutFixture(string uuid, FixtureResult fixture) =>
            this.PutAndUpdateContext(
                uuid,
                fixture,
                c => c.WithFixtureContext(fixture)
            );

        public FixtureResult RemoveFixture()
        {
            var fixture = this.CurrentFixture;
            this.UpdateContext(c => c.WithNoFixtureContext());
            return fixture;
        }

        public FixtureResult RemoveFixture(string uuid)
            => this.RemoveAndUpdateContext<FixtureResult>(
                uuid,
                c => ReferenceEquals(
                    c.CurrentFixture,
                    this.Get<FixtureResult>(uuid)
                ) ? c.WithNoFixtureContext() : c
            );

        public void PutTestCase(TestResult testResult) =>
            this.PutAndUpdateContext(
                testResult.uuid,
                testResult,
                c => c.WithTestContext(testResult)
            );

        public TestResult RemoveTestCase() =>
            this.RemoveAndUpdateContext<TestResult>(
                this.CurrentTest.uuid,
                c => c.WithNoTestContext()
            );

        public TestResult RemoveTestCase(string uuid) =>
            this.RemoveAndUpdateContext<TestResult>(
                uuid,
                c => c.CurrentTest.uuid == uuid ? c.WithNoTestContext() : c
            );

        public void PutStep(StepResult stepResult) =>
            this.UpdateContext(
                c => c.WithStep(stepResult)
            );

        public void PutStep(string uuid, StepResult stepResult) =>
            this.PutAndUpdateContext(
                uuid,
                stepResult,
                c => c.WithStep(stepResult)
            );

        public StepResult RemoveStep()
        {
            var step = this.CurrentStep;
            this.UpdateContext(c => c.WithNoLastStep());
            return step;
        }

        public StepResult RemoveStep(string uuid) =>
            this.RemoveAndUpdateContext<StepResult>(
                uuid,
                c => this.ContextWithNoStep(c, uuid)
            );

        T PutAndUpdateContext<T>(
            string uuid,
            T value,
            Func<AllureContext, AllureContext> updateFn
        ) where T : notnull
        {
            var result = this.Put(uuid, value);
            this.UpdateContext(updateFn);
            return result;
        }

        T RemoveAndUpdateContext<T>(string uuid, Func<AllureContext, AllureContext> updateFn)
        {
            this.UpdateContext(updateFn);
            return this.Remove<T>(uuid);
        }

        void UpdateContext(Func<AllureContext, AllureContext> updateFn)
        {
            this.CurrentContext = updateFn(this.CurrentContext);
        }

        AllureContext ContextWithNoStep(AllureContext context, string  uuid)
        {
            var stepResult = this.Get<StepResult>(uuid);
            var stepsToPushAgain = new Stack<StepResult>();
            while (!ReferenceEquals(context.CurrentStep, stepResult))
            {
                stepsToPushAgain.Push(context.CurrentStep);
                context = context.WithNoLastStep();
                if (context.StepContext.IsEmpty)
                {
                    throw new InvalidOperationException(
                        $"Step {stepResult.name} is not in the current context"
                    );
                }
            }
            while (stepsToPushAgain.Any())
            {
                context = context.WithStep(
                    stepsToPushAgain.Pop()
                );
            }
            return context;
        }

        static AllureContext ContextWithNoContainer(
            AllureContext context,
            string uuid
        )
        {
            var containersToPushAgain = new Stack<TestResultContainer>();
            while (context.CurrentContainer.uuid != uuid)
            {
                containersToPushAgain.Push(context.CurrentContainer);
                context = context.WithNoLastContainer();
                if (context.ContainerContext.IsEmpty)
                {
                    throw new InvalidOperationException(
                        $"Container {uuid} is not in the current context"
                    );
                }
            }
            while (containersToPushAgain.Any())
            {
                context = context.WithContainer(
                    containersToPushAgain.Pop()
                );
            }
            return context;
        }
    }
}