using System;
using System.Collections.Generic;
using System.Globalization;
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
        static Dictionary<Type, string> LiteralSuffixes { get; } = new()
        {
            { typeof(uint), "u" },
            { typeof(long), "L" },
            { typeof(ulong), "UL" },
            { typeof(float), "f" },
        };

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
            ApplyDefaultSuiteHierarchy(_test);
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


            AllureLifecycle.StopTestCase(testCase =>
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
                name = ResolveDisplayName(test),
                titlePath = [.. EnumerateTitlePathElements(test)],
                labels = [
                    Label.Thread(),
                    Label.Host(),
                    Label.Language(),
                    Label.Framework("NUnit"),
                    Label.Package(test.ClassName),
                    Label.TestMethod(test.MethodName),
                    Label.TestClass(
                        GetClassName(test.ClassName)
                    ),
                    ..ModelFunctions.EnumerateEnvironmentLabels(),
                    ..ModelFunctions.EnumerateGlobalLabels(),
                ]
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

        static string ResolveDisplayName(ITest test) =>
            test.Parent switch
            {
                ParameterizedMethodSuite suite => suite.Name,
                _ => test.Name,
            };

        static IEnumerable<ITest> EnumerateTestElements(ITest test)
        {
            Stack<ITest> stack = [];
            for (; test is not null; test = test.Parent)
            {
                stack.Push(test);
            }
            return stack;
        }

        static IEnumerable<string> EnumerateTitlePathElements(ITest test) =>
            EnumerateTestElements(test).Skip(1).Select(suite => suite switch
            {
                TestAssembly a => a.Assembly?.GetName()?.Name ?? a.Name,
                _ => suite.Name,
            });

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
            || result.Assertions.Any(a => a.Status == AssertionStatus.Error
            );

        static void SetIdentifiers(ITest test, TestResult testResult)
        {
            if (AllureLifecycle.AllureConfiguration.UseLegacyIds)
            {
                SetLegacyIdentifiers(test, testResult);
                return;
            }

            testResult.uuid = IdFunctions.CreateUUID();
            testResult.fullName = CreateFullName(test);
        }

        /// <summary>
        /// For test fixtures with no parameters, returns the same value as
        /// <see cref="IdFunctions.CreateFullName(System.Reflection.MethodInfo)"/>.
        /// For test fixtures with parameters, inserts the arguments between the class and method IDs.
        /// </summary>
        static string CreateFullName(ITest test)
        {
            var testMethod = test.Method.MethodInfo;
            var testFixtureClass = testMethod.DeclaringType;
            var testFixtureArgs = GetTestFixture(test).Arguments;

            var testFixtureClassPart = IdFunctions.GetTypeId(testFixtureClass);
            var testFixtureArgsPart = testFixtureArgs.Any()
                ? $"({string.Join(",", testFixtureArgs.Select(FormatTestFixtureArg))})"
                : "";
            var methodPart = IdFunctions.GetMethodId(testMethod);


            return $"{testFixtureClassPart}{testFixtureArgsPart}.{methodPart}";
        }

        /// <summary>
        /// Converts a test fixture argument to a string. Doesn't depend on the
        /// currently installed locale.
        /// </summary>
        /// <remarks>
        /// For possible values and types, see <see href="https://learn.microsoft.com/en-us/dotnet/csharp/language-reference/language-specification/attributes#2324-attribute-parameter-types">here</see>.
        /// </remarks>
        static string FormatTestFixtureArg(object value) => value switch
        {
            null => "null",
            string text => FormatFunctions.Format(text),
            Type type => $"<{IdFunctions.GetTypeId(type)}>",
            Array array => FormatArray(array),
            char c => FormatChar(c),
            _ => FormatPrimitive(value),
        };

        static string FormatArray(Array array) =>
            $"[{string.Join(",", array.Cast<object>().Select(FormatTestFixtureArg))}]";

        static string FormatChar(char c)
        {
            var text = FormatFunctions.Format(c);
            return $"'{text.Substring(1, text.Length - 2)}'";
        }

        static string FormatPrimitive(object value)
        {
            var text = Convert.ToString(value, CultureInfo.InvariantCulture);
            return LiteralSuffixes.TryGetValue(value.GetType(), out var suffix)
                ? $"{text}{suffix}"
                : text;
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
            var lastDotIndex = StripTypeArgs(classFullName)?.LastIndexOf('.') ?? -1;
            return lastDotIndex == -1
                ? null
                : classFullName.Substring(
                    0,
                    lastDotIndex
                );
        }

        static string GetClassName(string classFullName)
        {
            var lastDotIndex = StripTypeArgs(classFullName)?.LastIndexOf('.') ?? -1;
            return lastDotIndex == -1
                ? classFullName
                : classFullName.Substring(
                    lastDotIndex + 1
                );
        }

        static string StripTypeArgs(string classFullName)
        {
            var typeArgsStart = classFullName?.IndexOf('<') ?? -1;
            return typeArgsStart == -1
                ? classFullName
                : classFullName.Substring(0, typeArgsStart);
        }

        static TestFixture GetTestFixture(ITest test)
        {
            var currentTest = test;

            while (currentTest != null)
            {
                if (currentTest is TestFixture testFixture)
                {
                    return testFixture;
                }

                currentTest = currentTest.Parent;
            }

            throw new InvalidOperationException(
                $"Could not find TestFixture in the hierarchy for test: {test.FullName}. " +
                $"Test type: {test.GetType().Name}"
            );
        }

        internal static void ApplyDefaultSuiteHierarchy(ITest test)
        {
            var testClassFullName = GetTestFixture(test).FullName;
            var assemblyName = test.TypeInfo?.Assembly?.GetName().Name;
            var @namespace = GetNamespace(testClassFullName);
            var className = GetClassName(testClassFullName);

            AllureLifecycle.UpdateTestCase(
                testResult => ModelFunctions.EnsureSuites(
                    testResult,
                    assemblyName,
                    @namespace,
                    className
                )
            );
        }

        private void UpdateTestDataFromNUnitProperties()
        {
            foreach (var p in GetTestProperties(PropertyNames.Description))
            {
                AllureLifecycle.UpdateTestCase(x => x.description += $"{p}\n"
                );
            }

            foreach (var p in GetTestProperties(PropertyNames.Author))
            {
                AllureLifecycle.UpdateTestCase(x => x.labels.Add(Label.Owner(p))
                );
            }

            foreach (var p in GetTestProperties(PropertyNames.Category))
            {
                AllureLifecycle.UpdateTestCase(x => x.labels.Add(Label.Tag(p))
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
