using System;
using System.Collections.Concurrent;
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

        public void BeforeTest(ITest test)
        {
            var helper = new AllureNUnitHelper(test);
            _allureNUnitHelper.AddOrUpdate(
                test.Id,
                helper,
                (key, existing) => helper
            );

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

        public ActionTargets Targets =>
            ActionTargets.Test | ActionTargets.Suite;

        public void ApplyToContext(TestExecutionContext context)
        {
            var test = context.CurrentTest;
            var helper = new AllureNUnitHelper(test);
            helper.StartTestContainer();
            StepsHelper.StartBeforeFixture($"fr-{test.Id}");
        }
    }
}