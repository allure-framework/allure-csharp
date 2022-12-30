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

        public AllureNUnitAttribute(bool wrapIntoStep = true)
        {
            _isWrappedIntoStep = wrapIntoStep;
        }

        public void BeforeTest(ITest test)
        {
            var value = new AllureNUnitHelper(test);

            _allureNUnitHelper.AddOrUpdate(test.Id, value, (key, existing) => value);

            value.StartAll(_isWrappedIntoStep);
        }

        public void AfterTest(ITest test)
        {
            if(_allureNUnitHelper.TryGetValue(test.Id, out var value))
            {
                value.StopAll(_isWrappedIntoStep);
            }
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}