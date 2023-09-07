using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Allure.Net.Commons;
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
            AllureLifecycle.StartTestContainer(new()
            {
                uuid = ContainerId,
                name = _test.FullName
            });
        }

        internal void StartTestCase()
        {
            var testResult = new TestResult
            {
                uuid = string.Concat(
                    Guid.NewGuid().ToString(),
                    "-tr-",
                    _test.Id
                ),
                name = _test.Name,
                historyId = _test.FullName,
                fullName = _test.FullName,
                labels = new List<Label>
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Package(
                        GetNamespace(_test.ClassName)
                    ),
                    Label.TestMethod(_test.MethodName),
                    Label.TestClass(
                        GetClassName(_test.ClassName)
                    )
                }
            };
            AllureLifecycle.StartTestCase(testResult);
        }

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

        private TestFixture GetTestFixture(ITest test)
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

        internal void StopTestCase()
        {
            UpdateTestDataFromAttributes();
            AddConsoleOutputAttachment();
            
            for (var i = 0; i < _test.Arguments.Length; i++)
            {
                AllureLifecycle.UpdateTestCase(
                    x => x.parameters.Add(
                        new Parameter
                        {
                            // ReSharper disable once AccessToModifiedClosure
                            name = $"Param #{i}",
                            // ReSharper disable once AccessToModifiedClosure                   
                            value = _test.Arguments[i] == null
                                ? "NULL"
                                : _test.Arguments[i].ToString()
                        }
                    )
                );
            }

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

        private void UpdateTestDataFromAttributes()
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

            var attributes = _test.Method
                .GetCustomAttributes<AllureTestCaseAttribute>(true)
                .ToList();
            attributes.AddRange(
                GetTestFixture(_test)
                    .GetCustomAttributes<AllureTestCaseAttribute>(true)
                    .ToList()
            );

            attributes.ForEach(a =>
            {
                AllureLifecycle.UpdateTestCase(a.UpdateTestResult);
            });
        }

        private void AddConsoleOutputAttachment()
        {
            var output = TestExecutionContext
                .CurrentContext
                .CurrentResult
                .Output;
            AllureLifecycle.AddAttachment(
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
