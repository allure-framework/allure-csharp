using System;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Net.Commons;
using AspectInjector.Broker;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace NUnit.Allure.Internals
{
    [Aspect(Scope.Global)]
    public class StopContainerAspect
    {
        [Advice(Kind.Around)]
        public object StopTestContainerAfterTheLastOneTimeTearDown(
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Metadata)] MethodBase metadata
        )
        {
            if (IsOneTimeTearDown(metadata))
            {
                CurrentTearDownCount++;
                if (IsLastTearDown)
                {
                    return CallAndStopContainer(target);
                }
            }
            return target(Array.Empty<object>());
        }

        static object CallAndStopContainer(Func<object[], object> target)
        {
            object returnValue = null;
            try
            {
                returnValue = target(Array.Empty<object>());
            }
            finally
            {
                if (returnValue is null)
                {
                    StopContainer();
                }
                else
                {
                    // This branch is executed only in case of an async one time tear down
                    returnValue = StopContainerAfterAsyncTearDown(returnValue);
                }
            }
            return returnValue;
        }

        async static Task StopContainerAfterAsyncTearDown(object awaitable)
        {
            await ((Task)awaitable).ConfigureAwait(false);
            StopContainer();
        }

        static void StopContainer()
        {
            AllureLifecycle.Instance.StopTestContainer();
            AllureLifecycle.Instance.WriteTestContainer();
        }

        static bool IsOneTimeTearDown(MethodBase metadata) =>
            Attribute.IsDefined(
                metadata,
                typeof(OneTimeTearDownAttribute)
            );

        static bool IsLastTearDown
        {
            get => CurrentTearDownCount == TotalTearDownCount;
        }

        static int CurrentTearDownCount
        {
            get => (int?) TestExecutionContext.CurrentContext
                .CurrentTest.Properties.Get(CurrentTearDownKey) ?? 0;
            set => TestExecutionContext.CurrentContext
                .CurrentTest.Properties.Set(
                    CurrentTearDownKey,
                    value
                );
        }

        static int TotalTearDownCount
        {
            get => (
                (TestSuite)TestExecutionContext.CurrentContext.CurrentTest
            ).OneTimeTearDownMethods.Length;
        }

        const string CurrentTearDownKey = "CurrentTearDownCount";
    }
}
