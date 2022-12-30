using System;
using System.Collections.Concurrent;
using Allure.Net.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NUnit.Allure.Core
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class AllureNUnitAttribute : PropertyAttribute, ITestAction
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
            helper.StartTestContainer();
            helper.StartTestCase();
        }

        public void AfterTest(ITest test)
        {
            if (_allureNUnitHelper.TryGetValue(test.Id, out var helper))
            {
                helper.StopTestCase();
                helper.StopTestContainer();
            }
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}