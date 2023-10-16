using System;
using System.Collections.Concurrent;
using System.Linq;
using Allure.Net.Commons;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Allure.Core
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class AllureNUnitAttribute : PropertyAttribute, ITestAction, IApplyToContext
    {
        private readonly ConcurrentDictionary<string, AllureNUnitHelper> _allureNUnitHelper = new();

        [Obsolete("wrapIntoStep parameter is obsolete. Use [AllureStep] method attribute")]
        public AllureNUnitAttribute(bool wrapIntoStep = true)
        {
        }
        
        public AllureNUnitAttribute()
        {
        }

        public void BeforeTest(ITest test) =>
            RunHookInRestoredAllureContext(test, () =>
            {
                var helper = new AllureNUnitHelper(test);
                _allureNUnitHelper.AddOrUpdate(
                    test.Id,
                    helper,
                    (key, existing) => helper
                );

                if (!test.IsSuite)
                {
                    helper.PrepareTestContext();
                }
            });

        public void AfterTest(ITest test)
        {
            if (test.IsDeselected())
            {
                return;
            }

            RunHookInRestoredAllureContext(test, () =>
            {
                if (_allureNUnitHelper.TryGetValue(test.Id, out var helper))
                {
                    if (!test.IsSuite)
                    {
                        helper.StopTestCase();
                        helper.StopTestContainer();
                    }
                    else if (IsSuiteWithNoAfterFixtures(test))
                    {
                        // If a test class has no class-scope after-feature
                        // (i.e., a method with both [OneTimeTearDown] and
                        // [AllureAfter]), the class-scope container is closed
                        // here. Otherwise, it's closed in StopContainerAspect
                        // instead.
                        helper.StopTestContainer();
                    }
                }
            });
        }
        
        public ActionTargets Targets =>
            ActionTargets.Test | ActionTargets.Suite;

        public void ApplyToContext(TestExecutionContext context)
        {
            var test = context.CurrentTest;
            // A container for OneTimeSetUp/OneTimeTearDown methods.
            new AllureNUnitHelper(test).StartTestContainer();
            CaptureGlobalAllureContext(test);
        }

        static bool IsSuiteWithNoAfterFixtures(ITest test) =>
            test is TestSuite suite && !suite.OneTimeTearDownMethods.Any(
                m => IsDefined(m.MethodInfo, typeof(AllureAfterAttribute))
            );

        #region Allure context manipulation

        /*
         * The methods this region are to make sure the AllureContext
         * flows into setup/teardown/test methods correctly. This is needed
         * because NUnit might spread hooks of this class and user's code
         * across unrelated threads, hiding changes made to the allure context
         * in, say, BeforeTest from, say, a one-time tear down method.
         */

        static void RunHookInRestoredAllureContext(ITest test, Action action)
        {
            RestoreAssociatedAllureContext(test);
            try
            {
                action();
            }
            finally
            {
                CaptureGlobalAllureContext(test);
            }
        }

        static void CaptureGlobalAllureContext(ITest test) =>
            test.Properties.Set(ALLURE_CONTEXT_KEY, AllureLifecycle.Instance.Context);

        static void RestoreAssociatedAllureContext(ITest test) =>
            AllureLifecycle.Instance.RestoreContext(
                GetAssociatedAllureContext(test)
            );

        static AllureContext GetAssociatedAllureContext(ITest test) =>
            (AllureContext)test.Properties.Get(ALLURE_CONTEXT_KEY)
                ?? GetAssociatedAllureContext(test.Parent);

        const string ALLURE_CONTEXT_KEY = "AllureContext";

        #endregion
    }
}