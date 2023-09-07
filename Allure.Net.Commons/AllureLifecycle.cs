using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Storage;
using Allure.Net.Commons.Writer;
using HeyRed.Mime;
using Newtonsoft.Json.Linq;

#nullable enable

[assembly: InternalsVisibleTo("Allure.Net.Commons.Tests")]

namespace Allure.Net.Commons;

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
    private static readonly Lazy<AllureLifecycle> instance =
        new(Initialize);

    public IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters =>
        new ReadOnlyDictionary<Type, ITypeFormatter>(typeFormatters);

    readonly AllureStorage storage;
    readonly AsyncLocal<AllureContext> context = new();

    readonly IAllureResultsWriter writer;

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
        get => this.context.Value ??= new AllureContext();
        private set => this.context.Value = value;
    }

    internal AllureLifecycle() : this(GetConfiguration())
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

    /// <summary>
    /// Binds the provided value as the current Allure context. This allows the
    /// Allure context to bypass .NET execution context boundaries. Use this
    /// function in a highly concurrent environments where framework hooks and
    /// user's test functions might all be run in different execution contexts.
    /// For other scenarios consider using <see cref="RunInContext"/> first.
    /// </summary>
    /// <param name="context">
    /// A context that was previously captured with <see cref="Context"/>.
    /// It can't be null.
    /// </param>
    /// <exception cref="ArgumentNullException"></exception>
    public void RestoreContext(AllureContext context)
    {
        this.Context = context ?? throw new ArgumentNullException(
            nameof(context)
        );
    }

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
        this.storage.Put(container.uuid, container);
        this.UpdateContext(c => c.WithContainer(container));
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
        var container = this.Context.CurrentContainer;
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
        var container = this.Context.CurrentContainer;
        this.storage.Remove<TestResultContainer>(container.uuid);
        this.UpdateContext(c => c.WithNoLastContainer());
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
        this.UpdateTestContainer(c => c.befores.Add(result));
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
        var fixture = this.Context.CurrentFixture;
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
        this.UpdateFixture(stopAllureItem);
        this.UpdateContext(c => c.WithNoFixtureContext());
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
        var containers = this.Context.ContainerContext;
        lock (this.modelMonitor)
        {
            foreach (TestResultContainer container in containers)
            {
                container.children.Add(uuid);
            }
        }
        this.storage.Put(uuid, testResult);
        this.UpdateContext(c => c.WithTestContext(testResult));
        this.UpdateTestCase(startAllureItem);
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
        var testResult = this.Context.CurrentTest;
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
    ) => this.UpdateTestCase(
        Chain(beforeStop, stopAllureItem)
    );

    /// <summary>
    /// Stops the current test.
    /// </summary>
    /// <remarks>
    /// Requires the test context to be active.
    /// </remarks>
    /// <exception cref="InvalidOperationException"/>
    public virtual AllureLifecycle StopTestCase() =>
        this.UpdateTestCase(stopAllureItem);

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
        var testResult = this.Context.CurrentTest;
        string uuid;
        lock (this.modelMonitor)
        {
            uuid = testResult.uuid;
        }
        this.storage.Remove<TestResult>(uuid);
        this.UpdateContext(c => c.WithNoTestContext());
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
        var parent = this.Context.CurrentStepContainer;
        lock (this.modelMonitor)
        {
            parent.steps.Add(result);
        }
        this.UpdateContext(c => c.WithStep(result));
        this.UpdateStep(startAllureItem);
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
        var stepResult = this.Context.CurrentStep;
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
        this.UpdateStep(stopAllureItem);
        this.UpdateContext(c => c.WithNoLastStep());
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
        return this.AddAttachment(
            name,
            type,
            File.ReadAllBytes(path),
            fileExtension
        );
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
        var target = this.Context.CurrentStepContainer;
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

    /// <summary>
    /// Attaches screen diff images to the current test case.
    /// </summary>
    /// <remarks>
    /// Requires the test context to be active.
    /// </remarks>
    /// <param name="expectedPng">A path to the actual screen.</param>
    /// <param name="actualPng">A path to the expected screen.</param>
    /// <param name="diffPng">A path to the screen diff.</param>
    /// <exception cref="InvalidOperationException"/>
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
        this.UpdateContext(c => c.WithFixtureContext(fixtureResult));
        this.UpdateFixture(startAllureItem);
    }

    static readonly Action<TestResultContainer> stopContainer =
        c => c.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();

    static readonly Action<ExecutableItem> startAllureItem =
        item =>
        {
            item.stage = Stage.running;
            item.start = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        };

    static readonly Action<ExecutableItem> stopAllureItem =
        item =>
        {
            item.stage = Stage.finished;
            item.stop = DateTimeOffset.Now.ToUnixTimeMilliseconds();
        };

    void UpdateContext(Func<AllureContext, AllureContext> updateFn)
    {
        this.Context = updateFn(this.Context);
    }

    static string CreateUuid() =>
        Guid.NewGuid().ToString("N");

    static Action<T> Chain<T>(params Action<T>[] actions) => v =>
    {
        foreach (var action in actions)
        {
            action(v);
        }
    };

    #endregion

    #region Obsoleted

    internal const string EXPLICIT_STATE_MGMT_OBSOLETE =
        "Explicit allure state management is obsolete. Methods with " +
            "explicit uuid parameters will be removed in the future. Use " +
            "their counterparts without uuids to manipulate the current" +
            " context.";

    internal const string API_RUDIMENT_OBSOLETE_MSG =
        "This is a rudimentary part of the API. It has no " +
            "effect and will be removed in the future.";

    [Obsolete(API_RUDIMENT_OBSOLETE_MSG)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public static Func<string>? CurrentTestIdGetter { get; set; }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartTestContainer(
        string parentUuid,
        TestResultContainer container
    )
    {
        this.UpdateTestContainer(
            parentUuid,
            c => c.children.Add(container.uuid)
        );
        this.StartTestContainer(container);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
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

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StopTestContainer(string uuid) =>
        this.UpdateTestContainer(uuid, stopContainer);

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle WriteTestContainer(string uuid)
    {
        var container = this.storage.Remove<TestResultContainer>(uuid);
        this.UpdateContext(c => ContextWithNoContainer(c, uuid));
        this.writer.Write(container);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartBeforeFixture(
        FixtureResult result,
        out string uuid
    )
    {
        uuid = CreateUuid();
        this.StartBeforeFixture(uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartBeforeFixture(
        string uuid,
        FixtureResult result
    )
    {
        this.UpdateTestContainer(c => c.befores.Add(result));
        this.StartFixture(uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartBeforeFixture(
        string parentUuid,
        FixtureResult result,
        out string uuid
    )
    {
        uuid = CreateUuid();
        this.StartBeforeFixture(parentUuid, uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartBeforeFixture(
        string parentUuid,
        string uuid,
        FixtureResult result
    )
    {
        this.UpdateTestContainer(parentUuid, c => c.befores.Add(result));
        this.StartFixture(uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartAfterFixture(
        string parentUuid,
        FixtureResult result,
        out string uuid
    )
    {
        uuid = CreateUuid();
        this.StartAfterFixture(parentUuid, uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartAfterFixture(
        string parentUuid,
        string uuid,
        FixtureResult result
    )
    {
        this.UpdateTestContainer(parentUuid, c => c.afters.Add(result));
        this.StartFixture(uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
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

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StopFixture(string uuid)
    {
        this.UpdateFixture(uuid, stopAllureItem);
        var fixture = this.storage.Remove<FixtureResult>(uuid);
        if (ReferenceEquals(fixture, this.Context.FixtureContext))
        {
            this.UpdateContext(c => c.WithNoFixtureContext());
        }
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartTestCase(
        string containerUuid,
        TestResult testResult
    )
    {
        this.UpdateTestContainer(
            containerUuid,
            c => c.children.Add(testResult.uuid)
        );
        return this.StartTestCase(testResult);
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
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

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StopTestCase(string uuid) =>
        this.UpdateTestCase(uuid, stopAllureItem);

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle WriteTestCase(string uuid)
    {
        var testResult = this.storage.Remove<TestResult>(uuid);
        if (this.Context.TestContext?.uuid == uuid)
        {
            this.UpdateContext(c => c.WithNoTestContext());
        }
        this.writer.Write(testResult);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartStep(
        StepResult result,
        out string uuid
    )
    {
        uuid = CreateUuid();
        this.StartStep(this.Context.CurrentStepContainer, uuid, result);
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StartStep(
        string uuid,
        StepResult result
    ) => this.StartStep(this.Context.CurrentStepContainer, uuid, result);

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
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

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
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

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle StopStep(string uuid)
    {
        this.UpdateStep(uuid, stopAllureItem);
        var stepResult = this.storage.Remove<StepResult>(uuid);
        this.UpdateContext(c => ContextWithNoStep(c, stepResult));
        return this;
    }

    [Obsolete(EXPLICIT_STATE_MGMT_OBSOLETE)]
    [EditorBrowsable(EditorBrowsableState.Never)]
    public virtual AllureLifecycle AddScreenDiff(
        string testCaseUuid,
        string expectedPng,
        string actualPng,
        string diffPng
    ) => this.AddAttachment(expectedPng, "expected")
        .AddAttachment(actualPng, "actual")
        .AddAttachment(diffPng, "diff")
        .UpdateTestCase(
            testCaseUuid,
            x => x.labels.Add(Label.TestType("screenshotDiff"))
        );

    [Obsolete]
    void StartFixture(string uuid, FixtureResult fixtureResult)
    {
        this.storage.Put(uuid, fixtureResult);
        this.UpdateContext(c => c.WithFixtureContext(fixtureResult));
        this.UpdateStep(uuid, startAllureItem);
    }

    [Obsolete]
    AllureLifecycle StartStep(
        ExecutableItem parent,
        string uuid,
        StepResult stepResult
    )
    {
        lock (this.modelMonitor)
        {
            parent.steps.Add(stepResult);
        }
        this.storage.Put(uuid, stepResult);
        this.UpdateContext(c => c.WithStep(stepResult));
        this.UpdateStep(uuid, startAllureItem);
        return this;
    }

    [Obsolete]
    static AllureContext ContextWithNoContainer(
        AllureContext context,
        string uuid
    )
    {
        var containersToPushAgain = new Stack<TestResultContainer>();
        while (context.CurrentContainer.uuid != uuid)
        {
            containersToPushAgain.Push(context.CurrentContainer);
            context = context.WithNoLastContainer();
            if (context.ContainerContext.IsEmpty)
            {
                throw new InvalidOperationException(
                    $"Container {uuid} is not in the current context"
                );
            }
        }
        context = context.WithNoLastContainer();
        while (containersToPushAgain.Any())
        {
            context = context.WithContainer(
                containersToPushAgain.Pop()
            );
        }
        return context;
    }

    [Obsolete]
    static AllureContext ContextWithNoStep(
        AllureContext context,
        StepResult stepResult
    )
    {
        var stepsToPushAgain = new Stack<StepResult>();
        while (!ReferenceEquals(context.CurrentStep, stepResult))
        {
            stepsToPushAgain.Push(context.CurrentStep);
            context = context.WithNoLastStep();
            if (context.StepContext.IsEmpty)
            {
                throw new InvalidOperationException(
                    $"Step {stepResult.name} is not in the current context"
                );
            }
        }
        context = context.WithNoLastStep();
        while (stepsToPushAgain.Any())
        {
            context = context.WithStep(
                stepsToPushAgain.Pop()
            );
        }
        return context;
    }

    #endregion
}