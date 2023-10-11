using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Allure.Net.Commons;
using NUnit.Allure.Core;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestResult = Allure.Net.Commons.TestResult;

#nullable enable

namespace NUnit.Allure.Attributes
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
            AllureLifecycle.Instance.StartTestContainer(new()
            {
                uuid = suite.Id + "-ignored",
                name = suite.ClassName
            });
        }

        public void AfterTest(ITest suite)
        {
            suite = (TestSuite) suite;
            if (suite.HasChildren)
            {
                var ignoredTests =
                    GetAllTests(suite).Where(
                        t => t.RunState == RunState.Ignored
                            || t.RunState == RunState.Skipped
                    );
                foreach (var test in ignoredTests)
                {
                    this.EmitResultForIgnoredTest(test);
                }

                AllureLifecycle.Instance.StopTestContainer();
                AllureLifecycle.Instance.WriteTestContainer();
            }
        }

        public ActionTargets Targets => ActionTargets.Suite;

        private static IEnumerable<ITest> GetAllTests(ITest test)
        {
            return test.Tests.Concat(test.Tests.SelectMany(GetAllTests));
        }

        void EmitResultForIgnoredTest(ITest test)
        {
            AllureLifecycle.Instance.UpdateTestContainer(
                        t => t.children.Add(test.Id)
                    );

            var reason = test.Properties.Get(
                PropertyNames.SkipReason
            )?.ToString() ?? "";

            var ignoredTestResult = AllureNUnitHelper.CreateTestResult(test);
            ignoredTestResult.status = Status.skipped;
            ignoredTestResult.statusDetails = new() { message = test.Name };
            this.ApplyLegacySuiteLabels(ignoredTestResult, reason);
            AllureLifecycle.Instance.StartTestCase(ignoredTestResult);
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