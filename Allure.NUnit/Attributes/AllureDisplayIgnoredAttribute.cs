using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Allure.Net.Commons;
using Allure.NUnit.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestResult = Allure.Net.Commons.TestResult;

#nullable enable

namespace Allure.NUnit.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AllureDisplayIgnoredAttribute : NUnitAttribute, ITestAction
    {
        private readonly string? _suiteName = null;

        public AllureDisplayIgnoredAttribute(){}

        [Obsolete("Allure attributes are now supported for ignored tests. Use them instead")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public AllureDisplayIgnoredAttribute(string suiteNameForIgnoredTests = "Ignored")
        {
            _suiteName = suiteNameForIgnoredTests;
        }

        public void BeforeTest(ITest suite)
        {
        }

        public void AfterTest(ITest suite)
        {
            suite = (TestSuite) suite;
            if (suite.HasChildren)
            {
                var ignoredTests = GetIgnoredTests(suite);
                foreach (var test in ignoredTests)
                {
                    this.EmitResultForIgnoredTestInTestPlan(test);
                }
            }
        }

        public ActionTargets Targets => ActionTargets.Suite;

        static IEnumerable<ITest> GetIgnoredTests(ITest suite, bool parentIgnored = false)
        {
            return suite.Tests.SelectMany(test =>
            {
                bool isIgnored = parentIgnored || IsTestIgnored(test);
                return test.IsSuite
                    ? GetIgnoredTests(test, isIgnored)
                    : isIgnored
                        ? Enumerable.Repeat(test, 1)
                        : Enumerable.Empty<ITest>();
            });
        }

        static bool IsTestIgnored(ITest test) => test.RunState switch
        {
            RunState.Ignored or RunState.Skipped => true,
            _ => false
        };

        void EmitResultForIgnoredTestInTestPlan(ITest test)
        {
            var ignoredTestResult = AllureNUnitHelper.CreateTestResult(test);
            if (AllureNUnitHelper.IsSelectedByTestPlan(ignoredTestResult))
            {
                this.EmitTestResult(test, ignoredTestResult);
            }
        }

        void EmitTestResult(ITest test, TestResult testResult)
        {
            var reason = test.Properties.Get(
                PropertyNames.SkipReason
            )?.ToString() ?? "";
            testResult.status = Status.skipped;
            testResult.statusDetails = new() { message = test.Name };
            this.ApplyLegacySuiteLabels(testResult, reason);

            AllureLifecycle.Instance.StartTestCase(testResult);
            AllureLifecycle.Instance.StopTestCase();
            AllureLifecycle.Instance.WriteTestCase();
        }

        void ApplyLegacySuiteLabels(TestResult testResult, string reason)
        {
            if (!string.IsNullOrWhiteSpace(this._suiteName))
            {
                testResult.labels.AddRange(new[]
                {
                    Label.Suite(this._suiteName),
                    Label.SubSuite(reason)
                });
            }
        }
    }
}