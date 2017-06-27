using Allure.Commons.Storage;
using Allure.Commons.Writer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Allure.Commons
{
    public class AllureLifeсycle
    {
        private AllureStorage storage = new AllureStorage();
        private IAllureResultsWriter writer;

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile(AllureConstants.CONFIG_FILENAME, optional: true)
            .Build();

        public AllureLifeсycle()
        {
            writer = GetDefaultResultsWriter();
        }

        #region Fixture
        public AllureLifeсycle StartBeforeFixture(string parentUuid, string uuid, FixtureResult result)
        {
            UpdateTestContainer(parentUuid, container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        public AllureLifeсycle StartAfterFixture(string parentUuid, string uuid, FixtureResult result)
        {
            UpdateTestContainer(parentUuid, container => container.afters.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        private void StartFixture(string uuid, FixtureResult result)
        {
            storage.AddFixture(uuid, result);
            result.stage = Stage.running;
            result.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            storage.StartStep(uuid);
        }
        public AllureLifeсycle UpdateFixture(Action<FixtureResult> update)
        {
            UpdateFixture(storage.GetRootStep(), update);
            return this;
        }
        public AllureLifeсycle UpdateFixture(string uuid, Action<FixtureResult> update)
        {
            update.Invoke(storage.Get<FixtureResult>(uuid));
            return this;
        }

        public AllureLifeсycle StopFixture(Action<FixtureResult> beforeStop)
        {
            UpdateFixture(beforeStop);
            return StopFixture(storage.GetRootStep());
        }
        public AllureLifeсycle StopFixture(string uuid)
        {
            var fixture = storage.Remove<FixtureResult>(uuid);
            storage.ClearStepContext();
            fixture.stage = Stage.finished;
            fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }
        #endregion

        #region TestContainer
        public AllureLifeсycle StartTestContainer(TestResultContainer container)
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(container);
            return this;
        }

        public AllureLifeсycle StartTestContainer(string parentUuid, TestResultContainer container)
        {
            UpdateTestContainer(parentUuid, c => c.children.Add(container.uuid));
            this.StartTestContainer(container);
            return this;
        }

        public AllureLifeсycle UpdateTestContainer(string uuid, Action<TestResultContainer> update)
        {
            update.Invoke(storage.Get<TestResultContainer>(uuid));
            return this;
        }

        public AllureLifeсycle StopTestContainer(string uuid)
        {
            UpdateTestContainer(uuid, c => c.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return this;
        }

        public AllureLifeсycle WriteTestContainer(string uuid)
        {
            writer.Write(storage.Remove<TestResultContainer>(uuid));
            return this;
        }
        #endregion

        #region TestCase

        public AllureLifeсycle StartTestCase(string parentUuid, TestResult testResult)
        {
            this.UpdateTestContainer(parentUuid, c => c.children.Add(testResult.uuid));
            return StartTestCase(testResult);
        }

        public AllureLifeсycle StartTestCase(TestResult testResult)
        {
            testResult.stage = Stage.running;
            testResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(testResult);
            storage.ClearStepContext();
            storage.StartStep(testResult.uuid);
            return this;
        }

        public AllureLifeсycle UpdateTestCase(string uuid, Action<TestResult> update)
        {
            update.Invoke(storage.Get<TestResult>(uuid));
            return this;
        }

        public AllureLifeсycle UpdateTestCase(Action<TestResult> update)
        {
            UpdateTestCase(storage.GetRootStep(), update);
            return this;
        }

        public AllureLifeсycle StopTestCase(Action<TestResult> beforeStop)
        {
            UpdateTestCase(beforeStop);
            return StopTestCase(storage.GetRootStep());
        }

        public AllureLifeсycle StopTestCase(string uuid)
        {
            var testResult = storage.Get<TestResult>(uuid);
            testResult.stage = Stage.finished;
            testResult.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            return this;
        }

        public AllureLifeсycle WriteTestCase(string uuid)
        {
            writer.Write(storage.Remove<TestResult>(uuid));
            return this;

        }

        #endregion

        #region Step
        public AllureLifeсycle StartStep(string uuid, StepResult result)
        {
            StartStep(storage.GetCurrentStep(), uuid, result);
            return this;
        }

        public AllureLifeсycle StartStep(string parentUuid, string uuid, StepResult stepResult)
        {
            stepResult.stage = Stage.running;
            stepResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StartStep(uuid);
            storage.AddStep(parentUuid, uuid, stepResult);
            return this;
        }

        public AllureLifeсycle UpdateStep(Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(storage.GetCurrentStep()));
            return this;
        }

        public AllureLifeсycle UpdateStep(string uuid, Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(uuid));
            return this;
        }
        public AllureLifeсycle StopStep(Action<StepResult> beforeStop)
        {
            UpdateStep(beforeStop);
            return StopStep(storage.GetCurrentStep());
        }

        public AllureLifeсycle StopStep(string uuid)
        {
            var step = storage.Remove<StepResult>(uuid);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StopStep();
            return this;
        }

        public AllureLifeсycle StopStep()
        {
            StopStep(storage.GetCurrentStep());
            return this;
        }

        #endregion

        #region Attachment

        public AllureLifeсycle AddAttachment(string name, string type, string path)
        {
            var fileExtension = new FileInfo(path).Extension;
            return this.AddAttachment(name, type, File.ReadAllBytes(path), fileExtension);
        }
        public AllureLifeсycle AddAttachment(string name, string type, byte[] content, string fileExtension = "")
        {
            var source = $"{Guid.NewGuid().ToString("N")}{AllureConstants.ATTACHMENT_FILE_SUFFIX}{fileExtension}";
            var attachment = new Attachment()
            {
                name = name,
                type = type,
                source = source
            };
            writer.Write(source, content);
            storage.Get<ExecutableItem>(storage.GetCurrentStep()).attachments.Add(attachment);
            return this;
        }

        private void WriteAttachment(string source, byte[] content)
        {
            //writer.Write(attachmentSource, stream);
        }
        #endregion

        #region Extensions
        public AllureLifeсycle AddScreenDiff(string expectedPng, string actualPng, string diffPng)
        {
            this
                .AddAttachment("expected", "image/png", "expected.png")
                .AddAttachment("actual", "image/png", "actual.png")
                .AddAttachment("diff", "image/png", "diff.png")
                .UpdateTestCase(x => x.labels.Add(Label.TestType("screenshotDiff")));

            return this;
        }

        #endregion
        private IAllureResultsWriter GetDefaultResultsWriter()
        {
            var resultsFolder = configuration["allure:directory"]
                ?? AllureConstants.DEFAULT_RESULTS_FOLDER;

            var cleanup = bool.Parse(configuration["allure:cleanup"]);

            return new FileSystemResultsWriter(resultsFolder, cleanup);
        }

    }
}
