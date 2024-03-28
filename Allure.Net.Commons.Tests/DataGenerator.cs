using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace Allure.Net.Commons.Tests
{
    enum Fixture { BeforeFeature, BeforeScenario, AfterFeature, AfterScenario }
    class DataGenerator
    {
        internal static (string path, byte[] content) GetAttachment(string extension = "")
        {
            var path = $"{Guid.NewGuid().ToString()}{extension}";
            var content = "test";
            File.WriteAllText(path, content);
            return (path, File.ReadAllBytes(path));
        }

        internal static TestResult GetTestResult()
        {
            var uuid = Guid.NewGuid().ToString("N");
            return new()
            {
                uuid = uuid,
                name = uuid,
                description = uuid,
                fullName = uuid,
                labels = new()
                {
                    Label.Feature("Feature 1"),
                    Label.Story("Story 2"),
                    Label.Thread()
                }
            };
        }

        internal static List<TestResult> GetTestResults(int capacity = 10)
        {
            var trs = new List<TestResult>(capacity);
            Parallel.For(0, capacity, (i) => trs.Add(GetTestResult()));
            return trs;
        }


        internal static TestResultContainer GetTestResultContainer()
        {
            var trc = new TestResultContainer();
            trc.uuid = Guid.NewGuid().ToString("N");
            trc.name = trc.uuid;
            trc.description = trc.uuid;
            trc.links = new List<Link>()
            {
                Link.Issue(name : new Random().Next(999999).ToString(), url:"/bug"),
                Link.Tms(name : new Random().Next(999999).ToString(), url:"/test")
            };
            return trc;
        }

        internal static (string uuid, FixtureResult fixture) GetFixture(Fixture type) =>
            (Guid.NewGuid().ToString(),
            new FixtureResult()
            {
                name = type.ToString(),
                steps = GetSteps(1)
            });

        internal static (string uuid, StepResult step) GetStep() =>
            (Guid.NewGuid().ToString(),
            new StepResult()
            {
                name = Guid.NewGuid().ToString("N"),
                stage = Stage.pending,
                parameters = new List<Parameter>
                    {
                        new Parameter() { name = "param1", value = "value1"},
                        new Parameter() { name = "param2", value = "value2"}
                    }
            });

        internal static List<StepResult> GetSteps(int capacity = 10)
        {
            var steps = new List<StepResult>(capacity);
            Parallel.For(0, capacity, (i) => steps.Add(GetStep().step));
            return steps;
        }
    }
}
