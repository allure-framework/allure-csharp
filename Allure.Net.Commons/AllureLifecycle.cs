using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Threading;
using Allure.Net.Commons.Configuration;
using Allure.Net.Commons.Functions;
using Allure.Net.Commons.TestPlan;
using Allure.Net.Commons.Writer;
using Newtonsoft.Json.Linq;

#nullable enable

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

    /// <summary>
    /// The list of the currently registered formatters used by Allure to
    /// convert test and step arguments to strings.
    /// </summary>
    public IReadOnlyDictionary<Type, ITypeFormatter> TypeFormatters =>
        new ReadOnlyDictionary<Type, ITypeFormatter>(typeFormatters);

    readonly AsyncLocal<AllureContext> context = new();

    readonly Lazy<AllureTestPlan> lazyTestPlan;

    readonly IAllureResultsWriter writer;

    internal IAllureResultsWriter Writer => this.writer;

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

    /// <summary>
    /// The current testplan. If no testplan was specified, the default one is
    /// used that doesn't filter any test.
    /// </summary>
    public AllureTestPlan TestPlan { get => this.lazyTestPlan.Value; }

    internal AllureLifecycle() : this(GetConfiguration())
    {
    }

    internal AllureLifecycle(
        Func<AllureConfiguration, IAllureResultsWriter> writerFactory
    ) : this(
        GetConfiguration(),
        writerFactory,
        AllureTestPlan.FromEnvironment
    )
    {
    }

    internal AllureLifecycle(JObject config) : this(
        config,
        c => new FileSystemResultsWriter(c),
        AllureTestPlan.FromEnvironment
    )
    {

    }

    internal AllureLifecycle(
        JObject config,
        Func<AllureConfiguration, IAllureResultsWriter> writerFactory,
        Func<AllureTestPlan> testPlanFactory
    )
    {
        JsonConfiguration = config.ToString();
        AllureConfiguration = AllureConfiguration.ReadFromJObject(config);
        writer = writerFactory(AllureConfiguration);
        lazyTestPlan = new(testPlanFactory);
    }

    /// <summary>
    /// The JSON representation of the current Allure configuration.
    /// </summary>
    public string JsonConfiguration { get; private set; }

    /// <summary>
    /// The current Allure configuration.
    /// </summary>
    public AllureConfiguration AllureConfiguration { get; }

    /// <summary>
    /// The full path to the Allure results directory.
    /// </summary>
    public string ResultsDirectory => writer.ToString();

    /// <summary>
    /// The current instance of the Allure lifecycle.
    /// </summary>
    public static AllureLifecycle Instance { get => instance.Value; }

    /// <summary>
    /// Registers a type formatter to be used when converting a test's or
    /// step's argument to the string that will be included in the Allure
    /// report.
    /// </summary>
    /// <typeparam name="T">
    /// The type that the formatter converts. The formatter will be used for
    /// arguments of that exact type. Otherwise, the argument will be converted
    /// using JSON serialization.
    /// </typeparam>
    /// <param name="typeFormatter">The formatter instance.</param>
    public void AddTypeFormatter<T>(TypeFormatter<T> typeFormatter) =>
        AddTypeFormatterImpl(typeof(T), typeFormatter);

    private void AddTypeFormatterImpl(Type type, ITypeFormatter formatter) =>
        typeFormatters[type] = formatter;

    /// <summary>
    /// Binds the provided value as the current Allure context and executes
    /// the specified function. The context is then restored to the initial
    /// value. That allows the Allure context to bypass .NET execution
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
    /// Prepares a new test and activates the test context with it. The test
    /// should be then started separately with <see cref="StartTestCase()"/>
    /// </summary>
    /// <remarks>
    /// This method modifies the Allure context.<br></br>
    /// Requires the test context to be active.
    /// </remarks>
    /// <param name="testResult">A new test case.</param>
    /// <exception cref="InvalidOperationException"/>
    public virtual AllureLifecycle ScheduleTestCase(TestResult testResult)
    {
        var uuid = testResult.uuid;
        testResult.stage = Stage.scheduled;
        var containers = this.Context.ContainerContext;
        lock (this.modelMonitor)
        {
            foreach (TestResultContainer container in containers)
            {
                container.children.Add(uuid);
            }
        }
        this.UpdateContext(c => c.WithTestContext(testResult));
        return this;
    }


    /// <summary>
    /// Starts a previously scheduled test.
    /// </summary>
    /// <remarks>
    /// Requires the test context to be active.
    /// </remarks>
    /// <exception cref="InvalidOperationException"/>
    public virtual AllureLifecycle StartTestCase() =>
        this.UpdateTestCase(startAllureItem);

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
    public virtual AllureLifecycle StartTestCase(TestResult testResult) =>
        this.ScheduleTestCase(testResult)
            .UpdateTestCase(startAllureItem);

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
        Chain(beforeStop, stopTestCase)
    );

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
        var testResult = this.Context.CurrentTest;
        string uuid;
        lock (this.modelMonitor)
        {
            uuid = testResult.uuid;
        }
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

    #region Executable item

    /// <summary>
    /// If the step context is active, updates the current step.
    /// Otherwise, if the fixture context is active, updates the current fixture.
    /// Otherwise, updates the current test.
    /// Fails if neither test, nor fixture, nor step context is active.
    /// </summary>
    /// <remarks>
    /// The method is intended to be used by authors of integrations.
    /// </remarks>
    /// <param name="updateItem">The update callback.</param>
    public virtual AllureLifecycle UpdateExecutableItem(
        Action<ExecutableItem> updateItem
    )
    {
        var item = this.Context.CurrentStepContainer;
        lock (this.modelMonitor)
        {
            updateItem(item);
        }
        return this;
    }

    #endregion

    #region Extensions

    /// <summary>
    /// Removes all files and folders in the current Allure results directory.
    /// </summary>
    public virtual void CleanupResultDirectory()
    {
        writer.CleanUp();
    }

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

    static readonly Action<TestResult> stopTestCase = Chain(
        stopAllureItem,
        (TestResult tr) => tr.historyId ??= IdFunctions.CreateHistoryId(
            tr.fullName,
            tr.parameters
        ),
        (TestResult tr) => tr.testCaseId ??= IdFunctions.CreateTestCaseId(
            tr.fullName
        )
    );

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
}