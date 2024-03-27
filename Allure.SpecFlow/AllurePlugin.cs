using Allure.SpecFlowPlugin;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;
using TechTalk.SpecFlow.UnitTestProvider;

[assembly: RuntimePlugin(typeof(AllurePlugin))]

namespace Allure.SpecFlowPlugin
{
    public class AllurePlugin : IRuntimePlugin
    {
        public void Initialize(
            RuntimePluginEvents runtimePluginEvents,
            RuntimePluginParameters runtimePluginParameters,
            UnitTestProviderConfiguration unitTestProviderConfiguration
        )
        {
            runtimePluginEvents.CustomizeGlobalDependencies +=
                (sender, args) => args.ObjectContainer
                    .RegisterTypeAs<AllureBindingInvoker, IBindingInvoker>();

            runtimePluginEvents.CustomizeTestThreadDependencies +=
                (sender, args) => args.ObjectContainer
                    .RegisterTypeAs<AllureTestTracerWrapper, ITestTracer>();
        }
    }
}