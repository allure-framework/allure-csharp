using System;
using System.Reflection;
using System.Runtime.CompilerServices;
using Allure.Net.Commons;
using NUnit.Framework.Internal;
using System.Linq;
using System.Threading.Tasks;

namespace NUnit.Allure.Core
{
    internal class SetUpTearDownHelper
    {
        public string CustomName { get; set; }
        public string MethodName { get; set; }
        public long StartTime { get; set; }
        public long EndTime { get; set; }
        public Exception Exception { get; set; }

        public override string ToString()
        {
            return MethodName;
        }
    }

    public static class AllureExtensions
    {
        internal static long ToUnixTimeMilliseconds(this DateTimeOffset dateTimeOffset)
        {
            return (long) dateTimeOffset.UtcDateTime.Subtract(new DateTime(1970, 1, 1)).TotalMilliseconds;
        }

        /// <summary>
        /// Ability to measure time of execution of [SetUp], [TearDown], [OneTimeSetUp] NUnit attributes for Allure reports
        /// </summary>
        public static void WrapSetUpTearDownParams(Action action, string customName = "",
            [CallerMemberName] string callerName = "")
        {
            var setUpTearDownHelper = new SetUpTearDownHelper {MethodName = callerName};
            if (!string.IsNullOrEmpty(customName)) setUpTearDownHelper.CustomName = customName;
            try
            {
                setUpTearDownHelper.StartTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                action.Invoke();
            }
            catch (Exception e)
            {
                setUpTearDownHelper.Exception = e;
                throw;
            }
            finally
            {
                setUpTearDownHelper.EndTime = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                TestExecutionContext.CurrentContext.CurrentTest.Properties.Add(callerName, setUpTearDownHelper);
            }
        }

        /// <summary>
        /// Wraps Action into AllureStep.
        /// </summary>
        [Obsolete("Use [AllureStep] method attribute")]
        public static void WrapInStep(this AllureLifecycle lifecycle, Action action, string stepName = "", [CallerMemberName] string callerName = "")
        {
            if (string.IsNullOrEmpty(stepName)) stepName = callerName;

            var id = Guid.NewGuid().ToString();
            var stepResult = new StepResult {name = stepName};
            try
            {
                lifecycle.StartStep(id, stepResult);
                action.Invoke();
                lifecycle.StopStep(step => stepResult.status = Status.passed);
            }
            catch (Exception e)
            {
                lifecycle.StopStep(step =>
                {
                    step.statusDetails = new StatusDetails
                    {
                        message = e.Message,
                        trace = e.StackTrace
                    };
                    step.status = AllureNUnitHelper.GetNUnitStatus();
                });
                throw;
            }
        }

        /// <summary>
        /// Wraps Func into AllureStep.
        /// </summary>
        public static T WrapInStep<T>(this AllureLifecycle lifecycle, Func<T> func, string stepName = "", [CallerMemberName] string callerName = "")
        {
            if (string.IsNullOrEmpty(stepName)) stepName = callerName;
            var id = Guid.NewGuid().ToString();
            var stepResult = new StepResult {name = stepName};
            try
            {
                lifecycle.StartStep(id, stepResult);
                var result = func.Invoke();
                lifecycle.StopStep(step => stepResult.status = Status.passed);
                return result;
            }
            catch (Exception e)
            {
                lifecycle.StopStep(step =>
                {
                    step.statusDetails = new StatusDetails
                    {
                        message = e.Message,
                        trace = e.StackTrace
                    };
                    step.status = AllureNUnitHelper.GetNUnitStatus();
                });
                throw;
            }
        }

        /// <summary>
        /// Wraps async Action into AllureStep.
        /// </summary>
        public static async Task WrapInStepAsync(
            this AllureLifecycle lifecycle,
            Func<Task> action,
            string stepName = "",
            [CallerMemberName] string callerName = ""
        )
        {
            if (string.IsNullOrEmpty(stepName)) stepName = callerName;

            var id = Guid.NewGuid().ToString();
            var stepResult = new StepResult { name = stepName };
            try
            {
                lifecycle.StartStep(id, stepResult);
                await action();
                lifecycle.StopStep(step => stepResult.status = Status.passed);
            }
            catch (Exception e)
            {
                lifecycle.StopStep(step =>
                {
                    step.statusDetails = new StatusDetails
                    {
                        message = e.Message,
                        trace = e.StackTrace
                    };
                    step.status = AllureNUnitHelper.GetNUnitStatus();
                });
                throw;
            }
        }

        /// <summary>
        /// Wraps async Func into AllureStep.
        /// </summary>
        public static async Task<T> WrapInStepAsync<T>(
            this AllureLifecycle lifecycle,
            Func<Task<T>> func,
            string stepName = "",
            [CallerMemberName] string callerName = ""
        )
        {
            if (string.IsNullOrEmpty(stepName)) stepName = callerName;
            var id = Guid.NewGuid().ToString();
            var stepResult = new StepResult { name = stepName };
            try
            {
                lifecycle.StartStep(id, stepResult);
                var result = await func();
                lifecycle.StopStep(step => stepResult.status = Status.passed);
                return result;
            }
            catch (Exception e)
            {
                lifecycle.StopStep(step =>
                {
                    step.statusDetails = new StatusDetails
                    {
                        message = e.Message,
                        trace = e.StackTrace
                    };
                    step.status = AllureNUnitHelper.GetNUnitStatus();
                });
                throw;
            }
        }

        /// <summary>
        /// AllureNUnit AddScreenDiff wrapper method.
        /// </summary>
        public static void AddScreenDiff(this AllureLifecycle lifecycle, string expected, string actual, string diff)
        {
            var storageMain = lifecycle.GetType().GetField("storage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(lifecycle);
            var storageInternal = storageMain.GetType().GetField("storage", BindingFlags.NonPublic | BindingFlags.Instance).GetValue(storageMain);
            var keys = (storageInternal as System.Collections.Concurrent.ConcurrentDictionary<string, object>).Keys.ToList();
            AllureLifecycle.Instance.AddScreenDiff(keys.Find(key => key.Contains("-tr-")), expected, actual, diff);
        }
    }
}