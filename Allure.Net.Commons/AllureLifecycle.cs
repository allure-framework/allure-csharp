using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.CompilerServices;
using System.Threading;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Storage;
using Allure.Net.Commons.Writer;
using HeyRed.Mime;
using Newtonsoft.Json.Linq;

#nullable enable

[assembly: InternalsVisibleTo("Allure.Net.Commons.Tests")]

namespace Allure.Net.Commons
{
    public class AllureLifecycle
    {
        private readonly Dictionary<Type, ITypeFormatter> typeFormatters = new();

        public IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters =>
            new ReadOnlyDictionary<Type, ITypeFormatter>(typeFormatters);

        private static readonly Lazy<AllureLifecycle> instance =
            new(Initialize);
        private readonly AllureStorage storage;
        private readonly IAllureResultsWriter writer;


        /// <summary>
        /// Gets or sets an execution context of Allure. Use this property if
        /// the context is set not in the same async domain where a
        /// test/fixture function is executed.
        /// </summary>
        /// <remarks>
        /// This property is intended to be used by Allure integrations with
        /// test frameworks, not by end user's code.
        /// </remarks>
        public AllureContext Context
        {
            get => this.storage.CurrentContext;
            set => this.storage.CurrentContext = value;
        }

        /// <summary> Method to get the key for separation the steps for different tests. </summary>
        public static Func<string> CurrentTestIdGetter { get; set; } =
            () => Thread.CurrentThread.ManagedThreadId.ToString();

        internal AllureLifecycle(): this(GetConfiguration())
        {
        }

        internal AllureLifecycle(
            Func<AllureConfiguration, IAllureResultsWriter> writerFactory
        ) : this(GetConfiguration(), writerFactory)
        {
        }

        internal AllureLifecycle(JObject config): this(config, c => new FileSystemResultsWriter(c))
        {
        }

        internal AllureLifecycle(
            JObject config,
            Func<AllureConfiguration, IAllureResultsWriter> writerFactory
        )
        {
            JsonConfiguration = config.ToString();
            AllureConfiguration = AllureConfiguration.ReadFromJObject(config);
            writer = writerFactory(AllureConfiguration);
            storage = new AllureStorage();
        }

        public string JsonConfiguration { get; private set; }

        public AllureConfiguration AllureConfiguration { get; }

        public string ResultsDirectory => writer.ToString();

        public static AllureLifecycle Instance { get => instance.Value; }

        public void AddTypeFormatter<T>(TypeFormatter<T> typeFormatter) =>
            AddTypeFormatterImpl(typeof(T), typeFormatter);

        private void AddTypeFormatterImpl(Type type, ITypeFormatter formatter) =>
            typeFormatters[type] = formatter;

        #region TestContainer

        public virtual AllureLifecycle StartTestContainer(TestResultContainer container)
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.storage.CurrentTestContainerOrNull?.children.Add(
                container.uuid
            );
            storage.PutTestContainer(container);
            return this;
        }

        public virtual AllureLifecycle StartTestContainer(string parentUuid, TestResultContainer container)
        {
            UpdateTestContainer(parentUuid, c => c.children.Add(container.uuid));
            StartTestContainer(container);
            return this;
        }

        public virtual AllureLifecycle UpdateTestContainer(Action<TestResultContainer> update)
        {
            update.Invoke(storage.CurrentTestContainer);
            return this;
        }

        public virtual AllureLifecycle UpdateTestContainer(string uuid, Action<TestResultContainer> update)
        {
            update.Invoke(storage.Get<TestResultContainer>(uuid));
            return this;
        }

        public virtual AllureLifecycle StopTestContainer()
        {
            UpdateTestContainer(stopContainer);
            return this;
        }

        public virtual AllureLifecycle StopTestContainer(string uuid)
        {
            UpdateTestContainer(uuid, stopContainer);
            return this;
        }

        public virtual AllureLifecycle WriteTestContainer()
        {
            writer.Write(
                storage.RemoveTestContainer()
            );
            return this;
        }

        public virtual AllureLifecycle WriteTestContainer(string uuid)
        {
            writer.Write(
                storage.RemoveTestContainer(uuid)
            );
            return this;
        }

        #endregion

        #region Fixture

        public virtual AllureLifecycle StartBeforeFixture(FixtureResult result)
        {
            UpdateTestContainer(container => container.befores.Add(result));
            this.StartFixture(result);
            return this;
        }

