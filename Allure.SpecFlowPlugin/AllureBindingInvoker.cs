using System;
using System.Collections.Specialized;
using Allure.Net.Commons;
using TechTalk.SpecFlow;
using TechTalk.SpecFlow.Bindings;
using TechTalk.SpecFlow.Configuration;
using TechTalk.SpecFlow.ErrorHandling;
using TechTalk.SpecFlow.Infrastructure;
using TechTalk.SpecFlow.Tracing;


namespace Allure.SpecFlowPlugin
{
    using AllureBindingCall = Func<
        IBinding,
        IContextManager,
        object[],
        ITestTracer,
        (object, TimeSpan)
    >;

    internal class AllureBindingInvoker : BindingInvoker
    {
        const string PLACEHOLDER_TESTCASE_KEY =
            "Allure.SpecFlowPlugin.HAS_PLACEHOLDER_TESTCASE";

        static readonly AllureLifecycle allure = AllureLifecycle.Instance;

        public AllureBindingInvoker(
            SpecFlowConfiguration specFlowConfiguration,
            IErrorProvider errorProvider,
            ISynchronousBindingDelegateInvoker synchronousBindingDelegateInvoker
        ) : base(
            specFlowConfiguration,
            errorProvider,
            synchronousBindingDelegateInvoker
        )
        {
        }

        public override object InvokeBinding(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            out TimeSpan duration
        )
        {
            if (binding is HookBinding hook)
            {
                (var result, duration) = this.ProcessHook(
                    binding,
                    contextManager,
                    arguments,
                    testTracer,
                    hook
                );
                return result;
            }
            return base.InvokeBinding(
                binding,
                contextManager,
                arguments,
                testTracer,
                out duration
            );
        }

        (object, TimeSpan) ProcessHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            IsAllureHook(hook) ? this.InvokeAllureHookBinding(
                binding,
                contextManager,
                arguments,
                testTracer,
                hook
            ) : this.MakeFixtureFromFeatureOrScenarioHook(
                binding,
                contextManager,
                arguments,
                testTracer,
                hook
            );

        (object, TimeSpan) MakeFixtureFromFeatureOrScenarioHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            hook.HookType switch
            {
                HookType.BeforeFeature =>
                    this.MakeFixtureFromBeforeFeatureHook(
                        binding,
                        contextManager,
                        arguments,
                        testTracer,
                        hook
                    ),
                HookType.BeforeScenario =>
                    this.MakeFixtureFromBeforeScenarioHook(
                        binding,
                        contextManager,
                        arguments,
                        testTracer,
                        hook
                    ),
                HookType.BeforeStep or HookType.AfterStep =>
                    this.ProcessStepHook(
                        binding,
                        contextManager,
                        arguments,
                        testTracer
                    ),
                HookType.AfterScenario =>
                    this.MakeFixtureFromAfterScenarioHook(
                        binding,
                        contextManager,
                        arguments,
                        testTracer,
                        hook
                    ),
                HookType.AfterFeature =>
                    this.MakeFixtureFromAfterFeatureHook(
                        binding,
                        contextManager,
                        arguments,
                        testTracer,
                        hook
                    ),
                _ => this.CallBaseInvokeBinding(
                    binding,
                    contextManager,
                    arguments,
                    testTracer
                )
            };

        (object, TimeSpan) ProcessStepHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer
        )
        {
            try
            {
                return this.CallBaseInvokeBinding(
                    binding,
                    contextManager,
                    arguments,
                    testTracer
                );
            }
            catch (Exception ex)
            {
                ReportStepError(ex);
                throw;
            }
        }

        (object, TimeSpan) MakeFixtureFromBeforeFeatureHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            this.MakeFixtureFromFeatureHook(
                StartBeforeFixture,
                _ => { },
                binding,
                contextManager,
                arguments,
                testTracer,
                hook
            );

        (object, TimeSpan) MakeFixtureFromAfterFeatureHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            PluginHelper.UseCapturedAllureContext(
                contextManager.FeatureContext,
                () => this.MakeFixtureFromFeatureHook(
                    StartAfterFixture,
                    _ => AllureBindings.LastAfterFeature(),
                    binding,
                    contextManager,
                    arguments,
                    testTracer,
                    hook
                )
            );

        (object, TimeSpan) MakeFixtureFromFeatureHook(
            Action<HookBinding> startFixture,
            Action<FeatureContext> callLastHook,
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        )
        {
            object result;
            TimeSpan duration;

            startFixture(hook);
            try
            {
                result = base.InvokeBinding(
                    binding,
                    contextManager,
                    arguments,
                    testTracer,
                    out duration
                );
            }
            catch (Exception ex)
            {
                var featureContext = contextManager.FeatureContext;
                ReportFeatureFixtureError(featureContext, ex);
                callLastHook(featureContext);
                throw;
            }
            allure.StopFixture(MakePassed);

            return (result, duration);
        }

        (object, TimeSpan) MakeFixtureFromBeforeScenarioHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            this.MakeFixtureFromScenarioHook(
                AllureBindings.LastBeforeScenario,
                StartBeforeFixture,
                binding,
                contextManager,
                arguments,
                testTracer,
                hook
            );

