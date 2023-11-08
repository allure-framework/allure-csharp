using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Newtonsoft.Json.Linq;
using NUnit.Allure.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

using TestResult = Allure.Net.Commons.TestResult;

// ReSharper disable AccessToModifiedClosure

namespace NUnit.Allure.Core
{
    public sealed class AllureNUnitHelper
    {
        internal static List<Type> ExceptionTypes = new()
        {
            typeof(NUnitException),
            typeof(AssertionException)
        };

        private readonly ITest _test;

        public AllureNUnitHelper(ITest test)
        {
            _test = test;
        }

        private static AllureLifecycle AllureLifecycle => AllureLifecycle.Instance;

        internal void StartTestContainer()
        {
            AllureLifecycle.StartTestContainer(
                this.CreateTestContainer()
            );
        }

        internal void PrepareTestContext()
        {
            var testResult = CreateTestResult(this._test);
            if (IsSelectedByTestPlan(testResult))
            {
                this.StartTestContainer(); // A container for SetUp/TearDown methods
                AllureLifecycle.StartTestCase(testResult);
            }
            else
            {
                this._test.Deselect();
                Assert.Ignore("Deselected by the testplan.");
            }
        }

        internal void StopTestCase()
        {
            UpdateTestDataFromNUnitProperties();
            AddConsoleOutputAttachment();

            AllureLifecycle.UpdateTestCase(
                x => x.statusDetails = new StatusDetails
                {
                    message = string.IsNullOrWhiteSpace(
                        TestContext.CurrentContext.Result.Message
                    ) ? TestContext.CurrentContext.Test.Name
                        : TestContext.CurrentContext.Result.Message,
                    trace = TestContext.CurrentContext.Result.StackTrace
                }
            );

            AllureLifecycle.StopTestCase(
                testCase => testCase.status = GetNUnitStatus()
            );
            AllureLifecycle.WriteTestCase();
        }

        internal void StopTestContainer()
        {
            AllureLifecycle.StopTestContainer();
            AllureLifecycle.WriteTestContainer();
        }

        internal static TestResult CreateTestResult(ITest test)
        {
            var testResult = new TestResult
            {
                name = test.Name,
                labels = new List<Label>
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Language(),
                    Label.Framework("NUnit 3"),
                    Label.Package(
                        GetNamespace(test.ClassName)
                    ),
                    Label.TestMethod(test.MethodName),
                    Label.TestClass(
                        GetClassName(test.ClassName)
                    )
                }
            };
            UpdateTestDataFromAllureAttributes(test, testResult);
            AddTestParametersFromNUnit(test, testResult);
            SetIdentifiers(test, testResult);
            return testResult;
        }

