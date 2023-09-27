using Allure.Net.Commons.TestPlan;
using NUnit.Framework;
using System.Collections.Generic;
using System.Linq;

#nullable enable

namespace Allure.Net.Commons.Tests.SelectiveRunTests
{
    class TestPlanTests
    {
        [TestCase("null")]
        [TestCase("{}")]
        [TestCase("{\"tests\": []}")]
        public void EmptyTestPlanEnablesAllTests(string json)
        {
            var testPlan = AllureTestPlan.FromJson(json);

            Assert.That(testPlan, Is.Not.Null);
            Assert.That(
                testPlan.IsMatch("", null),
                Is.True
            );
        }

        [TestCase(null, false)]
        [TestCase("100", true)]
        [TestCase("101", false)]
        public void TestPlanWithIdEntry(
            string allureIdOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson = "{\"tests\": [{\"id\": \"100\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    "",
                    allureIdOfTestCase
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase("a", true)]
        [TestCase("A", false)]
        [TestCase("aa", false)]
        public void TestPlanWithSelectorEntry(
            string fullNameOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson =
                "{\"tests\": [{\"selector\": \"a\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    fullNameOfTestCase,
                    null
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase("a", "100", true)]
        [TestCase("b", "100", true)]
        [TestCase("a", "101", true)]
        [TestCase("a", null, true)]
        [TestCase("b", "101", false)]
        [TestCase("b", null, false)]
        public void TestPlanWithIdAndSelectorEntry(
            string fullNameOfTestCase,
            string allureIdOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson =
                "{\"tests\": [{\"selector\": \"a\", \"id\": \"100\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    fullNameOfTestCase,
                    allureIdOfTestCase
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase("100", true)]
        [TestCase("101", true)]
        [TestCase("102", false)]
        public void TestPlanWithMultipleIdEntries(
            string allureIdOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson =
                "{\"tests\": [{\"id\": \"100\"}, {\"id\": \"101\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    "",
                    allureIdOfTestCase
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase("a", true)]
        [TestCase("b", true)]
        [TestCase("c", false)]
        public void TestPlanWithMultipleSelectorEntries(
            string fullNameOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson =
                "{\"tests\": [{\"selector\": \"a\"}, {\"selector\": \"b\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    fullNameOfTestCase,
                    null
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase("a", "100", true)]
        [TestCase("b", "100", true)]
        [TestCase("a", "101", true)]
        [TestCase("a", null, true)]
        [TestCase("b", "101", false)]
        [TestCase("b", null, false)]
        public void TestPlanWithMultipleDifferentEntries(
            string fullNameOfTestCase,
            string allureIdOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson =
                "{\"tests\": [{\"id\": \"100\"}, {\"selector\": \"a\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    fullNameOfTestCase,
                    allureIdOfTestCase
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase("a", null, true)]
        [TestCase("a", "100", true)]
        [TestCase("a", "101", true)]
        [TestCase("a", "102", true)]
        [TestCase("b", null, true)]
        [TestCase("b", "100", true)]
        [TestCase("b", "101", true)]
        [TestCase("b", "102", true)]
        [TestCase("c", null, false)]
        [TestCase("c", "100", true)]
        [TestCase("c", "101", true)]
        [TestCase("c", "102", false)]
        public void TestPlanWithMultipleFullEntries(
            string fullNameOfTestCase,
            string allureIdOfTestCase,
            bool expectedMatch
        )
        {
            var testPlanJson =
                "{\"tests\": [{\"selector\": \"a\", \"id\": \"100\"}, {\"selector\": \"b\", \"id\": \"101\"}]}";
            var testPlan = AllureTestPlan.FromJson(testPlanJson);

            Assert.That(
                testPlan.IsMatch(
                    fullNameOfTestCase,
                    allureIdOfTestCase
                ),
                Is.EqualTo(expectedMatch)
            );
        }

        [TestCase(new string[] { }, null)]
        [TestCase(new[] { "ALLURE_ID", "100" }, "100")]
        [TestCase(new[] { "l1", "v1" }, null)]
        [TestCase(new[] { "AS_ID", "100" }, "100")]
        [TestCase(new[] { "allure_id", "100" }, "100")]
        [TestCase(new[] { "as_id", "100" }, "100")]
        [TestCase(new[]
        {
            "l", "v",
            "ALLURE_ID", "100"
        }, "100")]
        [TestCase(new[]
        {
            "l", "v",
            "AS_ID", "100"
        }, "100")]
        [TestCase(new[]
        {
            "allure_id", "100",
            "ALLURE_ID", "101",
            "AS_ID", "102"
        }, "100")]
        [TestCase(new[]
        {
            "AS_ID", "100",
            "ALLURE_ID", "101"
        }, "101")]
        public void GetAllureIdTest(string[] labels, string? expectedAllureId)
        {
            Assert.That(
                AllureTestPlan.GetAllureId(
                    CreateLabels(labels)
                ),
                Is.EqualTo(expectedAllureId)
            );
        }

        static IEnumerable<Label> CreateLabels(params string[] labels) =>
            Enumerable.Range(0, labels.Length / 2).Select(
                i => new Label()
                {
                    name = labels[2 * i],
                    value = labels[2 * i + 1]
                }
            );
    }
}