        (object, TimeSpan) MakeFixtureFromAfterScenarioHook(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            this.MakeFixtureFromScenarioHook(
                (_, sc) => AllureBindings.LastAfterScenario(sc),
                StartAfterFixture,
                binding,
                contextManager,
                arguments,
                testTracer,
                hook
            );

        (object, TimeSpan) MakeFixtureFromScenarioHook(
            Action<FeatureContext, ScenarioContext> callLastHook,
            Action<HookBinding> startFixture,
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        )
        {
            (object, TimeSpan) result;

            startFixture(hook);
            try
            {
                result = this.CallBaseInvokeBinding(
                    binding,
                    contextManager,
                    arguments,
                    testTracer
                );
            }
            catch (Exception ex)
            {
                ReportScenarioFixtureError(ex);

                // SpecFlow doesn't call the remained hooks in case of an
                // exception is thrown. We have to call them explicitly to
                // ensure side effects on the Allure context are properly
                // applied.
                callLastHook(
                    contextManager.FeatureContext,
                    contextManager.ScenarioContext
                );

                throw;
            }

            allure.StopFixture(MakePassed);
            return result;
        }

        (object, TimeSpan) InvokeAllureHookBinding(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer,
            HookBinding hook
        ) =>
            this.ResolveAllureBindingCall(hook)(
                binding,
                contextManager,
                arguments,
                testTracer
            );

        AllureBindingCall ResolveAllureBindingCall(HookBinding hook) =>
            hook.HookType is HookType.AfterFeature
                ? this.CallBaseInvokeBindingInFeatureContext
                : this.CallBaseInvokeBinding;

        (object, TimeSpan) CallBaseInvokeBinding(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer
        ) =>
            (base.InvokeBinding(
                binding,
                contextManager,
                arguments,
                testTracer,
                out var duration
            ), duration);

        (object, TimeSpan) CallBaseInvokeBindingInFeatureContext(
            IBinding binding,
            IContextManager contextManager,
            object[] arguments,
            ITestTracer testTracer
        ) =>
            PluginHelper.UseCapturedAllureContext(
                contextManager.FeatureContext,
                () => this.CallBaseInvokeBinding(
                    binding,
                    contextManager,
                    arguments,
                    testTracer
                )
            );

        static void ReportFeatureFixtureError(
            FeatureContext featureContext,
            Exception error
        )
        {
            var makeBroken = WrapMakeBroken(error);
            allure.StopFixture(makeBroken);

            // Create one placeholder test case per failed feature-level hook
            // to indicate the error.
            if (!featureContext.ContainsKey(PLACEHOLDER_TESTCASE_KEY))
            {
                PluginHelper.StartTestCase(featureContext.FeatureInfo, new(
                    "Feature hook failure placeholder",
                    string.Format(
                        "This is a placeholder scenario to indicate an " +
                            "exception occured in a feature-level fixture " +
                            "of '{0}'",
                        featureContext.FeatureInfo.Title
                    ),
                    Array.Empty<string>(),
                    new OrderedDictionary()
                ));

                allure
                    .StopTestCase(makeBroken)
                    .WriteTestCase();

                featureContext.Add(PLACEHOLDER_TESTCASE_KEY, true);
            }
        }

        static void ReportScenarioFixtureError(Exception error)
        {
            var status = PluginHelper.IsIgnoreException(error)
                ? Status.skipped
                : Status.broken;
            var statusDetails = PluginHelper.GetStatusDetails(error);

            allure.StopFixture(
                PluginHelper.WrapStatusUpdate(status, statusDetails)
            );

            // If there is a scenario with no previous error, we update its
            // status here (this is the case for AfterScenraio hooks).
            // Otherwise (BeforeScenario) the scenario is updated later based
            // on the information provided by SpecFlow.
            if (allure.Context.HasTest)
            {
                allure.UpdateTestCase(
                    PluginHelper.WrapStatusOverwrite(
                        status,
                        statusDetails,
                        Status.none,
                        Status.passed
                    )
                );
            }
        }

        static void ReportStepError(Exception error)
        {
            if (allure.Context.HasStep)
            {
                MakeStepBroken(error);
            }
            MakeTestCaseBroken(error);
        }

        static void StartBeforeFixture(HookBinding hook) =>
            allure.StartBeforeFixture(
                PluginHelper.GetFixtureResult(hook)
            );

        static void StartAfterFixture(HookBinding hook) =>
            allure.StartAfterFixture(
                PluginHelper.GetFixtureResult(hook)
            );

        static bool IsAllureHook(HookBinding hook) =>
            hook.Method.Type.FullName == typeof(AllureBindings).FullName;

        static Action<ExecutableItem> WrapMakeBroken(Exception error) =>
            PluginHelper.WrapStatusOverwrite(
                Status.broken,
                PluginHelper.GetStatusDetails(error),
                Status.none,
                Status.passed
            );

        static void MakePassed(ExecutableItem item) =>
            item.status = Status.passed;

        static void MakeTestCaseBroken(Exception error) =>
            allure.UpdateTestCase(
                WrapMakeBroken(error)
            );

        static void MakeStepBroken(Exception error) =>
            allure.UpdateStep(
                WrapMakeBroken(error)
            );
    }
}