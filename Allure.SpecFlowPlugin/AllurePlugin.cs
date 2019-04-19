using Allure.SpecFlowPlugin;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Tracing;

[assembly: RuntimePlugin(typeof(AllurePlugin))]

namespace Allure.SpecFlowPlugin
{
    public class AllurePlugin : IRuntimePlugin
    {
        public void Initialize(RuntimePluginEvents runtimePluginEvents, RuntimePluginParameters runtimePluginParameters)
        {
            runtimePluginEvents.CustomizeGlobalDependencies += (sender, args) =>
                args.ObjectContainer.RegisterTypeAs<AllureBindingInvoker, IBindingInvoker>();

            runtimePluginEvents.CustomizeTestThreadDependencies += (sender, args) =>
                args.ObjectContainer.RegisterTypeAs<AllureTestTracerWrapper, ITestTracer>();
        }
    }
}