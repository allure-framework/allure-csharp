using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Runtime.CompilerServices;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Storage;
using Allure.Net.Commons.Writer;
using HeyRed.Mime;
using Newtonsoft.Json.Linq;

#nullable enable

[assembly: InternalsVisibleTo("Allure.Net.Commons.Tests")]

namespace Allure.Net.Commons
{
    /// <summary>
    /// A facade that allows to control the Allure context, set up allure model
    /// objects and emit output files.
    /// </summary>
    /// <remarks>
    /// This class is primarily intended to be used by a test framework
    /// integration. We don't advice to use it from test code unless strictly
    /// necessary.<br></br>
    /// NOTE: Modifications of the Allure context persist until either some
    /// method has affect them, or the execution context is restored to the
    /// point beyond the call that had introduced them.
    /// </remarks>
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
        /// Protects mutations of shared allure model objects against data
        /// races that may otherwise occur because of multithreaded access.
        /// </summary>
        readonly object modelMonitor = new();


        /// <summary>
        /// Captures the current value of Allure context.
        /// </summary>
        public AllureContext Context
        {
            get => this.storage.CurrentContext;
            private set => this.storage.CurrentContext = value;
        }

        /// <summary>
        /// Binds the provided value as the current Allure context and executes
        /// the specified function. The context is then restored to the initial
        /// value. This allows the Allure context to bypass .NET execution
        /// context boundaries.
        /// </summary>
        /// <param name="context">
        /// A context that was previously captured with <see cref="Context"/>.
        /// If it is null, the code is executed in the current context.
        /// </param>
        /// <param name="action">A code to run.</param>
        /// <returns>The context after the code is executed.</returns>
        public AllureContext RunInContext(
            AllureContext? context,
            Action action
        )
        {
            if (context is null)
            {
                action();
                return this.Context;
            }

            var originalContext = this.Context;
            try
            {
                this.Context = context;
                action();
                return this.Context;
            }
            finally
            {
                this.Context = originalContext;
            }
        }

        internal AllureLifecycle(): this(GetConfiguration())
        {
        }

        internal AllureLifecycle(
            Func<AllureConfiguration, IAllureResultsWriter> writerFactory
        ) : this(GetConfiguration(), writerFactory)
        {
        }

        internal AllureLifecycle(JObject config)
            : this(config, c => new FileSystemResultsWriter(c))
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

