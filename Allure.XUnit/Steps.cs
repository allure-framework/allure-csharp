using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons;
using Allure.XUnit;
using static Allure.Xunit.AllureXunitHelper;

namespace Allure.Xunit
{
    public static class Steps
    {
        private static readonly AsyncLocal<ITestResultAccessor> TestResultAccessorAsyncLocal = new();
        private static readonly AsyncLocal<List<ExecutableItem>> StepsHierarchy = new();

        private static IList<FixtureResult> Befores =>
            TestResultAccessor.TestResultContainer.befores ??= new List<FixtureResult>();

        private static IList<FixtureResult> Afters =>
            TestResultAccessor.TestResultContainer.afters ??= new List<FixtureResult>();

        internal static ITestResultAccessor TestResultAccessor
        {
            get => TestResultAccessorAsyncLocal.Value;
            set => TestResultAccessorAsyncLocal.Value = value;
        }

        public static ExecutableItem Current
        {
            get => StepsHierarchy.Value.LastOrDefault();
            internal set => StepsHierarchy.Value = new List<ExecutableItem> { value };
        }

        public static void StartBeforeFixture(string name)
        {
            var fixtureResult = new FixtureResult
            {
                name = name,
                stage = Stage.running,
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            Befores.Add(fixtureResult);
            StepsHierarchy.Value.Add(fixtureResult);
            Log($"Started Before: {name}");
        }

        public static void StartAfterFixture(string name)
        {
            var fixtureResult = new FixtureResult
            {
                name = name,
                stage = Stage.running,
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            Afters.Add(fixtureResult);
            StepsHierarchy.Value.Add(fixtureResult);
            Log($"Started After: {name}");
        }

        public static void StartStep(string name)
        {
            var stepResult = new StepResult
            {
                name = name,
                stage = Stage.running,
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
            var current = Current;
            current.steps ??= new List<StepResult>();
            current.steps.Add(stepResult);
            StepsHierarchy.Value.Add(stepResult);
            Log($"Started Step: {name}");
        }

        public static void PassStep()
        {
            PassStep(Current);
        }

        public static void PassStep(ExecutableItem step)
        {
            StepsHierarchy.Value.Remove(step);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            step.status = Status.passed;
            Log("Passed");
        }

        public static void FailStep()
        {
            FailStep(Current);
        }

        public static void FailStep(ExecutableItem step)
        {
            StepsHierarchy.Value.Remove(step);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            step.status = Status.failed;
            Log("Failed");
        }
        
        public static void BrokeStep()
        {
            BrokeStep(Current);
        }
        
        public static void BrokeStep(ExecutableItem step)
        {
            StepsHierarchy.Value.Remove(step);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            step.status = Status.broken;
            Log("Broken");
        }

        public static Task<T> Step<T>(string name, Func<Task<T>> action)
        {
            StartStep(name);
            return Execute(action);
        }

        public static T Step<T>(string name, Func<T> action)
        {
            StartStep(name);
            return Execute(name, action);
        }

        public static void Step(string name, Action action)
        {
            Step(name, (Func<object>) (() =>
            {
                action();
                return null;
            }));
        }

        public static Task Step(string name, Func<Task> action)
        {
            return Step(name, async () =>
            {
                await action();
                return Task.FromResult<object>(null);
            });
        }

        public static void Step(string name)
        {
            Step(name, () => { });
        }

        public static Task<T> Before<T>(string name, Func<Task<T>> action)
        {
            StartBeforeFixture(name);
            return Execute(action);
        }

        public static T Before<T>(string name, Func<T> action)
        {
            StartBeforeFixture(name);
            return Execute(name, action);
        }

        public static void Before(string name, Action action)
        {
            Before(name, (Func<object>) (() =>
            {
                action();
                return null;
            }));
        }

        public static Task Before(string name, Func<Task> action)
        {
            return Before(name, async () =>
            {
                await action();
                return Task.FromResult<object>(null);
            });
        }

        public static Task<T> After<T>(string name, Func<Task<T>> action)
        {
            StartAfterFixture(name);
            return Execute(action);
        }

        public static T After<T>(string name, Func<T> action)
        {
            StartAfterFixture(name);
            return Execute(name, action);
        }

        public static void After(string name, Action action)
        {
            After(name, (Func<object>) (() =>
            {
                action();
                return null;
            }));
        }

        public static Task After(string name, Func<Task> action)
        {
            return After(name, async () =>
            {
                await action();
                return Task.FromResult<object>(null);
            });
        }

        private static async Task<T> Execute<T>(Func<Task<T>> action)
        {
            T result;
            try
            {
                result = await action();
            }
            catch (Exception)
            {
                FailStep();
                throw;
            }

            PassStep();
            return result;
        }

        private static T Execute<T>(string name, Func<T> action)
        {
            T result;
            try
            {
                result = action();
            }
            catch (Exception e)
            {
                FailStep();
                throw new StepFailedException(name, e);
            }

            PassStep();
            return result;
        }
    }
}