using System;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Plugins;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Tracing;
using BoDi;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;
using System.Collections;

[assembly: RuntimePlugin(typeof(Allure.SpecFlowPlugin.AllurePlugin))]
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

            //runtimePluginEvents.CustomizeTestThreadDependencies += (sender, args) =>
            //{
            //    var registrations = args.ObjectContainer.GetType()
            //    .GetField("registrations", BindingFlags.NonPublic | BindingFlags.Instance)
            //    .GetValue(args.ObjectContainer);

            //    args.ObjectContainer.RegisterTypeAs<TestExecutionEngine, ITestExecutionEngine>("CurrentTestExecutionEngine");
            //    args.ObjectContainer.RegisterTypeAs<AllureTestExecutionEngine, ITestExecutionEngine>();

            //};
        }
    }
}
