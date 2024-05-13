using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using Allure.Net.Commons;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.TestPlan;
using Allure.NUnit.Attributes;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

using TestResult = Allure.Net.Commons.TestResult;

// ReSharper disable AccessToModifiedClosure

namespace Allure.NUnit.Core
{
    sealed class AllureNUnitHelper
    {
        private readonly ITest _test;

        internal AllureNUnitHelper(ITest test)
        {
            _test = test;
            AllureLifecycle.AllureConfiguration.FailExceptions ??= new()
            {
                typeof(NUnitException).FullName,
                typeof(AssertionException).FullName,
                typeof(MultipleAssertException).FullName
            };
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
                Assert.Ignore(AllureTestPlan.SkipReason);
            }
        }

        internal void StopTestCase()
        {
            UpdateTestDataFromNUnitProperties();
            AddConsoleOutputAttachment();

            var result = TestContext.CurrentContext.Result;
            var nunitStatus = result.Outcome.Status;
            var status = GetNUnitStatus();
            var message = result.Message;
            var hasMessage = !string.IsNullOrWhiteSpace(message);
            var trace = result.StackTrace;
            var statusDetails = hasMessage || !string.IsNullOrWhiteSpace(trace)
                ? new StatusDetails
                {
                    message = hasMessage ? message : $"Test {nunitStatus}",
                    trace = trace
                }
                : null;


            AllureLifecycle.StopTestCase(
                testCase =>
                {
                    testCase.status = status;
                    testCase.statusDetails = statusDetails;
                }
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

        internal static bool IsSelectedByTestPlan(TestResult testResult) =>
            AllureLifecycle.TestPlan.IsSelected(testResult);

        internal static Status GetNUnitStatus()
        {
            var result = TestContext.CurrentContext.Result;
            return result.Outcome.Status switch
            {
                TestStatus.Inconclusive or TestStatus.Warning =>
                    Status.broken,
                TestStatus.Skipped => Status.skipped,
                TestStatus.Passed => Status.passed,
                TestStatus.Failed when IsBroken(result) => Status.broken,
                TestStatus.Failed => Status.failed,
                _ => Status.none
            };
        }

        TestResultContainer CreateTestContainer() =>
            new()
            {
                name = this._test.FullName,
                uuid = AllureLifecycle.AllureConfiguration.UseLegacyIds
                    ? this.ContainerId
                    : IdFunctions.CreateUUID()
            };

        static bool IsBroken(TestContext.ResultAdapter result) =>
            !result.Assertions.Any()
                || result.Assertions.Any(
                    a => a.Status == AssertionStatus.Error
                );

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
            if (!string.IsNullOrWhiteSpace(output))
            {
                AllureApi.AddAttachment(
                    "Console Output",
                    "text/plain",
                    Encoding.UTF8.GetBytes(output),
                    ".txt"
                );
            }
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

        private string ContainerId => $"tc-{_test.Id}";
    }
}
