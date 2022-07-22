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
        public enum NUnitHelpMethodType
        {
            SetUp,
            TearDown,
            OneTimeSetup,
            OneTimeTearDown
        }

        private readonly ITest _test;

        private string _containerGuid;
        private bool _isSetupFailed;
        private string _stepGuid;

        private StepResult _stepResult;
        private string _testResultGuid;

        public AllureNUnitHelper(ITest test)
        {
            _test = test;
        }

        private static AllureLifecycle AllureLifecycle => AllureLifecycle.Instance;

        private void StartTestContainer()
        {
            var testFixture = GetTestFixture(_test);
            _containerGuid = string.Concat(Guid.NewGuid().ToString(), "-tc-", testFixture.Id);
            var container = new TestResultContainer
            {
                uuid = _containerGuid,
                name = _test.FullName
            };
            AllureLifecycle.StartTestContainer(container);
        }

        private void StartTestCase()
        {
            _testResultGuid = string.Concat(Guid.NewGuid().ToString(), "-tr-", _test.Id);
            var testResult = new TestResult
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
            AllureLifecycle.StartTestCase(_containerGuid, testResult);
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

        private IEnumerable<string> GetNUnitHelpMethods(NUnitHelpMethodType type, TestFixture fixture)
        {
            switch (type)
            {
                case NUnitHelpMethodType.SetUp:
                    return fixture.SetUpMethods.Select(m => m.Name).ToList();
                case NUnitHelpMethodType.TearDown:
                    return fixture.TearDownMethods.Select(m => m.Name).ToList();
                case NUnitHelpMethodType.OneTimeSetup:
                    return fixture.OneTimeSetUpMethods.Select(m => m.Name).ToList();
                case NUnitHelpMethodType.OneTimeTearDown:
                    return fixture.OneTimeTearDownMethods.Select(m => m.Name).ToList();
                default:
                    throw new ArgumentOutOfRangeException(nameof(type), type, null);
            }
        }

        private void StartTestStep()
        {
            _stepGuid = string.Concat(Guid.NewGuid().ToString(), "-ts-", _test.Id);
            _stepResult = new StepResult {name = _test.Name};
            AllureLifecycle.StartStep(_stepGuid, _stepResult);
        }

        private List<FixtureResult> BuildFixtureResults(NUnitHelpMethodType type, TestFixture testFixture)
        {
            var fixtureResultsList = new HashSet<FixtureResult>();
            var testResult = TestExecutionContext.CurrentContext.CurrentResult;

            foreach (var method in GetNUnitHelpMethods(type, testFixture))
            {
                var fr = new FixtureResult {name = method};


                if (fr.start == 0)
                    fr.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
                if (fr.stop == 0)
                    fr.stop = fr.start;

                if (testResult.StackTrace != null && testResult.StackTrace.Contains(method))
                {
                    AllureLifecycle.UpdateTestCase(x => x.description += $"\n{method} {type} method failed\n");
                    fr.status = Status.failed;
                    fr.statusDetails.message = testResult.Message;
                    fr.statusDetails.trace = testResult.StackTrace;
                    if (type == NUnitHelpMethodType.SetUp) _isSetupFailed = true;
                }
                else
                {
                    fr.status = Status.passed;
                    if (type == NUnitHelpMethodType.OneTimeTearDown)
                    {
                        fr.statusDetails.message = "It's not possible to get status of OneTimeTearDown";
                        fr.statusDetails.trace = "See Allure.NUnit wiki";
                        fr.status = Status.none;
                    }
                }

                try
                {
                    if (testFixture.HasChildren)
                    {
                        var properties = _test.Properties;
                        if (properties.ContainsKey(method))
                        {
                            var methodProperty = (SetUpTearDownHelper) properties.Get(method);
                            fr.start = methodProperty.StartTime;
                            fr.stop = methodProperty.EndTime;
                            fr.statusDetails.message = methodProperty.Exception?.Message;
                            fr.statusDetails.trace = methodProperty.Exception?.StackTrace;
                            if (!string.IsNullOrEmpty(methodProperty.CustomName)) fr.name = methodProperty.CustomName;
                        }
                        else
                        {
                            properties = GetTestFixture(_test).Properties;
                            if (properties.ContainsKey(method))
                            {
                                var methodProperty = (SetUpTearDownHelper) properties.Get(method);
                                fr.start = methodProperty.StartTime;
                                fr.stop = methodProperty.EndTime;
                                fr.statusDetails.message = methodProperty.Exception?.Message;
                                fr.statusDetails.trace = methodProperty.Exception?.StackTrace;
                                if (!string.IsNullOrEmpty(methodProperty.CustomName))
                                    fr.name = methodProperty.CustomName;
                            }
                        }
                    }
                }
                catch (Exception)
                {
                    //
                }


                fr.stage = Stage.finished;
                fixtureResultsList.Add(fr);
            }

            return fixtureResultsList.ToList();
        }

        public void StopAll(bool isWrappedIntoStep)
        {
            var testFixture = GetTestFixture(_test);

            var listSetups = new List<FixtureResult>();
            listSetups.AddRange(BuildFixtureResults(NUnitHelpMethodType.OneTimeSetup, testFixture));
            listSetups.AddRange(BuildFixtureResults(NUnitHelpMethodType.SetUp, testFixture));

            var listTearDowns = new List<FixtureResult>();
            listTearDowns.AddRange(BuildFixtureResults(NUnitHelpMethodType.TearDown, testFixture));
            listTearDowns.AddRange(BuildFixtureResults(NUnitHelpMethodType.OneTimeTearDown, testFixture));

            var result = TestExecutionContext.CurrentContext.CurrentResult;
            try
            {
                if (isWrappedIntoStep)
                    AllureLifecycle.StopStep(step =>
                    {
                        step.statusDetails = new StatusDetails
                        {
                            message = result.Message,
                            trace = result.StackTrace
                        };
                        AllureLifecycle.AddAttachment("Console Output", "text/plain",
                            Encoding.UTF8.GetBytes(result.Output), ".txt");
                        if (_isSetupFailed)
                        {
                            step.status = Status.skipped;
                            step.name +=
                                $" skipped because of {listSetups.Select(sm => sm.name).FirstOrDefault()} failure";
                        }
                        else
                        {
                            step.status = GetNUnitStatus();
                        }
                    });

                StopTestCase();

                StopTestContainer(listSetups, listTearDowns);
            }
            catch (ArgumentNullException e)
            {
                Console.WriteLine(e.Message);
                // Ignore already killed Allure dictionary 
            }
        }


        private void StopTestCase()
        {
            UpdateTestDataFromAttributes();
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

        private void StopTestContainer(List<FixtureResult> listSetups, List<FixtureResult> listTearDowns)
        {
            AllureLifecycle.UpdateTestContainer(_containerGuid, cont =>
            {
                cont.befores.AddRange(listSetups);
                cont.afters.AddRange(listTearDowns);
            });
            AllureLifecycle.StopTestContainer(_containerGuid);
            AllureLifecycle.WriteTestContainer(_containerGuid);
        }

        public void StartAll(bool isWrappedIntoStep)
        {
            StartTestContainer();
            StartTestCase();
            if (isWrappedIntoStep) StartTestStep();
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
    }
}
