using System.Collections.Generic;
using Allure.ReqnrollPlugin.Configuration;
using Allure.ReqnrollPlugin.Events;
using Allure.ReqnrollPlugin.SelectiveRun;
using Allure.ReqnrollPlugin.State;
using Reqnroll;
using Reqnroll.BoDi;
using Reqnroll.Events;
using Reqnroll.Infrastructure;
using Reqnroll.Plugins;
using Reqnroll.UnitTestProvider;

[assembly: RuntimePlugin(typeof(Allure.ReqnrollPlugin.AllurePlugin))]

namespace Allure.ReqnrollPlugin;

public class AllurePlugin : IRuntimePlugin
{
    public void Initialize(
        RuntimePluginEvents runtimePluginEvents,
        RuntimePluginParameters runtimePluginParameters,
        UnitTestProviderConfiguration unitTestProviderConfiguration
    )
    {
        runtimePluginEvents.CustomizeTestThreadDependencies += SetupAllure;
    }

    static void SetupAllure(
        object _,
        CustomizeTestThreadDependenciesEventArgs args
    )
    {
        var container = args.ObjectContainer;

        SetUpTestPlanSupport(container);
        RegisterEventHandlers(container);
    }

    static void RegisterEventHandlers(IObjectContainer container)
    {
        if (!container.IsRegistered<ITestThreadExecutionEventPublisher>())
        {
            return;
        }
        var publisher = container.Resolve<ITestThreadExecutionEventPublisher>();
        foreach (var handler in CreateHandlers(container))
        {
            handler.Register(publisher);
        }
    }

    static void SetUpTestPlanSupport(ObjectContainer container)
    {
        var type = AllureReqnrollConfiguration.CurrentConfig.RunnerType;
        container.RegisterFactoryAs<ITestRunner>(
            () => new TestPlanAwareTestRunner(
                container.Resolve<IUnitTestRuntimeProvider>(),
                container.Resolve<ITestRunnerManager>(),
                container.Resolve<ITestRunner>(type.FullName)
            )
        );
        container.RegisterTypeAs<ITestRunner>(type, type.FullName);
    }

    static IEnumerable<AllureReqnrollEventHandler> CreateHandlers(
        IObjectContainer container
    )
    {
        yield return new TestRunStartedEventHandler(stateTransportFactory);
        yield return new HookStartedEventHandler(
            stateTransportFactory,
            runnerManagerFactory,
            contextFactory
        );
        yield return new HookBindingStartedEventHandler(stateTransportFactory);
        yield return new HookBindingFinishedEventHandler(stateTransportFactory);
        yield return new HookFinishedEventHandler(
            stateTransportFactory,
            runnerManagerFactory
        );
        yield return new StepStartedEventHandler(stateTransportFactory);
        yield return new StepFinishedEventHandler(stateTransportFactory);
        yield return new ScenarioFinishedEventHandle(stateTransportFactory);
        yield return new TestOutputEventHandler(stateTransportFactory);
        yield return new FeatureFinishedEventHandler(stateTransportFactory);

        CrossBindingContextTransport stateTransportFactory() => new(
            container.Resolve<IContextManager>()
        );

        ITestRunnerManager runnerManagerFactory() =>
            container.Resolve<ITestRunnerManager>();

        IContextManager contextFactory() =>
            container.Resolve<IContextManager>();
    }
}
