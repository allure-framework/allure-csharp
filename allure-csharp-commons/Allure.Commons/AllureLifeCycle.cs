using Allure.Commons.Storage;
using Allure.Commons.Writer;
using Microsoft.Extensions.Configuration;
using System;


namespace Allure.Commons
{
    public class AllureLifeCycle
    {
        private AllureStorage storage = new AllureStorage();
        private IAllureResultsWriter writer;

        IConfigurationRoot configuration = new ConfigurationBuilder()
            .AddJsonFile(AllureConstants.CONFIG_FILENAME, optional: true)
            .Build();

        public AllureLifeCycle()
        {
            writer = GetDefaultResultsWriter();
        }

        #region TestContainer
        public AllureLifeCycle StartTestContainer(TestResultContainer container)
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.Put(container);
            return this;
        }

        public AllureLifeCycle StartTestContainer(string parentUuid, TestResultContainer container)
        {
            UpdateTestContainer(parentUuid, c => c.children.Add(container.uuid));
            this.StartTestContainer(container);
            return this;
        }

        public AllureLifeCycle UpdateTestContainer(string uuid, Action<TestResultContainer> update)
        {
            update.Invoke(storage.Get<TestResultContainer>(uuid));
            return this;
        }

        public AllureLifeCycle StopTestContainer(string uuid)
        {
            UpdateTestContainer(uuid, c => c.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds());
            return this;
        }

        public AllureLifeCycle WriteTestContainer(string uuid)
        {
            writer.Write(storage.Remove<TestResultContainer>(uuid));
            return this;
        }
        #endregion

        #region TestResult
        public AllureLifeCycle ScheduleTestCase(string parentUuid, TestResult testResult)
        {
            UpdateTestContainer(parentUuid, c=>c.children.Add(testResult.uuid));
            ScheduleTestCase(testResult);
            return this;
        }

        public AllureLifeCycle ScheduleTestCase(TestResult testResult)
        {
            testResult.stage = Stage.scheduled;
            storage.Put(testResult);
            return this;
        }

        public AllureLifeCycle StartTestCase(string uuid)
        {
            var testResult = storage.Get<TestResult>(uuid);
            testResult.stage = Stage.running;
            testResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            storage.StartStep(uuid);
            return this;
        }

        public AllureLifeCycle UpdateTestCase(string uuid, Action<TestResult> update)
        {
            update.Invoke(storage.Get<TestResult>(uuid));
            return this;
        }

        public AllureLifeCycle UpdateTestCase(Action<TestResult> update)
        {
            update.Invoke(storage.Get<TestResult>(storage.GetRootStep()));
            return this;
        }

        public AllureLifeCycle StopTestCase(string uuid)
        {
            var testResult = storage.Get<TestResult>(uuid);
            testResult.stage = Stage.finished;
            testResult.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.ClearStepContext();
            return this;
        }

        public AllureLifeCycle WriteTestCase(string uuid)
        {
            writer.Write(storage.Remove<TestResult>(uuid));
            return this;

        }

        #endregion

        #region Step
        public AllureLifeCycle StartStep(string uuid, StepResult result)
        {
            StartStep(storage.GetCurrentStep(), uuid, result);
            return this;
        }

        public AllureLifeCycle StartStep(string parentUuid, string uuid, StepResult stepResult)
        {
            stepResult.stage = Stage.running;
            stepResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StartStep(uuid);
            storage.AddStep(parentUuid, uuid, stepResult);
            return this;
        }

        public AllureLifeCycle UpdateStep(Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(storage.GetCurrentStep()));
            return this;
        }

        public AllureLifeCycle UpdateStep(string uuid, Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(uuid));
            return this;
        }

        public AllureLifeCycle StopStep(string uuid)
        {
            var step = storage.Remove<StepResult>(uuid);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            storage.StopStep();
            return this;
        }

        public AllureLifeCycle StopStep()
        {
            StopStep(storage.GetCurrentStep());
            return this;
        }

        #endregion

        private IAllureResultsWriter GetDefaultResultsWriter()
        {
            var resultsFolder = configuration[AllureConstants.CONFIG_RESULTS_FOLDER_KEY] 
                ?? AllureConstants.DEFAULT_RESULTS_FOLDER;
            return new FileSystemResultsWriter(resultsFolder);
        }

    }
}