        public static Status GetNUnitStatus()
        {
            var result = TestContext.CurrentContext.Result;

            if (result.Outcome.Status != TestStatus.Passed)
            {
                var jo = JObject.Parse(AllureLifecycle.JsonConfiguration);
                var allureSection = jo["allure"];
                try
                {
                    var config = allureSection
                        ?.ToObject<AllureExtendedConfiguration>();
                    if (config?.BrokenTestData != null)
                    {
                        foreach (var word in config.BrokenTestData)
                        {
                            if (result.Message.Contains(word))
                            {
                                return Status.broken;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //Ignored
                }

                switch (result.Outcome.Status)
                {
                    case TestStatus.Inconclusive:
                        return Status.broken;
                    case TestStatus.Skipped:
                        return Status.skipped;
                    case TestStatus.Passed:
                        return Status.passed;
                    case TestStatus.Warning:
                        return Status.broken;
                    case TestStatus.Failed:
                        return Status.failed;
                    default:
                        return Status.none;
                }
            }

            return Status.passed;
        }

        TestResultContainer CreateTestContainer() =>
            new()
            {
                name = this._test.FullName,
                uuid = AllureLifecycle.AllureConfiguration.UseLegacyIds
                    ? this.ContainerId
                    : IdFunctions.CreateUUID()
            };

        static void SetIdentifiers(ITest test, TestResult testResult)
        {
            if (AllureLifecycle.AllureConfiguration.UseLegacyIds)
            {
                SetLegacyIdentifiers(test, testResult);
                return;
            }

            testResult.uuid = IdFunctions.CreateUUID();
            testResult.fullName = IdFunctions.CreateFullName(
                test.Method.MethodInfo
            );
            testResult.testCaseId = IdFunctions.CreateTestCaseId(
                testResult.fullName
            );
        }

        static void SetLegacyIdentifiers(ITest test, TestResult testResult)
        {
            testResult.uuid = string.Concat(
                Guid.NewGuid().ToString(),
                "-tr-",
                test.Id
            );
            testResult.fullName = test.FullName;
            testResult.historyId = test.FullName;
        }

        static void AddTestParametersFromNUnit(ITest test, TestResult testResult)
        {
            var arguments = CollectNUnitArguments(test);
            var formatters = AllureLifecycle.TypeFormatters;
            foreach (var (name, value) in arguments)
            {
                testResult.parameters.Add(new()
                {
                    name = name,
                    value = FormatFunctions.Format(value, formatters)
                });
            }
        }

        static IEnumerable<(string, object)> CollectNUnitArguments(ITest test) =>
            test.Method.MethodInfo.GetParameters()
                .Select(p => p.Name)
                .Zip(
                    test.Arguments,
                    (n, v) => (n, v)
                );

        static void UpdateTestDataFromAllureAttributes(ITest test, TestResult testResult)
        {
            foreach (var attribute in IterateAllAllureAttribites(test))
            {
                attribute.UpdateTestResult(testResult);
            }
        }

        static bool IsSelectedByTestPlan(TestResult testResult) =>
            AllureLifecycle.TestPlan.IsSelected(testResult);

        static IEnumerable<AllureTestCaseAttribute> IterateAllAllureAttribites(ITest test) =>
            test.Method
                .GetCustomAttributes<AllureTestCaseAttribute>(true)
                .Concat(
                    GetTestFixture(test)
                        .GetCustomAttributes<AllureTestCaseAttribute>(true)
                );

        static string GetNamespace(string classFullName)
        {
            var lastDotIndex = classFullName?.LastIndexOf('.') ?? -1;
            return lastDotIndex == -1 ? null : classFullName.Substring(
                0,
                lastDotIndex
            );
        }

        static string GetClassName(string classFullName)
        {
            var lastDotIndex = classFullName?.LastIndexOf('.') ?? -1;
            return lastDotIndex == -1 ? classFullName : classFullName.Substring(
                lastDotIndex + 1
            );
        }

        static TestFixture GetTestFixture(ITest test)
        {
            var currentTest = test;
            var isTestSuite = currentTest.IsSuite;
            while (isTestSuite != true)
            {
                currentTest = currentTest.Parent;
                if (currentTest is ParameterizedMethodSuite)
                {
                    currentTest = currentTest.Parent;
                }
                isTestSuite = currentTest.IsSuite;
            }

            return (TestFixture) currentTest;
        }

        private void UpdateTestDataFromNUnitProperties()
        {
            foreach (var p in GetTestProperties(PropertyNames.Description))
            {
                AllureLifecycle.UpdateTestCase(
                    x => x.description += $"{p}\n"
                );
            }

            foreach (var p in GetTestProperties(PropertyNames.Author))
            {
                AllureLifecycle.UpdateTestCase(
                    x => x.labels.Add(Label.Owner(p))
                );
            }

            foreach (var p in GetTestProperties(PropertyNames.Category))
            {
                AllureLifecycle.UpdateTestCase(
                    x => x.labels.Add(Label.Tag(p))
                );
            }
        }

        private void AddConsoleOutputAttachment()
        {
            var output = TestExecutionContext
                .CurrentContext
                .CurrentResult
                .Output;
            AllureApi.AddAttachment(
                "Console Output",
                "text/plain", 
                Encoding.UTF8.GetBytes(output),
                ".txt"
            );
        }

        private IEnumerable<string> GetTestProperties(string name)
        {
            var list = new List<string>();
            var currentTest = _test;
            while (currentTest.GetType() != typeof(TestSuite)
                && currentTest.GetType() != typeof(TestAssembly))
            {
                if (currentTest.Properties.ContainsKey(name))
                {
                    if (currentTest.Properties[name].Count > 0)
                    {
                        for (var i = 0; i < currentTest.Properties[name].Count; i++)
                        {
                            list.Add(
                                currentTest.Properties[name][i].ToString()
                            );
                        }
                    }
                }

                currentTest = currentTest.Parent;
            }

            return list;
        }

        [Obsolete("Use extension method AllureLifecycle.WrapInStep")]
        public void WrapInStep(Action action, string stepName = "")
        {
            AllureLifecycle.WrapInStep(action, stepName);
        }

        private string ContainerId => $"tc-{_test.Id}";

        [Obsolete("Not intended as a part of the public API")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void SaveOneTimeResultToContext()
        {
            var currentResult = TestExecutionContext
                .CurrentContext
                .CurrentResult;

            if (!string.IsNullOrEmpty(currentResult.Output))
            {
                AllureLifecycle.Instance.AddAttachment(
                    "Console Output",
                    "text/plain",
                    Encoding.UTF8.GetBytes(currentResult.Output),
                    ".txt"
                );
            }

            FixtureResult fixtureResult = null;
            AllureLifecycle.Instance.UpdateFixture(fr =>
            {
                fr.name = "OneTimeSetUp";
                fr.status = fr.steps.SelectMany(s => s.steps)
                    .All(s => s.status == Status.passed)
                    ? Status.passed
                    : Status.failed;

                fixtureResult = fr;
            });
            
            var testFixture = GetTestFixture(
                TestExecutionContext.CurrentContext.CurrentTest
            );
            testFixture.Properties.Set("OneTimeSetUpResult", fixtureResult);
        }

        [Obsolete("Not intended as a part of the public API")]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void AddOneTimeSetupResult()
        {
            var testFixture = GetTestFixture(
                TestExecutionContext.CurrentContext.CurrentTest
            );
            FixtureResult fixtureResult = null;

            fixtureResult = testFixture.Properties.Get(
                "OneTimeSetUpResult"
            ) as FixtureResult;

            if (fixtureResult != null && fixtureResult.steps.Any())
            {
                AllureLifecycle.UpdateTestContainer(container =>
                {
                    container.befores.Add(fixtureResult);
                });
            }
        }
    }
}
