using Allure.Commons;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;

namespace Allure.SpecFlowPlugin
{
    class AllureBindingInvoker : BindingInvoker
    {
        public AllureBindingInvoker(SpecFlowConfiguration specFlowConfiguration, IErrorProvider errorProvider) : base(specFlowConfiguration, errorProvider)
        {
        }

        public override object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration)
        {
            return AllureInvoke(binding, contextManager, arguments, testTracer, out duration);
        }

        private object AllureInvoke(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration)
        {
            var hook = binding as HookBinding;

            // skip Allure hooks
            // TODO: make them scoped
            if (hook != null && hook.HookOrder != int.MinValue && hook.HookOrder != int.MaxValue)
            {
                // create empty container to handle BeforeTestRun
                var container = Allure.CreateContainer();
                try
                {
                    switch (hook.HookType)
                    {
                        case HookType.BeforeTestRun:
                            Allure.Lifecycle.StartTestContainer(container);
                            break;

                        case HookType.BeforeFeature:
                        case HookType.AfterFeature:
                            contextManager.FeatureContext.TryGetValue(out container);
                            break;

                        case HookType.BeforeScenario:
                        case HookType.AfterScenario:
                        case HookType.BeforeScenarioBlock:
                        case HookType.AfterScenarioBlock:
                        case HookType.BeforeStep:
                        case HookType.AfterStep:
                            contextManager.ScenarioContext.TryGetValue(out container);
                            break;

                        case HookType.AfterTestRun:
                        default:
                            break;
                    }

                    var fixture = new FixtureResult()
                    {
                        name = hook.HookType.ToString()
                    };

                    if (hook.HookType.ToString().StartsWith("Before"))
                        Allure.Lifecycle.StartBeforeFixture(
                                container.uuid,
                                hook.GetHashCode().ToString(),
                                fixture);
                    else
                        Allure.Lifecycle.StartAfterFixture(
                                container.uuid,
                                hook.GetHashCode().ToString(),
                                fixture);

                    Allure.Lifecycle.StartStep(
                        Allure.Uuid,
                        new StepResult()
                        {
                            name = hook.Method.Name,
                            parameters = new List<Parameter>()
                            {
                                    new Parameter()
                                    {
                                        name = "HookOrder",
                                        value = hook.HookOrder.ToString()
                                    }
                            }
                        });

                    var result = base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);

                    Allure.Lifecycle
                        .StopStep(x => x.status = Status.passed)
                        .StopFixture(x => x.status = Status.passed);

                    return result;


                }

                // Exit point. Stop and write scenario.
                catch (Exception ex)
                {
                    Allure.Lifecycle
                        .StopStep(x =>
                        {
                            x.status = Status.broken;
                        })
                        .StopFixture(x =>
                        {
                            x.status = Status.broken;
                        });

                    // Create empty scenario if it doesn't exist.
                    if (!(contextManager.ScenarioContext != null &&
                        contextManager.ScenarioContext.TryGetValue(out TestResult testCase)))
                    {
                        testCase = Allure.GetScenario(contextManager.FeatureContext?.FeatureInfo, hook.HookType.ToString());
                        Allure.Lifecycle.StartTestCase(container.uuid, testCase);
                    }

                    Allure.Lifecycle
                        .UpdateTestCase(testCase.uuid, x =>
                            {
                                x.status = Status.broken;
                                x.statusDetails = new StatusDetails()
                                {
                                    message = ex.Message,
                                    trace = ex.StackTrace
                                };
                            })
                        .StopTestCase(testCase.uuid)
                        .WriteTestCase(testCase.uuid);

                    throw;
                }
            }

            else
                return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
        }

    }
    //class AllureBindingInvoker : BindingInvoker
    //{
    //    public AllureBindingInvoker(SpecFlowConfiguration specFlowConfiguration, IErrorProvider errorProvider) :
    //        base(specFlowConfiguration, errorProvider)
    //    { }

    //    public override object InvokeBinding(IBinding binding, IContextManager contextManager, object[] arguments, ITestTracer testTracer, out TimeSpan duration)
    //    {
    //        var hook = binding as HookBinding;
    //        var step = binding as StepDefinitionBinding;
    //        if (hook != null)
    //        {
    //            try
    //            {
    //                if (hook.HookType == HookType.BeforeFeature && hook.HookOrder == int.MinValue)
    //                    AllureAdapter.Instance.StartSuite(contextManager.FeatureContext.FeatureInfo);

    //                if (hook.HookType == HookType.AfterFeature && hook.HookOrder == int.MaxValue)
    //                    AllureAdapter.Instance.FinishSuite(contextManager.FeatureContext.FeatureInfo);

    //                return base.InvokeBinding(hook, contextManager, arguments, testTracer, out duration);
    //            }
    //            catch (Exception ex)
    //            {
    //                switch (hook.HookType)
    //                {
    //                    case HookType.BeforeTestRun:
    //                    case HookType.BeforeFeature:
    //                    case HookType.BeforeScenarioBlock:
    //                        var scenarioInfo = new ScenarioInfo(hook.HookType.ToString());
    //                        AllureAdapter.Instance.FailTestSuite(contextManager.FeatureContext.FeatureInfo, scenarioInfo, ex);
    //                        break;

    //                    case HookType.BeforeScenario:
    //                        AllureAdapter.Instance.CancelTestCase(ex);
    //                        break;

    //                    case HookType.AfterScenario:
    //                    case HookType.AfterStep:
    //                        AllureAdapter.Instance.FinishTestCase(contextManager.ScenarioContext, ex);
    //                        break;

    //                    case HookType.BeforeStep:
    //                    case HookType.AfterScenarioBlock:
    //                    case HookType.AfterFeature:
    //                    case HookType.AfterTestRun:
    //                    default:
    //                        break;
    //                }
    //                throw;
    //            }

    //        }

    //        return base.InvokeBinding(binding, contextManager, arguments, testTracer, out duration);
    //    }
    //}
}