        /// <summary>
        /// Starts a new test container and pushes it into the container
        /// context making the container context active. The container becomes
        /// the current one in the current execution context.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Can't be called if the fixture or the test context is active.
        /// </remarks>
        /// <param name="container">A new test container to start.</param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StartTestContainer(
            TestResultContainer container
        )
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            this.storage.PutTestContainer(container.uuid, container);
            return this;
        }

        /// <summary>
        /// Applies the specified update function to the current test container.
        /// </summary>
        /// <remarks>
        /// Requires the container context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle UpdateTestContainer(
            Action<TestResultContainer> update
        )
        {
            var container = this.storage.CurrentTestContainer;
            lock (this.modelMonitor)
            {
                update.Invoke(container);
            }
            return this;
        }

        /// <summary>
        /// Stops the current test container.
        /// </summary>
        /// <remarks>
        /// Requires the container context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopTestContainer()
        {
            UpdateTestContainer(stopContainer);
            return this;
        }

        /// <summary>
        /// Writes the current test container and removes it from the context.
        /// If there are another test containers in the context, the most
        /// recently started one becomes the current container in the current
        /// execution context. Otherwise the container context is deactivated.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the container context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle WriteTestContainer()
        {
            var container = this.storage.CurrentTestContainer;
            this.storage.RemoveTestContainer(container.uuid);
            this.writer.Write(container);
            return this;
        }

        #endregion

        #region Fixture

        /// <summary>
        /// Starts a new before fixture and activates the fixture context with
        /// it. The fixture is set as the current one in the current execution
        /// context. Does nothing if the fixture context is already active.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the container context to be active.
        /// </remarks>
        /// <param name="result">A new fixture.</param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StartBeforeFixture(FixtureResult result)
        {
            this.UpdateTestContainer(container => container.befores.Add(result));
            this.StartFixture(result);
            return this;
        }

        /// <summary>
        /// Starts a new after fixture and activates the fixture context with
        /// it. The fixture is set as the current one in the current execution
        /// context. Does nothing if the fixture context is already active.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the container context to be active.
        /// </remarks>
        /// <param name="result">A new fixture.</param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StartAfterFixture(FixtureResult result)
        {
            this.UpdateTestContainer(c => c.afters.Add(result));
            this.StartFixture(result);
            return this;
        }

        /// <summary>
        /// Applies the specified update function to the current fixture.
        /// </summary>
        /// <remarks>
        /// Requires the fixture context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle UpdateFixture(
            Action<FixtureResult> update
        )
        {
            var fixture = this.storage.CurrentFixture;
            lock (this.modelMonitor)
            {
                update.Invoke(fixture);
            }
            return this;
        }

        /// <summary>
        /// Stops the current fixture and deactivates the fixture context.
        /// </summary>
        /// <param name="beforeStop">
        /// A function applied to the fixture result before it is stopped.
        /// </param>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Required the fixture context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopFixture(
            Action<FixtureResult> beforeStop
        )
        {
            this.UpdateFixture(beforeStop);
            return this.StopFixture();
        }

        /// <summary>
        /// Stops the current fixture and deactivates the fixture context.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Required the fixture context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopFixture()
        {
            this.UpdateFixture(fixture =>
            {
                fixture.stage = Stage.finished;
                fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            });
            this.storage.RemoveFixture();
            return this;
        }

        #endregion

        #region TestCase

        /// <summary>
        /// Starts a new test and activates the test context with it. The test
        /// becomes the current one in the current execution context.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the test context to be active.
        /// </remarks>
        /// <param name="testResult">A new test case.</param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StartTestCase(TestResult testResult)
        {
            var uuid = testResult.uuid;
            var containers = this.storage.CurrentContext.ContainerContext;
            lock (this.modelMonitor)
            {
                foreach (TestResultContainer container in containers)
                {
                    container.children.Add(uuid);
                }
            }
            testResult.stage = Stage.running;
            testResult.start = testResult.start == 0L
                ? DateTimeOffset.Now.ToUnixTimeMilliseconds()
                : testResult.start;
            this.storage.PutTestCase(uuid, testResult);
            return this;
        }

        /// <summary>
        /// Applies the specified update function to the current test.
        /// </summary>
        /// <remarks>
        /// Requires the test context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle UpdateTestCase(
            Action<TestResult> update
        )
        {
            var testResult = this.storage.CurrentTest;
            lock (this.modelMonitor)
            {
                update(testResult);
            }
            return this;
        }

        /// <summary>
        /// Stops the current test.
        /// </summary>
        /// <remarks>
        /// Requires the test context to be active.
        /// </remarks>
        /// <param name="beforeStop">
        /// A function applied to the test result before it is stopped.
        /// </param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopTestCase(
            Action<TestResult> beforeStop
        ) => this.UpdateTestCase(testResult =>
        {
            beforeStop(testResult);
            stopTestCase(testResult);
        });

        /// <summary>
        /// Stops the current test.
        /// </summary>
        /// <remarks>
        /// Requires the test context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopTestCase() =>
            this.UpdateTestCase(stopTestCase);

        /// <summary>
        /// Writes the current test and removes it from the context. The test
        /// context is then deactivated.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the test context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle WriteTestCase()
        {
            var testResult = this.storage.CurrentTest;
            string uuid;
            lock (this.modelMonitor)
            {
                uuid = testResult.uuid;
            }
            this.storage.RemoveTestCase(uuid);
            this.writer.Write(testResult);
            return this;
        }

        #endregion

        #region Step

        /// <summary>
        /// Starts a new step and pushes it into the step context making the
        /// step context active. The step becomes the current one in the
        /// current execution context.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires either the fixture or the test context to be active.
        /// </remarks>
        /// <param name="result">A new step.</param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StartStep(StepResult result)
        {
            result.stage = Stage.running;
            result.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            var parent = this.storage.CurrentStepContainer;
            lock (this.modelMonitor)
            {
                parent.steps.Add(result);
            }
            this.storage.PutStep(result);
            return this;
        }

        /// <summary>
        /// Applies the specified update function to the current step.
        /// </summary>
        /// <remarks>
        /// Requires the step context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle UpdateStep(Action<StepResult> update)
        {
            var stepResult = this.storage.CurrentStep;
            lock (this.modelMonitor)
            {
                update.Invoke(stepResult);
            }
            return this;
        }

        /// <summary>
        /// Stops the current step and removes it from the context. If there
        /// are another steps in the context, the most recently started one
        /// becomes the current step in the current execution context.
        /// Otherwise the step context is deactivated.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the step context to be active.
        /// </remarks>
        /// <param name="beforeStop">
        /// A function that is applied to the step result before it is stopped.
        /// </param>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopStep(Action<StepResult> beforeStop)
        {
            this.UpdateStep(beforeStop);
            return this.StopStep();
        }

        /// <summary>
        /// Stops the current step and removes it from the context. If there
        /// are another steps in the context, the most recently started one
        /// becomes the current step in the current execution context.
        /// Otherwise the step context is deactivated.
        /// </summary>
        /// <remarks>
        /// This method modifies the Allure context.<br></br>
        /// Requires the step context to be active.
        /// </remarks>
        /// <exception cref="InvalidOperationException"/>
        public virtual AllureLifecycle StopStep()
        {
            this.UpdateStep(step =>
            {
                step.stage = Stage.finished;
                step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            });
            this.storage.RemoveStep();
            return this;
        }

        #endregion

        #region Attachment

        // TODO: read file in background thread
        public virtual AllureLifecycle AddAttachment(
            string name,
            string type,
            string path
        )
        {
            var fileExtension = new FileInfo(path).Extension;
            return AddAttachment(name, type, File.ReadAllBytes(path), fileExtension);
        }

        public virtual AllureLifecycle AddAttachment(
            string name,
            string type,
            byte[] content,
            string fileExtension = ""
        )
        {
            var suffix = AllureConstants.ATTACHMENT_FILE_SUFFIX;
            var source = $"{CreateUuid()}{suffix}{fileExtension}";
            var attachment = new Attachment
            {
                name = name,
                type = type,
                source = source
            };
            this.writer.Write(source, content);
            var target = this.storage.CurrentStepContainer;
            lock (this.modelMonitor)
            {
                target.attachments.Add(attachment);
            }
            return this;
        }

        public virtual AllureLifecycle AddAttachment(
            string path,
            string? name = null
        )
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

        public virtual AllureLifecycle AddScreenDiff(
            string expectedPng,
            string actualPng,
            string diffPng
        ) => this.AddAttachment(expectedPng, "expected")
            .AddAttachment(actualPng, "actual")
            .AddAttachment(diffPng, "diff")
            .UpdateTestCase(
                x => x.labels.Add(Label.TestType("screenshotDiff"))
            );

        #endregion


        #region Privates

        static AllureLifecycle Initialize() => new();

        private static JObject GetConfiguration()
        {
            var configEnvVarName = AllureConstants.ALLURE_CONFIG_ENV_VARIABLE;
            var jsonConfigPath = Environment.GetEnvironmentVariable(
                configEnvVarName
            );

            if (jsonConfigPath != null && !File.Exists(jsonConfigPath))
            {
                throw new FileNotFoundException(
                    $"Couldn't find '{jsonConfigPath}' specified " +
                        $"in {configEnvVarName} environment variable"
                );
            }

            if (File.Exists(jsonConfigPath))
            {
                return JObject.Parse(File.ReadAllText(jsonConfigPath));
            }

            var defaultJsonConfigPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                AllureConstants.CONFIG_FILENAME
            );

            if (File.Exists(defaultJsonConfigPath))
            {
                return JObject.Parse(File.ReadAllText(defaultJsonConfigPath));
            }

            return JObject.Parse("{}");
        }

        private void StartFixture(FixtureResult fixtureResult)
        {
            this.storage.PutFixture(fixtureResult);
            fixtureResult.stage = Stage.running;
            fixtureResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
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

        #region Obsoleted

        [Obsolete(
            "This property is a rudimentary part of the API. It has no " +
                "effect and will be removed soon"
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public static Func<string>? CurrentTestIdGetter { get; set; }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartTestContainer(
            string parentUuid,
            TestResultContainer container
        )
        {
            container.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            UpdateTestContainer(parentUuid, c => c.children.Add(container.uuid));
            this.storage.PutTestContainer(container.uuid, container);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle UpdateTestContainer(
            string uuid,
            Action<TestResultContainer> update
        )
        {
            var container = this.storage.Get<TestResultContainer>(uuid);
            lock (this.modelMonitor)
            {
                update.Invoke(container);
            }
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StopTestContainer(string uuid) =>
            UpdateTestContainer(uuid, stopContainer);

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle WriteTestContainer(string uuid)
        {
            var container = this.storage.Get<TestResultContainer>(uuid);
            this.storage.RemoveTestContainer(uuid);
            this.writer.Write(container);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartBeforeFixture(
            FixtureResult result,
            out string uuid
        )
        {
            uuid = CreateUuid();
            StartBeforeFixture(uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartBeforeFixture(
            string uuid,
            FixtureResult result
        )
        {
            UpdateTestContainer(container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartBeforeFixture(
            string parentUuid,
            FixtureResult result,
            out string uuid
        )
        {
            uuid = CreateUuid();
            StartBeforeFixture(parentUuid, uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartBeforeFixture(
            string parentUuid,
            string uuid,
            FixtureResult result
        )
        {
            UpdateTestContainer(parentUuid, container => container.befores.Add(result));
            StartFixture(uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartAfterFixture(
            string parentUuid,
            FixtureResult result,
            out string uuid
        )
        {
            uuid = CreateUuid();
            StartAfterFixture(parentUuid, uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartAfterFixture(
            string parentUuid,
            string uuid,
            FixtureResult result
        )
        {
            UpdateTestContainer(
                parentUuid,
                container => container.afters.Add(result)
            );
            StartFixture(uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle UpdateFixture(
            string uuid,
            Action<FixtureResult> update
        )
        {
            var fixture = this.storage.Get<FixtureResult>(uuid);
            lock (this.modelMonitor)
            {
                update.Invoke(fixture);
            }
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StopFixture(string uuid)
        {
            this.UpdateFixture(fixture =>
            {
                fixture.stage = Stage.finished;
                fixture.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            });
            this.storage.RemoveFixture(uuid);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartTestCase(
            string containerUuid,
            TestResult testResult
        )
        {
            UpdateTestContainer(
                containerUuid,
                c => c.children.Add(testResult.uuid)
            );
            return StartTestCase(testResult);
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle UpdateTestCase(
            string uuid,
            Action<TestResult> update
        )
        {
            var testResult = this.storage.Get<TestResult>(uuid);
            lock (this.modelMonitor)
            {
                update(testResult);
            }
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StopTestCase(string uuid) =>
            this.UpdateTestCase(uuid, stopTestCase);

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle WriteTestCase(string uuid)
        {
            var testResult = this.storage.Get<TestResult>(uuid);
            this.storage.RemoveTestCase(uuid);
            this.writer.Write(testResult);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartStep(
            StepResult result,
            out string uuid
        )
        {
            uuid = CreateUuid();
            StartStep(this.storage.CurrentStepContainer, uuid, result);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartStep(
            string uuid,
            StepResult result
        ) => this.StartStep(this.storage.CurrentStepContainer, uuid, result);

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StartStep(
            string parentUuid,
            string uuid,
            StepResult stepResult
        ) => this.StartStep(
            this.storage.Get<ExecutableItem>(parentUuid),
            uuid,
            stepResult
        );

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle UpdateStep(
            string uuid,
            Action<StepResult> update
        )
        {
            var stepResult = storage.Get<StepResult>(uuid);
            lock (this.modelMonitor)
            {
                update.Invoke(stepResult);
            }
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle StopStep(string uuid)
        {
            this.UpdateStep(uuid, step =>
            {
                step.stage = Stage.finished;
                step.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            });
            this.storage.RemoveStep(uuid);
            return this;
        }

        [Obsolete(
            "Lifecycle methods with explicit uuids are obsolete. Use " +
                "their counterparts without uuids to manipulate the context."
        )]
        [EditorBrowsable(EditorBrowsableState.Never)]
        public virtual AllureLifecycle AddScreenDiff(
            string testCaseUuid,
            string expectedPng,
            string actualPng,
            string diffPng
        )
        {
            AddAttachment(expectedPng, "expected")
                .AddAttachment(actualPng, "actual")
                .AddAttachment(diffPng, "diff")
                .UpdateTestCase(testCaseUuid, x => x.labels.Add(Label.TestType("screenshotDiff")));

            return this;
        }

        [Obsolete]
        void StartFixture(string uuid, FixtureResult fixtureResult)
        {
            this.storage.PutFixture(uuid, fixtureResult);
            lock (this.modelMonitor)
            {
                fixtureResult.stage = Stage.running;
                fixtureResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            }
        }

        [Obsolete]
        AllureLifecycle StartStep(
            ExecutableItem parent,
            string uuid,
            StepResult stepResult
        )
        {
            stepResult.stage = Stage.running;
            stepResult.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
            lock (this.modelMonitor)
            {
                parent.steps.Add(stepResult);
            }
            this.storage.PutStep(uuid, stepResult);
            return this;
        }

        #endregion
    }
}