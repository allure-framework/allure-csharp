using System;
using System.Collections.Concurrent;
using Allure.Net.Commons;
using NUnit.Engine;
using NUnit.Engine.Extensibility;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace NUnit.Allure.Core
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class AllureNUnitAttribute : PropertyAttribute, ITestAction, IApplyToContext
    {
        private readonly ConcurrentDictionary<string, AllureNUnitHelper> _allureNUnitHelper = new ConcurrentDictionary<string, AllureNUnitHelper>();
        private readonly bool _isWrappedIntoStep;

        static AllureNUnitAttribute()
        {
            //!_! This is essential for async tests.
            //!_! Async tests are working on different threads, so
            //!_! default ManagedThreadId-separated behaviour in some cases fails on cross-thread execution.
            AllureLifecycle.CurrentTestIdGetter = () => TestContext.CurrentContext.Test.FullName;
        }

        [Obsolete("wrapIntoStep parameter is obsolete. Use [AllureStep] method attribute")]
        public AllureNUnitAttribute(bool wrapIntoStep = true)
        {
            _isWrappedIntoStep = wrapIntoStep;
        }
        
        public AllureNUnitAttribute()
        {
        }

        public void BeforeTest(ITest test)
        {
            var helper = new AllureNUnitHelper(test);
            _allureNUnitHelper.AddOrUpdate(test.Id, helper, (key, existing) => helper);

            if (test.IsSuite)
            {
                helper.SaveOneTimeResultToContext();
                StepsHelper.StopFixture();
            }
            else
            {
                helper.StartTestContainer();
                helper.AddOneTimeSetupResult();
                helper.StartTestCase();
            }
        }

        public void AfterTest(ITest test)
        {
            if (_allureNUnitHelper.TryGetValue(test.Id, out var helper))
            {
                if (!test.IsSuite)
                {
                    helper.StopTestCase();
                }

                helper.StopTestContainer();
            }
        }

        public ActionTargets Targets => ActionTargets.Test | ActionTargets.Suite;

        public void ApplyToContext(TestExecutionContext context)
        {
            var test = context.CurrentTest;
            var helper = new AllureNUnitHelper(test);
            helper.StartTestContainer();
            StepsHelper.StartBeforeFixture($"fr-{test.Id}");
        }
    }
}