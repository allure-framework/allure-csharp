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
    public sealed class AllureNUnitHelper : ITestResultAccessor
    {
        public TestResultContainer TestResultContainer { get; set; }
        public TestResult TestResult { get; set; }

        private readonly ITest _test;

        private string _containerGuid;
        private string _stepGuid;

        private StepResult _stepResult;
        private string _testResultGuid;

        public AllureNUnitHelper(ITest test)
        {
            _test = test;
        }

        private static AllureLifecycle AllureLifecycle => AllureLifecycle.Instance;

        internal void StartTestContainer()
        {
            StepsHelper.TestResultAccessor = this;
            TestResultContainer = new TestResultContainer
            {
                uuid = ContainerId,
                name = _test.FullName
            };
            AllureLifecycle.StartTestContainer(TestResultContainer);
        }

        internal void StartTestCase()
        {
            _testResultGuid = string.Concat(Guid.NewGuid().ToString(), "-tr-", _test.Id);
            TestResult = new TestResult
            {
                uuid = _testResultGuid,
                name = _test.Name,
                historyId = _test.FullName,
                fullName = _test.FullName,
                labels = new List<Label>
                {
                    Label.Thread(),
                    Label.Host(),
                    Label.Package(_test.ClassName?.Substring(0, _test.ClassName.LastIndexOf('.'))),
                    Label.TestMethod(_test.MethodName),
                    Label.TestClass(_test.ClassName?.Substring(_test.ClassName.LastIndexOf('.') + 1))
                }
            };
            AllureLifecycle.StartTestCase(ContainerId, TestResult);
        }

        private TestFixture GetTestFixture(ITest test)
        {
            var currentTest = test;
            var isTestSuite = currentTest.IsSuite;
            while (isTestSuite != true)
            {
                currentTest = currentTest.Parent;
                if (currentTest is ParameterizedMethodSuite) currentTest = currentTest.Parent;
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
                AllureLifecycle.UpdateTestCase(x => x.parameters.Add(new Parameter
                {
                    // ReSharper disable once AccessToModifiedClosure
                    name = $"Param #{i}",
                    // ReSharper disable once AccessToModifiedClosure                   
                    value = _test.Arguments[i] == null ? "NULL" : _test.Arguments[i].ToString()
                }));
            }

            AllureLifecycle.UpdateTestCase(x => x.statusDetails = new StatusDetails
            {
                message = string.IsNullOrWhiteSpace(TestContext.CurrentContext.Result.Message)
                ? TestContext.CurrentContext.Test.Name 
                : TestContext.CurrentContext.Result.Message,
                trace = TestContext.CurrentContext.Result.StackTrace
            });

            AllureLifecycle.StopTestCase(testCase => testCase.status = GetNUnitStatus());
            AllureLifecycle.WriteTestCase(_testResultGuid);
        }

        internal void StopTestContainer()
        {
            AllureLifecycle.StopTestContainer(ContainerId);
            AllureLifecycle.WriteTestContainer(ContainerId);
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
                    var config = allureSection?.ToObject<AllureExtendedConfiguration>();
                    if (config?.BrokenTestData != null)
                        foreach (var word in config.BrokenTestData)
                            if (result.Message.Contains(word))
                                return Status.broken;
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
                AllureLifecycle.UpdateTestCase(x => x.description += $"{p}\n");

            foreach (var p in GetTestProperties(PropertyNames.Author))
                AllureLifecycle.UpdateTestCase(x => x.labels.Add(Label.Owner(p)));

            foreach (var p in GetTestProperties(PropertyNames.Category))
                AllureLifecycle.UpdateTestCase(x => x.labels.Add(Label.Tag(p)));

            var attributes = _test.Method.GetCustomAttributes<AllureTestCaseAttribute>(true).ToList();
            attributes.AddRange(GetTestFixture(_test).GetCustomAttributes<AllureTestCaseAttribute>(true).ToList());

            attributes.ForEach(a =>
            {
                AllureLifecycle.UpdateTestCase(a.UpdateTestResult);
            });
        }

        private void AddConsoleOutputAttachment()
        {
            var output = TestExecutionContext.CurrentContext.CurrentResult.Output;
            AllureLifecycle.AddAttachment("Console Output", "text/plain", 
                Encoding.UTF8.GetBytes(output), ".txt");
        }

        private IEnumerable<string> GetTestProperties(string name)
        {
            var list = new List<string>();
            var currentTest = _test;
            while (currentTest.GetType() != typeof(TestSuite) && currentTest.GetType() != typeof(TestAssembly))
            {
                if (currentTest.Properties.ContainsKey(name))
                    if (currentTest.Properties[name].Count > 0)
                        for (var i = 0; i < currentTest.Properties[name].Count; i++)
                            list.Add(currentTest.Properties[name][i].ToString());

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
            var currentResult = TestExecutionContext.CurrentContext.CurrentResult;

            if (!string.IsNullOrEmpty(currentResult.Output))
            {
                AllureLifecycle.Instance.AddAttachment("Console Output", "text/plain",
                    Encoding.UTF8.GetBytes(currentResult.Output), ".txt");    
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
            
            var testFixture = GetTestFixture(TestExecutionContext.CurrentContext.CurrentTest);
            testFixture.Properties.Set("OneTimeSetUpResult", fixtureResult);
        }
        
        public void AddOneTimeSetupResult()
        {
            var testFixture = GetTestFixture(TestExecutionContext.CurrentContext.CurrentTest);
            FixtureResult fixtureResult = null;

            fixtureResult = testFixture.Properties.Get("OneTimeSetUpResult") as FixtureResult;

            if (fixtureResult != null && fixtureResult.steps.Any())
            {
                AllureLifecycle.UpdateTestContainer(TestResultContainer.uuid, container =>
                {
                    container.befores.Add(fixtureResult);
                });
            }
        }
    }
}
