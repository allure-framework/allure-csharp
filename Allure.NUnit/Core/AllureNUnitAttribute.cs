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

        public AllureNUnitAttribute(bool wrapIntoStep = true)
        {
        }

        public void BeforeTest(ITest test)
        {
            _allureNUnitHelper.Value = new AllureNUnitHelper(test);
            _allureNUnitHelper.Value.StartTestContainer();
            _allureNUnitHelper.Value.StartTestCase();
        }

        public void AfterTest(ITest test)
        {
            _allureNUnitHelper.Value.StopTestCase();
            _allureNUnitHelper.Value.StopTestContainer();
        }

        public ActionTargets Targets => ActionTargets.Test;
    }
}