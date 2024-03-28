using System;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using Allure.Net.Commons;
using AspectInjector.Broker;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using NUnit.Framework.Internal;

namespace Allure.NUnit.Internals
{
    [Aspect(Scope.Global)]
    public class StopContainerAspect
    {
        [Advice(Kind.Around)]
        public object StopTestContainerAfterTheLastOneTimeTearDown(
            [Argument(Source.Target)] Func<object[], object> target,
            [Argument(Source.Arguments)] object[] arguments,
            [Argument(Source.Metadata)] MethodBase metadata
        )
        {
            if (IsOneTimeTearDown(metadata))
            {
                CurrentTearDownCount++;
                if (IsLastTearDown)
                {
                    return CallAndStopContainer(target, arguments);
                }
            }
            return target(arguments);
        }

        static object CallAndStopContainer(
            Func<object[], object> target,
            object[] arguments
        )
        {
            object returnValue = null;
            try
            {
                returnValue = target(arguments);
            }
            finally
            {
                if (returnValue is null)
                {
                    StopContainer();
                }
                else
                {
                    // This branch is executed only in case of an async OneTimeTearDown
                    returnValue = StopContainerAfterAsyncTearDown(returnValue);
                }
            }
            return returnValue;
        }

        async static Task StopContainerAfterAsyncTearDown(object awaitable)
        {
            // Currently, we only support the Task return type for async tear
            // downs. ValueTask support should be added to commons first.
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

        static int TotalTearDownCount =>
            ((TestSuite)TestExecutionContext.CurrentContext.CurrentTest)
                .OneTimeTearDownMethods
                .Count(
                    m => Attribute.IsDefined(
                        m.MethodInfo,
                        typeof(AllureAfterAttribute)
                    )
                );

        const string CurrentTearDownKey = "CurrentTearDownCount";
    }
}
