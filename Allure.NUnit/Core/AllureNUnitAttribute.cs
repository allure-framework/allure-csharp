using System;
using System.Threading;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace NUnit.Allure.Core
{
    [AttributeUsage(AttributeTargets.Interface | AttributeTargets.Class)]
    public class AllureNUnitAttribute : PropertyAttribute, ITestAction
    {
        private readonly ThreadLocal<AllureNUnitHelper> _allureNUnitHelper = new ThreadLocal<AllureNUnitHelper>(true);
        private readonly bool _isWrappedIntoStep;

        public AllureNUnitAttribute(bool wrapIntoStep = true)
        {
            _isWrappedIntoStep = wrapIntoStep;
        }

        public void BeforeTest(ITest test)
        {
            _allureNUnitHelper.Value = new AllureNUnitHelper(test);
            _allureNUnitHelper.Value.StartAll(_isWrappedIntoStep);
        }

        public void AfterTest(ITest test)
        {
            _allureNUnitHelper.Value.StopAll(_isWrappedIntoStep);
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}