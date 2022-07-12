using System;
using System.Collections.Generic;
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
        internal static ITestResultAccessor TestResultAccessor
        {
            get => TestResultAccessorAsyncLocal.Value;
            set => TestResultAccessorAsyncLocal.Value = value;
        }
        
        #region Fixtures

        public static string StartBeforeFixture(string name)
        {
            var fixtureResult = new FixtureResult()
            {
                name = name,
                stage = Stage.running,
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            AllureLifecycle.Instance.StartBeforeFixture(TestResultAccessor.TestResultContainer.uuid, fixtureResult, out var uuid);
            Log($"Started Before: {name}");
            return uuid;
        }

        public static string StartAfterFixture(string name)
        {
            var fixtureResult = new FixtureResult()
            {
                name = name,
                stage = Stage.running,
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };

            // Afters.Add(fixtureResult);

            AllureLifecycle.Instance.StartAfterFixture(TestResultAccessor.TestResultContainer.uuid, fixtureResult, out var uuid);
            Log($"Started After: {name}");
            return uuid;
        }

        public static void StopFixture(Action<FixtureResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopFixture(result =>
            {
                result.stage = Stage.finished;
                result.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                updateResults?.Invoke(result);
            });
        }
        
        public static void StopFixtureSuppressTestCase(Action<FixtureResult> updateResults = null)
        {
            var newTestResult = TestResultAccessor.TestResult;
            StopFixture(updateResults);
            AllureLifecycle.Instance.StartTestCase(TestResultAccessor.TestResultContainer.uuid, newTestResult);
        }

        #endregion

        #region Steps

        public static string StartStep(string name, Action<StepResult> updateResults = null)
        {
            var stepResult = new StepResult
            {
                name = name,
                stage = Stage.running,
                start = DateTimeOffset.Now.ToUnixTimeMilliseconds()
            };
            updateResults?.Invoke(stepResult);

            AllureLifecycle.Instance.StartStep(stepResult, out var uuid);
            Log($"Started Step: {name}");
            return uuid;
        }

        public static void PassStep(Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopStep(result =>
            {
                result.status = Status.passed;
                updateResults?.Invoke(result);
            });
            Log("Passed");
        }

        public static void PassStep(string uuid, Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.UpdateStep(uuid, result =>
            {
                result.status = Status.passed;
                updateResults?.Invoke(result);
            });
            AllureLifecycle.Instance.StopStep(uuid);
            Log("Passed");
        }

        public static void FailStep(Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopStep(result =>
            {
                result.status = Status.failed;
                updateResults?.Invoke(result);
            });
            Log("Failed");
        }

        public static void FailStep(string uuid, Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.UpdateStep(uuid, result =>
            {
                result.status = Status.failed;
                updateResults?.Invoke(result);
            });
            AllureLifecycle.Instance.StopStep(uuid);
            Log("Failed");
        }
        
        public static void BrokeStep(Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopStep(result =>
            {
                result.status = Status.broken;
                updateResults?.Invoke(result);
            });
            Log("Broken");
        }
        
        public static void BrokeStep(string uuid, Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.UpdateStep(uuid, result =>
            {
                result.status = Status.broken;
                updateResults?.Invoke(result);
            });
            AllureLifecycle.Instance.StopStep(uuid);
            Log("Broken");
        }

        #endregion

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