        public virtual AllureLifecycle StartBeforeFixture(FixtureResult result, out string uuid)
        {
            uuid = CreateUuid();
            StartBeforeFixture(uuid, result);
            return this;
        }

        public virtual AllureLifecycle StartBeforeFixture(string uuid, FixtureResult result)
        {
            UpdateTestContainer(container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        public virtual AllureLifecycle StartBeforeFixture(string parentUuid, FixtureResult result, out string uuid)
        {
            uuid = Guid.NewGuid().ToString("N");
            StartBeforeFixture(parentUuid, uuid, result);
            return this;
        }

        public virtual AllureLifecycle StartBeforeFixture(string parentUuid, string uuid, FixtureResult result)
        {
            UpdateTestContainer(parentUuid, container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        public virtual AllureLifecycle StartAfterFixture(FixtureResult result)
        {
            this.UpdateTestContainer(c => c.afters.Add(result));
            this.StartFixture(result);
            return this;
        }

        public virtual AllureLifecycle StartAfterFixture(string parentUuid, FixtureResult result, out string uuid)
        {
            uuid = CreateUuid();
            StartAfterFixture(parentUuid, uuid, result);
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
            update?.Invoke(storage.CurrentFixture);
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
            return this.StopFixture();
        }

        public virtual AllureLifecycle StopFixture()
        {
            var fixture = this.storage.RemoveFixture();
            fixture.stage = Stage.finished;
            fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }

        public virtual AllureLifecycle StopFixture(string uuid)
        {
            var fixture = this.storage.RemoveFixture(uuid);
            fixture.stage = Stage.finished;
            fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }

        #endregion

        #region TestCase

        public virtual AllureLifecycle StartTestCase(string containerUuid, TestResult testResult)
        {
            UpdateTestContainer(containerUuid, c => c.children.Add(testResult.uuid));
            return StartTestCase(testResult);
        }

        public virtual AllureLifecycle StartTestCase(TestResult testResult)
        {
            this.storage.CurrentTestContainerOrNull?.children.Add(testResult.uuid);
            testResult.stage = Stage.running;
            testResult.start = testResult.start == 0L
                ? DateTimeOffset.Now.ToUnixTimeMilliseconds()
                : testResult.start;
            this.storage.PutTestCase(testResult);
            return this;
        }

        public virtual AllureLifecycle UpdateTestCase(
            string uuid,
            Action<TestResult> update
        )
        {
            var testResult = this.storage.Get<TestResult>(uuid);
            update(testResult);
            return this;
        }

        public virtual AllureLifecycle UpdateTestCase(
            Action<TestResult> update
        )
        {
            update(this.storage.CurrentTest);
            return this;
        }

        public virtual AllureLifecycle StopTestCase(
            Action<TestResult> beforeStop
        )
        {
            var testResult = this.storage.CurrentTest;
            beforeStop(testResult);
            stopTestCase(testResult);
            return this;
        }

        public virtual AllureLifecycle StopTestCase() =>
            this.UpdateTestCase(stopTestCase);

        public virtual AllureLifecycle StopTestCase(string uuid) =>
            this.UpdateTestCase(uuid, stopTestCase);

        public virtual AllureLifecycle WriteTestCase()
        {
            this.writer.Write(
                this.storage.RemoveTestCase()
            );
            return this;
        }

        public virtual AllureLifecycle WriteTestCase(string uuid)
        {
            this.writer.Write(
                this.storage.RemoveTestCase(uuid)
            );
            return this;
        }

        #endregion

        #region Step

        public virtual AllureLifecycle StartStep(StepResult result)
        {
            result.stage = Stage.running;
            result.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.storage.CurrentStepContainer.steps.Add(result);
            this.storage.PutStep(result);
            return this;
        }

        public virtual AllureLifecycle StartStep(StepResult result, out string uuid)
        {
            uuid = CreateUuid();
            StartStep(this.storage.CurrentStepContainer, uuid, result);
            return this;
        }
        
        public virtual AllureLifecycle StartStep(
            string uuid,
            StepResult result
        ) => this.StartStep(this.storage.CurrentStepContainer, uuid, result);

        public virtual AllureLifecycle StartStep(
            string parentUuid,
            string uuid,
            StepResult stepResult
        ) => this.StartStep(
            this.storage.Get<ExecutableItem>(parentUuid),
            uuid,
            stepResult
        );

        public virtual AllureLifecycle UpdateStep(Action<StepResult> update)
        {
            update.Invoke(this.storage.CurrentStep);
            return this;
        }

        public virtual AllureLifecycle UpdateStep(string uuid, Action<StepResult> update)
        {
            update.Invoke(storage.Get<StepResult>(uuid));
            return this;
        }

        public virtual AllureLifecycle StopStep(Action<StepResult> beforeStop)
        {
            this.UpdateStep(beforeStop);
            return this.StopStep();
        }

        public virtual AllureLifecycle StopStep(string uuid)
        {
            var step = this.storage.RemoveStep(uuid);
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }

        public virtual AllureLifecycle StopStep()
        {
            var step = this.storage.RemoveStep();
            step.stage = Stage.finished;
            step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            return this;
        }

        #endregion

        #region Attachment

        // TODO: read file in background thread
        public virtual AllureLifecycle AddAttachment(string name, string type, string path)
        {
            var fileExtension = new FileInfo(path).Extension;
            return AddAttachment(name, type, File.ReadAllBytes(path), fileExtension);
        }

        public virtual AllureLifecycle AddAttachment(string name, string type, byte[] content,
            string fileExtension = "")
        {
            var source = $"{CreateUuid()}{AllureConstants.ATTACHMENT_FILE_SUFFIX}{fileExtension}";
            var attachment = new Attachment
            {
                name = name,
                type = type,
                source = source
            };
            writer.Write(source, content);
            this.storage.CurrentStepContainer.attachments.Add(attachment);
            return this;
        }

        public virtual AllureLifecycle AddAttachment(string path, string? name = null)
        {
            name ??= Path.GetFileName(path);
            var type = MimeTypesMap.GetMimeType(path);
            return AddAttachment(name, type, path);
        }

        #endregion

        #region Extensions

        public virtual void CleanupResultDirectory()
        {
            writer.CleanUp();
        }

        public virtual AllureLifecycle AddScreenDiff(string testCaseUuid, string expectedPng, string actualPng,
            string diffPng)
        {
            AddAttachment(expectedPng, "expected")
                .AddAttachment(actualPng, "actual")
                .AddAttachment(diffPng, "diff")
                .UpdateTestCase(testCaseUuid, x => x.labels.Add(Label.TestType("screenshotDiff")));

            return this;
        }

        #endregion


        #region Privates

        static AllureLifecycle Initialize() => new();

        private static JObject GetConfiguration()
        {
            var jsonConfigPath = Environment.GetEnvironmentVariable(AllureConstants.ALLURE_CONFIG_ENV_VARIABLE);

            if (jsonConfigPath != null && !File.Exists(jsonConfigPath))
                throw new FileNotFoundException(
                    $"Couldn't find '{jsonConfigPath}' specified in {AllureConstants.ALLURE_CONFIG_ENV_VARIABLE} environment variable");

            if (File.Exists(jsonConfigPath))
                return JObject.Parse(File.ReadAllText(jsonConfigPath));

            var defaultJsonConfigPath =
                Path.Combine(AppDomain.CurrentDomain.BaseDirectory, AllureConstants.CONFIG_FILENAME);

            if (File.Exists(defaultJsonConfigPath))
                return JObject.Parse(File.ReadAllText(defaultJsonConfigPath));

            return JObject.Parse("{}");
        }

        private void StartFixture(FixtureResult fixtureResult)
        {
            storage.PutFixture(fixtureResult);
            fixtureResult.stage = Stage.running;
            fixtureResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        void StartFixture(string uuid, FixtureResult fixtureResult)
        {
            storage.PutFixture(uuid, fixtureResult);
            fixtureResult.stage = Stage.running;
            fixtureResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        }

        AllureLifecycle StartStep(
            ExecutableItem parent,
            string uuid,
            StepResult stepResult
        )
        {
            stepResult.stage = Stage.running;
            stepResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            parent.steps.Add(stepResult);
            this.storage.PutStep(uuid, stepResult);
            return this;
        }

        static readonly Action<TestResultContainer> stopContainer =
            c => c.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();

        static readonly Action<TestResult> stopTestCase =
            tr =>
            {
                tr.stage = Stage.finished;
                tr.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            };


        static string CreateUuid() =>
            Guid.NewGuid().ToString("N");

        #endregion
    }
}