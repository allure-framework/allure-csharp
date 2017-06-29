using Allure.Commons.Storage;
using Allure.Commons.Writer;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Allure.Commons
{
    public class AllureLifecycle
    {
        private AllureStorage storage = new AllureStorage();
        private IAllureResultsWriter writer;

        private IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile(AllureConstants.CONFIG_FILENAME, optional: true)
            .Build();

        public IConfiguration Configuration => configuration;

        public AllureLifecycle()
        {
            writer = GetDefaultResultsWriter();
        }

        #region Fixture
        public AllureLifecycle StartBeforeFixture(string parentUuid, string uuid, FixtureResult result)
        {
            UpdateTestContainer(parentUuid, container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        public AllureLifecycle StartAfterFixture(string parentUuid, string uuid, FixtureResult result)
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
        public AllureLifecycle UpdateFixture(Action<FixtureResult> update)
        {
            UpdateFixture(storage.GetRootStep(), update);
            return this;
        }
        public AllureLifecycle UpdateFixture(string uuid, Action<FixtureResult> update)
        {
            update.Invoke(storage.Get<FixtureResult>(uuid));
            return this;
        }

        public AllureLifecycle StopFixture(Action<FixtureResult> beforeStop)
        {
            UpdateFixture(beforeStop);
            return StopFixture(storage.GetRootStep());
        }
        public AllureLifecycle StopFixture(string uuid)
        {
            var fixture = storage.Remove<FixtureResult>(uuid);
            storage.ClearStepContext();
            fixture.stage = Stage.finished;
            fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }
        #endregion

        #region TestContainer
        public AllureLifecycle StartTestContainer(TestResultContainer container)
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(container);
            return this;
        }

        public AllureLifecycle StartTestContainer(string parentUuid, TestResultContainer container)
        {
            UpdateTestContainer(parentUuid, c => c.children.Add(container.uuid));
            this.StartTestContainer(container);
            return this;
        }

        public AllureLifecycle UpdateTestContainer(string uuid, Action<TestResultContainer> update)
        {
            update.Invoke(storage.Get<TestResultContainer>(uuid));
            return this;
        }

        public AllureLifecycle StopTestContainer(string uuid)
        {
            UpdateTestContainer(uuid, c => c.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return this;
        }

        public AllureLifecycle WriteTestContainer(string uuid)
        {
            writer.Write(storage.Remove<TestResultContainer>(uuid));
            return this;
        }
        #endregion

        #region TestCase

        public AllureLifecycle StartTestCase(string parentUuid, TestResult testResult)
        {
            this.UpdateTestContainer(parentUuid, c => c.children.Add(testResult.uuid));
            return StartTestCase(testResult);
        }

        public AllureLifecycle StartTestCase(TestResult testResult)
        {
            testResult.stage = Stage.running;
            testResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(testResult);
            storage.ClearStepContext();
            storage.StartStep(testResult.uuid);
            return this;
        }

        public AllureLifecycle UpdateTestCase(string uuid, Action<TestResult> update)
        {
            update.Invoke(storage.Get<TestResult>(uuid));
            return this;
        }

        public AllureLifecycle UpdateTestCase(Action<TestResult> update)
        {
            UpdateTestCase(storage.GetRootStep(), update);
            return this;
        }

        public AllureLifecycle StopTestCase(Action<TestResult> beforeStop)
        {
            UpdateTestCase(beforeStop);
            return StopTestCase(storage.GetRootStep());
        }

        public AllureLifecycle StopTestCase(string uuid)
        {
            var testResult = storage.Get<TestResult>(uuid);
            testResult.stage = Stage.finished;
            testResult.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            return this;
        }

        public AllureLifecycle WriteTestCase(string uuid)
        {
            writer.Write(storage.Remove<TestResult>(uuid));
            return this;

        }

        #endregion

        #region Step
        public AllureLifecycle StartStep(string uuid, StepResult result)
        {
            StartStep(storage.GetCurrentStep(), uuid, result);
            return this;
        }

        public AllureLifecycle StartStep(string parentUuid, string uuid, StepResult stepResult)
        {
            stepResult.stage = Stage.running;
            stepResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StartStep(uuid);
            storage.AddStep(parentUuid, uuid, stepResult);
            return this;
        }

        public AllureLifecycle UpdateStep(Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(storage.GetCurrentStep()));
            return this;
        }

        public AllureLifecycle UpdateStep(string uuid, Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(uuid));
            return this;
        }
        public AllureLifecycle StopStep(Action<StepResult> beforeStop)
        {
            UpdateStep(beforeStop);
            return StopStep(storage.GetCurrentStep());
        }

        public AllureLifecycle StopStep(string uuid)
        {
            var step = storage.Remove<StepResult>(uuid);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StopStep();
            return this;
        }

        public AllureLifecycle StopStep()
        {
            StopStep(storage.GetCurrentStep());
            return this;
        }

        #endregion

        #region Attachment

        public AllureLifecycle AddAttachment(string name, string type, string path)
        {
            var fileExtension = new FileInfo(path).Extension;
            return this.AddAttachment(name, type, File.ReadAllBytes(path), fileExtension);
        }
        public AllureLifecycle AddAttachment(string name, string type, byte[] content, string fileExtension = "")
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
        public AllureLifecycle AddScreenDiff(string expectedPng, string actualPng, string diffPng)
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

            bool.TryParse(configuration["allure:cleanup"], out bool cleanup);

            return new FileSystemResultsWriter(resultsFolder, cleanup);
        }

    }
}
