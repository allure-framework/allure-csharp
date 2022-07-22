using System;
using System.Collections.Generic;
using System.Linq;
using Allure.Net.Commons;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using TestResult = Allure.Net.Commons.TestResult;

namespace NUnit.Allure.Attributes
{
    [AttributeUsage(AttributeTargets.Class)]
    public class AllureDisplayIgnoredAttribute : NUnitAttribute, ITestAction
    {
        private readonly string _suiteName;
        private string _ignoredContainerId;

        public AllureDisplayIgnoredAttribute(string suiteNameForIgnoredTests = "Ignored")
        {
            _suiteName = suiteNameForIgnoredTests;
        }

        public void BeforeTest(ITest suite)
        {
            _ignoredContainerId = suite.Id + "-ignored";
            var fixture = new TestResultContainer
            {
                uuid = _ignoredContainerId,
                name = suite.ClassName
            };
            AllureLifecycle.Instance.StartTestContainer(fixture);
        }

        public void AfterTest(ITest suite)
        {
            suite = (TestSuite) suite;
            if (suite.HasChildren)
            {
                var ignoredTests =
                    GetAllTests(suite).Where(t => t.RunState == RunState.Ignored || t.RunState == RunState.Skipped);
                foreach (var test in ignoredTests)
                {
                    AllureLifecycle.Instance.UpdateTestContainer(_ignoredContainerId, t => t.children.Add(test.Id));

                    var reason = test.Properties.Get(PropertyNames.SkipReason).ToString();

                    var ignoredTestResult = new TestResult
                    {
                        uuid = test.Id,
                        name = test.Name,
                        fullName = test.FullName,
                        status = Status.skipped,
                        statusDetails = new StatusDetails
                        {
                            message = test.Name
                        },
                        labels = new List<Label>
                        {
                            Label.Suite(_suiteName),
                            Label.SubSuite(reason),
                            Label.Thread(),
                            Label.Host(),
                            Label.TestClass(test.ClassName),
                            Label.TestMethod(test.MethodName),
                            Label.Package(test.ClassName)
                        }
                    };
                    AllureLifecycle.Instance.StartTestCase(ignoredTestResult);
                    AllureLifecycle.Instance.StopTestCase(ignoredTestResult.uuid);
                    AllureLifecycle.Instance.WriteTestCase(ignoredTestResult.uuid);
                }

                AllureLifecycle.Instance.StopTestContainer(_ignoredContainerId);
                AllureLifecycle.Instance.WriteTestContainer(_ignoredContainerId);
            }
        }

        public ActionTargets Targets => ActionTargets.Suite;

        private static IEnumerable<ITest> GetAllTests(ITest test)
        {
            return test.Tests.Concat(test.Tests.SelectMany(GetAllTests));
        }
    }
}