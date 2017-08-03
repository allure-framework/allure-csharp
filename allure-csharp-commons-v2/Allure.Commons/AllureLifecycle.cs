using Allure.Commons.Storage;
using Allure.Commons.Writer;
using Castle.DynamicProxy;
using HeyRed.Mime;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Allure.Commons
{
    public class AllureLifecycle
    {
        private static object lockobj = new object();
        private AllureStorage storage;
        private IAllureResultsWriter writer;
        private static AllureLifecycle instance;

        public IConfiguration Configuration { get; private set; }
        public static AllureLifecycle Instance //=> instance = instance ?? CreateInstance();
        {
            get
            {
                if (instance == null)
                {
                    lock (lockobj)
                    {
                        instance = instance ?? CreateInstance();
                    }
                }

                return instance;
            }
        }
        protected AllureLifecycle(IConfigurationRoot configuration)
        {
            this.Configuration = configuration;
            this.writer = GetDefaultResultsWriter(configuration);
            this.storage = new AllureStorage();
        }

        public static AllureLifecycle CreateInstance()
        {

            var config = new ConfigurationBuilder()
        .AddJsonFile(AllureConstants.CONFIG_FILENAME, optional: true)
        .Build();

            bool.TryParse(config["allure:logging"], out bool logging);
            return (logging) ?
                (AllureLifecycle)new ProxyGenerator().CreateClassProxy(typeof(AllureLifecycle),
                    new object[] { config }, new LoggingInterceptor()) :
                new AllureLifecycle(config);

        }

        #region TestContainer
        public virtual AllureLifecycle StartTestContainer(TestResultContainer container)
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(container);
            return this;
        }

        public virtual AllureLifecycle StartTestContainer(string parentUuid, TestResultContainer container)
        {
            UpdateTestContainer(parentUuid, c => c.children.Add(container.uuid));
            this.StartTestContainer(container);
            return this;
        }

        public virtual AllureLifecycle UpdateTestContainer(string uuid, Action<TestResultContainer> update)
        {
            update.Invoke(storage.Get<TestResultContainer>(uuid));
            return this;
        }

        public virtual AllureLifecycle StopTestContainer(string uuid)
        {
            UpdateTestContainer(uuid, c => c.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return this;
        }

        public virtual AllureLifecycle WriteTestContainer(string uuid)
        {
            writer.Write(storage.Remove<TestResultContainer>(uuid));
            return this;
        }
        #endregion

        #region Fixture
        public virtual AllureLifecycle StartBeforeFixture(string parentUuid, string uuid, FixtureResult result)
        {
            UpdateTestContainer(parentUuid, container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        public virtual AllureLifecycle StartAfterFixture(string parentUuid, string uuid, FixtureResult result)
        {
            UpdateTestContainer(parentUuid, container => container.afters.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        public virtual AllureLifecycle UpdateFixture(Action<FixtureResult> update)
        {
            UpdateFixture(storage.GetRootStep(), update);
            return this;
        }
        public virtual AllureLifecycle UpdateFixture(string uuid, Action<FixtureResult> update)
        {
            update.Invoke(storage.Get<FixtureResult>(uuid));
            return this;
        }

        public virtual AllureLifecycle StopFixture(Action<FixtureResult> beforeStop)
        {
            UpdateFixture(beforeStop);
            return StopFixture(storage.GetRootStep());
        }
        public virtual AllureLifecycle StopFixture(string uuid)
        {
            var fixture = storage.Remove<FixtureResult>(uuid);
            storage.ClearStepContext();
            fixture.stage = Stage.finished;
            fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }
        #endregion

        #region TestCase

        public virtual AllureLifecycle StartTestCase(string containerUuid, TestResult testResult)
        {
            this.UpdateTestContainer(containerUuid, c => c.children.Add(testResult.uuid));
            return StartTestCase(testResult);
        }

        public virtual AllureLifecycle StartTestCase(TestResult testResult)
        {
            testResult.stage = Stage.running;
            testResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(testResult);
            storage.ClearStepContext();
            storage.StartStep(testResult.uuid);
            return this;
        }

        public virtual AllureLifecycle UpdateTestCase(string uuid, Action<TestResult> update)
        {
            update.Invoke(storage.Get<TestResult>(uuid));
            return this;
        }

        public virtual AllureLifecycle UpdateTestCase(Action<TestResult> update)
        {
            return UpdateTestCase(storage.GetRootStep(), update);
        }

        public virtual AllureLifecycle StopTestCase(Action<TestResult> beforeStop)
        {
            UpdateTestCase(beforeStop);
            return StopTestCase(storage.GetRootStep());
        }

        public virtual AllureLifecycle StopTestCase(string uuid)
        {
            var testResult = storage.Get<TestResult>(uuid);
            testResult.stage = Stage.finished;
            testResult.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            return this;
        }

        public virtual AllureLifecycle WriteTestCase(string uuid)
        {
            writer.Write(storage.Remove<TestResult>(uuid));
            return this;
        }

        #endregion

        #region Step
        public virtual AllureLifecycle StartStep(string uuid, StepResult result)
        {
            StartStep(storage.GetCurrentStep(), uuid, result);
            return this;
        }

        public virtual AllureLifecycle StartStep(string parentUuid, string uuid, StepResult stepResult)
        {
            stepResult.stage = Stage.running;
            stepResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StartStep(uuid);
            storage.AddStep(parentUuid, uuid, stepResult);
            return this;
        }

        public virtual AllureLifecycle UpdateStep(Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(storage.GetCurrentStep()));
            return this;
        }

        public virtual AllureLifecycle UpdateStep(string uuid, Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(uuid));
            return this;
        }
        public virtual AllureLifecycle StopStep(Action<StepResult> beforeStop)
        {
            UpdateStep(beforeStop);
            return StopStep(storage.GetCurrentStep());
        }

        public virtual AllureLifecycle StopStep(string uuid)
        {
            var step = storage.Remove<StepResult>(uuid);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StopStep();
            return this;
        }

        public virtual AllureLifecycle StopStep()
        {
            StopStep(storage.GetCurrentStep());
            return this;
        }

        #endregion

        #region Attachment

        public virtual AllureLifecycle AddAttachment(string name, string type, string path)
        {
            var fileExtension = new FileInfo(path).Extension;
            return this.AddAttachment(name, type, File.ReadAllBytes(path), fileExtension);
        }
        public virtual AllureLifecycle AddAttachment(string name, string type, byte[] content, string fileExtension = "")
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

        public virtual AllureLifecycle AddAttachment(string path, string name = null)
        {
            name = name ?? Path.GetFileName(path);
            var type = MimeTypesMap.GetMimeType(path);
            return this.AddAttachment(name, type, path);
        }

        #endregion

        #region Extensions
        public virtual AllureLifecycle AddScreenDiff(string expectedPng, string actualPng, string diffPng)
        {
            this
                .AddAttachment("expected", "image/png", "expected.png")
                .AddAttachment("actual", "image/png", "actual.png")
                .AddAttachment("diff", "image/png", "diff.png")
                .UpdateTestCase(x => x.labels.Add(Label.TestType("screenshotDiff")));

            return this;
        }

        #endregion


        #region Privates
        private void StartFixture(string uuid, FixtureResult fixtureResult)
        {
            storage.Put(uuid, fixtureResult);
            fixtureResult.stage = Stage.running;
            fixtureResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            storage.StartStep(uuid);
        }
        private IAllureResultsWriter GetDefaultResultsWriter(IConfigurationRoot configuration)
        {
            var resultsFolder = configuration["allure:directory"]
                ?? AllureConstants.DEFAULT_RESULTS_FOLDER;

            bool.TryParse(configuration["allure:cleanup"], out bool cleanup);

            return new FileSystemResultsWriter(resultsFolder, cleanup);
        }

        #endregion

    }
}
