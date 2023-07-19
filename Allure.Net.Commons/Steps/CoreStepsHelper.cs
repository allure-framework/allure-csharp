﻿using System;
using System.Threading;
using System.Threading.Tasks;
using Allure.Net.Commons.Storage;

namespace Allure.Net.Commons.Steps
{
    public class CoreStepsHelper
    {
        public static IStepLogger StepLogger { get; set; }

        private static readonly AsyncLocal<ITestResultAccessor> TestResultAccessorAsyncLocal = new();

        public static ITestResultAccessor TestResultAccessor
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

            AllureLifecycle.Instance.StartBeforeFixture(
                AllureLifecycle.Instance.Context.CurrentContainer.uuid,
                fixtureResult,
                out var uuid
            );
            StepLogger?.BeforeStarted?.Log(name);
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

            AllureLifecycle.Instance.StartAfterFixture(
                AllureLifecycle.Instance.Context.CurrentContainer.uuid,
                fixtureResult,
                out var uuid
            );
            StepLogger?.AfterStarted?.Log(name);
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
            var newTestResult = AllureLifecycle.Instance.Context.CurrentTest;
            StopFixture(updateResults);
            AllureLifecycle.Instance.StartTestCase(
                AllureLifecycle.Instance.Context.CurrentContainer.uuid,
                newTestResult
            );
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
            StepLogger?.StepStarted?.Log(name);
            return uuid;
        }

        public static void PassStep(Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopStep(result =>
            {
                result.status = Status.passed;
                updateResults?.Invoke(result);
                StepLogger?.StepPassed?.Log(result.name);
            });
        }

        public static void PassStep(string uuid, Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.UpdateStep(uuid, result =>
            {
                result.status = Status.passed;
                updateResults?.Invoke(result);
                StepLogger?.StepPassed?.Log(result.name);
            });
            AllureLifecycle.Instance.StopStep(uuid);
        }

        public static void FailStep(Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopStep(result =>
            {
                result.status = Status.failed;
                updateResults?.Invoke(result);
                StepLogger?.StepFailed?.Log(result.name);
            });
        }

        public static void FailStep(string uuid, Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.UpdateStep(uuid, result =>
            {
                result.status = Status.failed;
                updateResults?.Invoke(result);
                StepLogger?.StepFailed?.Log(result.name);
            });
            AllureLifecycle.Instance.StopStep(uuid);
        }
        
        public static void BrokeStep(Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.StopStep(result =>
            {
                result.status = Status.broken;
                updateResults?.Invoke(result);
                StepLogger?.StepBroken?.Log(result.name);
            });
        }
        
        public static void BrokeStep(string uuid, Action<StepResult> updateResults = null)
        {
            AllureLifecycle.Instance.UpdateStep(uuid, result =>
            {
                result.status = Status.broken;
                updateResults?.Invoke(result);
                StepLogger?.StepBroken?.Log(result.name);
            });
            AllureLifecycle.Instance.StopStep(uuid);
        }

        #endregion

        #region Misc

        public static void UpdateTestResult(Action<TestResult> update)
        {
            AllureLifecycle.Instance.UpdateTestCase(
                AllureLifecycle.Instance.Context.CurrentTest.uuid,
                update
            );
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